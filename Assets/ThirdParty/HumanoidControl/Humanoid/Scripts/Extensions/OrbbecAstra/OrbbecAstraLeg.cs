#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WSA_10_0)
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Passer.Humanoid.Tracking;

    [System.Serializable]
    public class AstraLeg : LegSensor {
        public override string name {
            get { return AstraDevice.name; }
        }

        private AstraTracker astraTracker;
        private SensorBone upperLegSensor;
        private SensorBone lowerLegSensor;
        private SensorBone footSensor;
        
        #region Start
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
            astraTracker = footTarget.humanoid.astra;
            tracker = astraTracker;

            if (astraTracker.device == null)
                return;

            Side side = footTarget.isLeft ? Side.Left : Side.Right;
            upperLegSensor = astraTracker.device.GetBone(0, side, SideBone.UpperLeg);
            lowerLegSensor = astraTracker.device.GetBone(0, side, SideBone.LowerLeg);
            footSensor = astraTracker.device.GetBone(0, side, SideBone.Foot);
        }
        #endregion

        #region Update
        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null ||
                    !tracker.enabled ||
                    !enabled ||
                    astraTracker.device == null ||
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

        private void UpdateUpperLeg(HumanoidTarget.TargetTransform upperLegTarget) {
            upperLegTarget.confidence.position = upperLegSensor.positionConfidence;
            if (upperLegTarget.confidence.position > 0)
                upperLegTarget.transform.position = upperLegSensor.position;
        }

        private void UpdateLowerLeg(HumanoidTarget.TargetTransform lowerLegTarget) {
            lowerLegTarget.confidence.position = lowerLegSensor.positionConfidence;
            if (lowerLegTarget.confidence.position > 0)
                lowerLegTarget.transform.position = lowerLegSensor.position;
        }

        private void UpdateFoot(HumanoidTarget.TargetTransform footTarget) {
            footTarget.confidence.position = footSensor.positionConfidence;
            if (footTarget.confidence.position > 0)
                footTarget.transform.position = footSensor.position;
        }

        #endregion
    }
}
#endif