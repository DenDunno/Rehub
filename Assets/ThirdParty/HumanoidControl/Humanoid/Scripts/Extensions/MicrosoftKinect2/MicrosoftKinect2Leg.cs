#if hKINECT2
using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    [System.Serializable]
    public class Kinect2Foot : LegSensor {
        public override string name {
            get { return Kinect2Device.name; }
        }

        private Kinect2Tracker kinectTracker;

        private SensorBone upperLegSensor;
        private SensorBone lowerLegSensor;
        private SensorBone footSensor;

        #region Start
        public override void Start(HumanoidControl humanoid, Transform targetTransform) {
            base.Start(humanoid, targetTransform);

            tracker = kinectTracker = footTarget.humanoid.kinect2;

            if (kinectTracker.device == null)
                return;

            upperLegSensor = kinectTracker.device.GetBone(0, footTarget.side, SideBone.UpperLeg);
            lowerLegSensor = kinectTracker.device.GetBone(0, footTarget.side, SideBone.LowerLeg);
            footSensor = kinectTracker.device.GetBone(0, footTarget.side, SideBone.Foot);
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
            if (footSensor.positionConfidence <= 0)
                return;

            UpdateUpperLeg(footTarget.upperLeg.target);
            UpdateLowerLeg(footTarget.lowerLeg.target);
            UpdateFoot(footTarget.foot.target);

            status = Tracker.Status.Tracking;
        }

        protected void UpdateUpperLeg(HumanoidTarget.TargetTransform upperLegTarget) {
            upperLegTarget.confidence.position = upperLegSensor.positionConfidence;
            if (upperLegTarget.confidence.position > 0)
                upperLegTarget.transform.position = upperLegSensor.position;
        }

        protected void UpdateLowerLeg(HumanoidTarget.TargetTransform lowerLegTarget) {
            lowerLegTarget.confidence.position = lowerLegSensor.positionConfidence;
            if (lowerLegTarget.confidence.position > 0)
                lowerLegTarget.transform.position = lowerLegSensor.position;
        }

        protected void UpdateFoot(HumanoidTarget.TargetTransform footTarget) {
            footTarget.confidence.position = footSensor.positionConfidence;
            if (footTarget.confidence.position > 0)
                footTarget.transform.position = footSensor.position;
        }

        #endregion
    }
}
#endif