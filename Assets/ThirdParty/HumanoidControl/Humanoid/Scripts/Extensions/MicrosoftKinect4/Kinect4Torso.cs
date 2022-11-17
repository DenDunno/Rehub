#if hKINECT4
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    [System.Serializable]
    public class Kinect4Torso : TorsoSensor {
        public override string name {
            get { return Kinect4Device.name; }
        }

        private AzureKinect azureKinect;
        private TrackedBone kinectHips;
        private TrackedBone kinectSpine;
        private TrackedBone kinectChest;

        #region Start
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            tracker = hipsTarget.humanoid.kinect4;
            azureKinect = hipsTarget.humanoid.kinect4.tracker;
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

            UpdateHips(hipsTarget.hips.target);
            UpdateSpine(hipsTarget.spine.target);
            UpdateChest(hipsTarget.chest.target);
        }

        protected void UpdateHips(HumanoidTarget.TargetTransform hipsTarget) {
            if (kinectHips == null) {
                kinectHips = azureKinect.GetBone(Bone.Hips);
                if (kinectHips == null)
                    return;
            }

            if (kinectHips.positionConfidence > hipsTarget.confidence.position) {
                hipsTarget.transform.position = kinectHips.transform.position;
                hipsTarget.confidence.position = kinectHips.positionConfidence;
                status = Tracker.Status.Tracking;
            }
            if (kinectHips.rotationConfidence > hipsTarget.confidence.rotation) {
                hipsTarget.transform.rotation = kinectHips.transform.rotation;
                hipsTarget.confidence.rotation = kinectHips.rotationConfidence;
            }
        }

        protected void UpdateSpine(HumanoidTarget.TargetTransform spineTarget) {
            if (kinectSpine == null) {
                kinectSpine = azureKinect.GetBone(Bone.Spine);
                if (kinectSpine == null)
                    return;
            }

            if (kinectSpine.positionConfidence > spineTarget.confidence.position) {
                spineTarget.transform.position = kinectSpine.transform.position;
                spineTarget.confidence.position = kinectSpine.positionConfidence;
            }
            if (kinectSpine.rotationConfidence > spineTarget.confidence.rotation) {
                spineTarget.transform.rotation = kinectSpine.transform.rotation;
                spineTarget.confidence.rotation = kinectSpine.rotationConfidence;
            }
        }

        protected void UpdateChest(HumanoidTarget.TargetTransform chestTarget) {
            if (kinectChest == null) {
                kinectChest = azureKinect.GetBone(Bone.Chest);
                if (kinectChest == null)
                    return;
            }

            if (kinectChest.positionConfidence > chestTarget.confidence.position) {
                chestTarget.transform.position = kinectChest.transform.position;
                chestTarget.confidence.position = kinectChest.positionConfidence;
            }
            if (kinectChest.rotationConfidence > chestTarget.confidence.rotation) {
                chestTarget.transform.rotation = kinectChest.transform.rotation;
                chestTarget.confidence.rotation = kinectChest.rotationConfidence;
            }
        }
        #endregion
    }
}
#endif