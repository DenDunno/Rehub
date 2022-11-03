#if hLEAP
using Leap.Unity;
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Passer.Humanoid.Tracking;

    [System.Serializable]
    public class LeapMotionHand : ArmSensor {
        public override string name {
            get { return LeapDevice.name; }
        }

        private Tracker.Status _status;
        public override Tracker.Status status {
            get { return _status; }
            set { _status = value; }
        }

        private LeapTracker leapTracker;
        private SensorBone forearmSensor;
        private SensorBone handSensor;
        private readonly SensorBone[,] fingerSensors = new SensorBone[5, 3];

#if UNITY_STANDALONE_WIN || (UNITY_2017_2_OR_NEWER && UNITY_WSA_10_0)

        #region Start

        public override void Init(HandTarget handTarget) {
            base.Init(handTarget);
            tracker = handTarget.humanoid.leapTracker;
        }

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
            leapTracker = handTarget.humanoid.leapTracker;
            tracker = leapTracker;

            if (leapTracker.device == null)
                return;

            Side side = handTarget.isLeft ? Side.Left : Side.Right;

            forearmSensor = leapTracker.device.GetBone(0, side, SideBone.Forearm);
            handSensor = leapTracker.device.GetBone(0, side, SideBone.Hand);

            for (int i = 0; i < (int)Finger.Count; i++)
                StartFinger(side, handTarget.fingers.allFingers[i], i);

            sensor2TargetPosition = Vector3.zero;
        }

        private void StartFinger(Side side, FingersTarget.TargetedFinger finger, int fingerIx) {
            SideBone sideBoneId = BoneReference.HumanoidSideBone((Finger)fingerIx, FingerBone.Proximal);
            fingerSensors[fingerIx, 0] = leapTracker.device.GetBone(0, side, sideBoneId);

            sideBoneId = BoneReference.HumanoidSideBone((Finger)fingerIx, FingerBone.Intermediate);
            fingerSensors[fingerIx, 1] = leapTracker.device.GetBone(0, side, sideBoneId);

            sideBoneId = BoneReference.HumanoidSideBone((Finger)fingerIx, FingerBone.Distal);
            fingerSensors[fingerIx, 2] = leapTracker.device.GetBone(0, side, sideBoneId);
        }

        protected override HandSkeleton FindHandSkeleton(bool isLeft) {
            if (tracker == null || tracker.trackerTransform == null)
                return null;

            LeapHandSkeleton[] handSkeletons = tracker.trackerTransform.GetComponentsInChildren<LeapHandSkeleton>();
            foreach (LeapHandSkeleton handSkeleton in handSkeletons) {
                if (handSkeleton.isLeft == isLeft)
                    return handSkeleton;
            }
            return null;
        }

        protected override HandSkeleton CreateHandSkeleton(bool isLeft, bool showRealObjects) {
            GameObject skeletonObj = new GameObject(isLeft ? "Left Hand Skeleton" : "Right Hand Skeleton");
            skeletonObj.transform.parent = tracker.trackerTransform;
            skeletonObj.transform.localPosition = Vector3.zero;
            skeletonObj.transform.localRotation = Quaternion.identity;

            LeapHandSkeleton handSkeleton = skeletonObj.AddComponent<LeapHandSkeleton>();
            handSkeleton.isLeft = isLeft;
            //handSkeleton.showSkeleton = showRealObjects;
            return handSkeleton;
        }
        #endregion

        #region Update

        // This correction can be used to correct for differences in 
        // avatar hand configuration and leap hand configuration
        public Vector3 rotationCorrection;
        [HideInInspector]
        private Quaternion rotationCorrectionQ;

        public override void Update() {
            status = Tracker.Status.Unavailable;

            if (tracker == null ||
                    !tracker.enabled ||
                    !enabled)
                return;

            if (handSkeleton == null)
                handSkeleton = CreateHandSkeleton(handTarget.isLeft, handTarget.showRealObjects);
            if (handSkeleton == null)
                return;

            status = Tracker.Status.Present;
            if (handSkeleton.status == Tracker.Status.Tracking) {
                UpdateHandFromSkeleton();
                status = Tracker.Status.Tracking;
            }
            handTarget.hand.target.confidence.position = handSkeleton.positionConfidence;
            handTarget.hand.target.confidence.rotation = handSkeleton.rotationConfidence;
        }

        protected void UpdateForearm(HumanoidTarget.TargetTransform forearmTarget) {
            if (forearmSensor.rotationConfidence > forearmTarget.confidence.rotation) {
                forearmTarget.transform.rotation = forearmSensor.rotation * rotationCorrectionQ;
                forearmTarget.confidence.rotation = forearmSensor.rotationConfidence;
            }
        }

        protected void UpdateHand() {
            if (handSensor.positionConfidence > handTarget.hand.target.confidence.position) {
                handTarget.hand.target.transform.position = handSensor.position;
                handTarget.hand.target.confidence.position = handSensor.positionConfidence;
            }
            if (handSensor.rotationConfidence > handTarget.hand.target.confidence.rotation) {
                handTarget.hand.target.transform.rotation = handSensor.rotation * rotationCorrectionQ;
                handTarget.hand.target.confidence.rotation = handSensor.rotationConfidence;
            }
        }

        protected virtual void UpdateFingers() {
            if (handTarget.hand.target.confidence.position == 0)
                return;

            Quaternion baseRotation =
                handTarget.hand.target.transform.rotation * Quaternion.Inverse(handSensor.rotation) * // = Rotation Correction
                tracker.trackerTransform.rotation;

            for (int i = 0; i < (int)Finger.Count; i++)
                UpdateFinger(baseRotation, handTarget.fingers.allFingers[i], i);

            handTarget.fingers.DetermineFingerCurl();
        }

        private void UpdateFinger(Quaternion baseRotation, FingersTarget.TargetedFinger finger, int fingerIx) {
            finger.proximal.target.transform.rotation = baseRotation * fingerSensors[fingerIx, 0].rotation;
            finger.intermediate.target.transform.rotation = baseRotation * fingerSensors[fingerIx, 1].rotation;
            finger.distal.target.transform.rotation = baseRotation * fingerSensors[fingerIx, 2].rotation;
        }

        #region Leap Package Support

        private Leap.Hand leapHand;

        public void SetHand(Leap.Hand hand) {
            leapHand = hand;
        }

        private void UpdateLeapHand() {
            if (leapHand == null)
                return;

            float handConfidence = leapHand.Confidence * 0.9F;

            if (sensorTransform == null) {
                GameObject leapHandSensor = new GameObject("LeapHandSensor");
                sensorTransform = leapHandSensor.transform;
            }

            sensorTransform.position = leapHand.WristPosition.ToVector3();
            sensorTransform.rotation = leapHand.Rotation.ToQuaternion() * Quaternion.AngleAxis(handTarget.isLeft ? 90 : -90, Vector3.up);

            handTarget.hand.target.transform.position = sensorTransform.position;
            handTarget.hand.target.confidence.position = handConfidence;
            handTarget.hand.target.transform.rotation = sensorTransform.rotation;
            handTarget.hand.target.confidence.rotation = handConfidence;
        }

        private void UpdateLeapFingers() {
            if (leapHand == null)
                return;

            if (handTarget.hand.target.confidence.position == 0)
                return;

            for (int i = 0; i < (int)Finger.Count; i++)
                UpdateLeapFinger(handTarget.fingers.allFingers[i], i);

            handTarget.fingers.DetermineFingerCurl();

            Vector3 leapIndexProximal = leapHand.Fingers[1].bones[0].NextJoint.ToVector3();
            Vector3 leapLittleProximal = leapHand.Fingers[4].bones[0].NextJoint.ToVector3();
            Vector3 leapProximalMiddle = (leapIndexProximal + leapLittleProximal) / 2;

            Vector3 targetIndexProximal = handTarget.fingers.index.proximal.target.transform.position;
            Vector3 targetLittleProximal = handTarget.fingers.little.proximal.target.transform.position;
            Vector3 targetProximalMiddle = (targetIndexProximal + targetLittleProximal) / 2;

            Vector3 leap2target = targetProximalMiddle - leapProximalMiddle;
            handTarget.hand.target.transform.position -= leap2target;
        }

        private void UpdateLeapFinger(FingersTarget.TargetedFinger finger, int fingerIx) {
            Leap.Finger leapFinger = leapHand.Fingers[fingerIx];
            Quaternion proximalRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Rotation.ToQuaternion();
            finger.proximal.target.transform.rotation = proximalRotation * Quaternion.AngleAxis(handTarget.isLeft ? 90 : -90, Vector3.up);

            Quaternion intermediateRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Rotation.ToQuaternion();
            finger.intermediate.target.transform.rotation = intermediateRotation * Quaternion.AngleAxis(handTarget.isLeft ? 90 : -90, Vector3.up);

            Quaternion distalRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Rotation.ToQuaternion();
            finger.distal.target.transform.rotation = distalRotation * Quaternion.AngleAxis(handTarget.isLeft ? 90 : -90, Vector3.up);
        }


        #endregion

        #endregion
#endif
    }
}
#endif