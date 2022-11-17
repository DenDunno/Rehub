#if hKINECT2
using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    [System.Serializable]
    public class Kinect2Arm : ArmSensor {
        public override string name {
            get { return Kinect2Device.name; }
        }

        private Kinect2Tracker kinectTracker;

        private SensorBone upperArmSensor;
        private SensorBone forearmSensor;
        public SensorBone handSensor;

        public bool handTracking = true;

        public override void Start(HumanoidControl humanoid, Transform targetTransform) {
            base.Start(humanoid, targetTransform);

            tracker = kinectTracker = handTarget.humanoid.kinect2;

            if (kinectTracker.device == null)
                return;

            upperArmSensor = kinectTracker.device.GetBone(0, handTarget.side, SideBone.UpperArm);
            forearmSensor = kinectTracker.device.GetBone(0, handTarget.side, SideBone.Forearm);
            handSensor = kinectTracker.device.GetBone(0, handTarget.side, SideBone.Hand);
        }

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null ||
                    !tracker.enabled ||
                    !enabled ||
                    kinectTracker.device == null ||
                    tracker.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;
            if (handSensor.positionConfidence == 0)
                return;

            // We only check for right, because then the left has already been updated)
            if (!handTarget.isLeft) {
                if ((handTarget.hand.target.confidence.position >= handSensor.positionConfidence) &&
                        (handTarget.otherHand.hand.target.confidence.position >= handSensor.positionConfidence)) {
                    kinectTracker.CalibrateWithHands(handSensor, handTarget.otherHand.kinect2.handSensor);
                    return;
                }
            }

            UpdateArm();
            //if (handTracking)
            //    UpdateFingers(kinectArm);

            status = Tracker.Status.Tracking;
        }

        private void UpdateArm() {
            UpdateUpperArm(handTarget.upperArm.target);
            UpdateForearm(handTarget.forearm.target);
            UpdateHand(handTarget.hand.target);
        }

        private void UpdateUpperArm(HumanoidTarget.TargetTransform upperArmTarget) {
            if (upperArmSensor.positionConfidence > upperArmTarget.confidence.position) {
                upperArmTarget.transform.position = upperArmSensor.position;
                upperArmTarget.confidence.position = upperArmSensor.positionConfidence;
            }

            if (upperArmSensor.rotationConfidence > upperArmTarget.confidence.rotation) {
                upperArmTarget.transform.rotation = upperArmSensor.rotation;
                upperArmTarget.confidence.rotation = 0.8F; // upperArmSensor.rotationConfidence;
            }
        }

        private void UpdateForearm(HumanoidTarget.TargetTransform forearmTarget) {
            if (forearmSensor.positionConfidence > forearmTarget.confidence.position) {
                forearmTarget.transform.position = forearmSensor.position;
                forearmTarget.confidence.position = forearmSensor.positionConfidence;
            }

            Vector3 upperArmForward = forearmSensor.position - upperArmSensor.position;
            Vector3 forearmForward = handSensor.position - forearmSensor.position;
            Vector3 forearmUp = Vector3.Cross(upperArmForward, forearmForward);
            if (!handTarget.isLeft)
                forearmUp = -forearmUp;

            forearmTarget.transform.rotation = Quaternion.LookRotation(forearmForward, forearmUp);
            //forearmTarget.transform.rotation =  forearmSensor.rotation; is not good enough
            forearmTarget.confidence.rotation = forearmSensor.rotationConfidence;
        }

        private void UpdateHand(HumanoidTarget.TargetTransform handTarget) {
            if (handSensor.positionConfidence > handTarget.confidence.position) {
                handTarget.transform.position = handSensor.position;
                handTarget.confidence.position = handSensor.positionConfidence;
            }
            if (handSensor.rotationConfidence > handTarget.confidence.rotation) {
                handTarget.transform.rotation = handSensor.rotation;
                handTarget.confidence.rotation = 0.8F; // handSensor.rotationConfidence;
            }
        }

        protected override void UpdateFingers(Tracking.ArmSensor armSensor) {
            for (int i = 0; i < (int)Finger.Count; i++)
                handTarget.SetFingerCurl((Finger)i, armSensor.fingers[i].curl);
        }

        protected void UpdateFingers() {
            Kinect2Device.HandPose handPose = kinectTracker.device.GetHandPose(0, handTarget.isLeft);
            bool handLasso = (handPose == Kinect2Device.HandPose.Lasso);
            bool handClosed = (handPose == Kinect2Device.HandPose.Closed);

            handTarget.SetFingerCurl(Finger.Thumb, (handLasso || handClosed) ? 1 : 0);
            handTarget.SetFingerCurl(Finger.Index, handClosed ? 1 : 0);
            handTarget.SetFingerCurl(Finger.Middle, handClosed ? 1 : 0);
            handTarget.SetFingerCurl(Finger.Ring, (handLasso || handClosed) ? 1 : 0);
            handTarget.SetFingerCurl(Finger.Little, (handLasso || handClosed) ? 1 : 0);
        }
    }
}
#endif