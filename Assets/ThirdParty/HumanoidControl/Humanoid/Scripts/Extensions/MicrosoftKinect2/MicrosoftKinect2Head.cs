#if hKINECT2
using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    [System.Serializable]
    public class Kinect2Head : HeadSensor {
        public override string name {
            get { return Kinect2Device.name; }
        }

        [System.NonSerialized]
        private Kinect2Tracker kinectTracker;
        private SensorBone headSensor;
        private SensorBone neckSensor;

        public bool headTracking = true;
        public enum RotationTrackingAxis {
            XYZ,
            XY
        }

        public RotationTrackingAxis rotationTrackingAxis = RotationTrackingAxis.XY;

        #region Start
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
            kinectTracker = headTarget.humanoid.kinect2;
            tracker = kinectTracker;

            if (kinectTracker.device == null)
                return;

            headSensor = kinectTracker.device.GetBone(0, Bone.Head);
            neckSensor = kinectTracker.device.GetBone(0, Bone.Neck);
        }
        #endregion

        #region Update
        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null ||
                    !tracker.enabled ||
                    !enabled ||
                    kinectTracker.device == null ||
                    tracker.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;
            if (headSensor.positionConfidence == 0)
                return;

            if (headTracking)
                UpdateBones();
            
            status = Tracker.Status.Tracking;
        }

        #region Bones
        protected void UpdateBones() {
            if (headTarget.head.target.confidence.position > headSensor.positionConfidence) {
                if (headTarget.humanoid.leftHandTarget.hand.target.confidence.position > kinectTracker.device.GetBoneConfidence(0, Side.Left, SideBone.Hand) &&
                    headTarget.humanoid.rightHandTarget.hand.target.confidence.position > kinectTracker.device.GetBoneConfidence(0, Side.Right, SideBone.Hand))
                    kinectTracker.CalibrateWithHeadAndHands(headSensor, headTarget.humanoid.leftHandTarget.kinect2.handSensor, headTarget.humanoid.rightHandTarget.kinect2.handSensor);
                else
                    kinectTracker.CalibrateWithHead(headSensor);
                return;
            }

            UpdateNeck(headTarget.neck.target);
            UpdateHead(headTarget.head.target);
        }

        private void UpdateNeck(HumanoidTarget.TargetTransform neckTarget) {
            neckTarget.confidence.position = neckSensor.positionConfidence;
            if (neckTarget.confidence.position > 0)
                neckTarget.transform.position = neckSensor.position;
        }

        private void UpdateHead(HumanoidTarget.TargetTransform headTarget) {
            headTarget.confidence.position = headSensor.positionConfidence;
            if (headTarget.confidence.position > 0)
                headTarget.transform.position = headSensor.position;

            headTarget.confidence.position = headSensor.rotationConfidence;
            if (headTarget.confidence.rotation > 0)
                headTarget.transform.rotation = headSensor.rotation;

            if (rotationTrackingAxis == RotationTrackingAxis.XY)
                headTarget.transform.rotation = Quaternion.LookRotation(headTarget.transform.rotation * Vector3.forward);
        }

        #endregion

        #endregion
    }
}
#endif