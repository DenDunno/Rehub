#if hKINECT4
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    [System.Serializable]
    public class Kinect4Leg : LegSensor {
        public override string name {
            get { return Kinect4Device.name; }
        }

        private AzureKinect azureKinect;
        private TrackedBone kinectUpperLeg;
        private TrackedBone kinectLowerLeg;
        private TrackedBone kinectFoot;

        #region Start
        public override void Start(HumanoidControl humanoid, Transform targetTransform) {
            base.Start(humanoid, targetTransform);

            tracker = footTarget.humanoid.kinect4;
            azureKinect = footTarget.humanoid.kinect4.tracker;
        }
        #endregion

        #region Update

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || !tracker.enabled ||
                    !enabled ||
                    azureKinect.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;

            UpdateUpperLeg(footTarget.upperLeg.target);
            UpdateLowerLeg(footTarget.lowerLeg.target);
            UpdateFoot(footTarget.foot.target);
        }

        protected void UpdateUpperLeg(HumanoidTarget.TargetTransform upperLegTarget) {
            if (kinectUpperLeg == null) {
                kinectUpperLeg = azureKinect.GetBone(BoneReference.HumanoidBone(footTarget.side, SideBone.UpperLeg));
                if (kinectUpperLeg == null)
                    return;
            }

            if (kinectUpperLeg.positionConfidence > upperLegTarget.confidence.position) {
                upperLegTarget.transform.position = kinectUpperLeg.transform.position;
                upperLegTarget.confidence.position = kinectUpperLeg.positionConfidence;
            }

            if (kinectUpperLeg.rotationConfidence > upperLegTarget.confidence.rotation) {
                upperLegTarget.transform.rotation = kinectUpperLeg.transform.rotation;
                upperLegTarget.confidence.rotation = kinectUpperLeg.rotationConfidence;
            }
        }

        protected void UpdateLowerLeg(HumanoidTarget.TargetTransform lowerLegTarget) {
            if (kinectLowerLeg == null) {
                kinectLowerLeg = azureKinect.GetBone(BoneReference.HumanoidBone(footTarget.side, SideBone.LowerLeg));
                if (kinectLowerLeg == null)
                    return;
            }
            
            if (kinectLowerLeg.positionConfidence > lowerLegTarget.confidence.position) {
                lowerLegTarget.transform.position = kinectLowerLeg.transform.position;
                lowerLegTarget.confidence.position = kinectLowerLeg.positionConfidence;
            }
            if (kinectLowerLeg.rotationConfidence > lowerLegTarget.confidence.rotation) {
                lowerLegTarget.transform.rotation = kinectLowerLeg.transform.rotation;
                lowerLegTarget.confidence.rotation = kinectLowerLeg.rotationConfidence;
            }
        }

        protected void UpdateFoot(HumanoidTarget.TargetTransform footTargetBone) {
            if (kinectFoot == null) {
                kinectFoot = azureKinect.GetBone(BoneReference.HumanoidBone(footTarget.side, SideBone.Foot));
                if (kinectFoot == null)
                    return;
            }

            if (kinectFoot.positionConfidence > footTargetBone.confidence.position) {
                footTargetBone.transform.position = kinectFoot.transform.position;
                footTargetBone.confidence.position = kinectFoot.positionConfidence;
            }
            if (kinectFoot.rotationConfidence > footTargetBone.confidence.rotation) {
                footTargetBone.transform.rotation = kinectFoot.transform.rotation;
                footTargetBone.confidence.rotation = kinectFoot.rotationConfidence;
            }
        }        

        #endregion
    }
}
#endif