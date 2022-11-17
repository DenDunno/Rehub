#if hKINECT2
using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    [System.Serializable]
    public class Kinect2Torso : TorsoSensor {
        public override string name {
            get { return Kinect2Device.name; }
        }

        private Kinect2Tracker kinectTracker;
        private SensorBone hipsSensor;
        private SensorBone spineSensor;
        private SensorBone chestSensor;

        #region Start
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            tracker = kinectTracker = hipsTarget.humanoid.kinect2;

            if (kinectTracker.device == null)
                return;

            hipsSensor = kinectTracker.device.GetBone(0, Bone.Hips);
            spineSensor = kinectTracker.device.GetBone(0, Bone.Spine);
            chestSensor = kinectTracker.device.GetBone(0, Bone.Chest);
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
            if (hipsSensor.positionConfidence == 0)
                return;

            UpdateHips(hipsTarget.hips.target);
            UpdateSpine(hipsTarget.spine.target);
            UpdateChest(hipsTarget.chest.target);
        }

        protected void UpdateHips(HumanoidTarget.TargetTransform hipsTarget) {
            hipsTarget.confidence.position = hipsSensor.positionConfidence;
            if (hipsTarget.confidence.position > 0)
                hipsTarget.transform.position = hipsSensor.position;
        }

        protected void UpdateSpine(HumanoidTarget.TargetTransform spineTarget) {
            spineTarget.confidence.position = spineSensor.positionConfidence;
            if (spineTarget.confidence.position > 0)
                spineTarget.transform.position = spineSensor.position;
        }

        protected void UpdateChest(HumanoidTarget.TargetTransform chestTarget) {
            chestTarget.confidence.position = chestSensor.positionConfidence;
            if (chestTarget.confidence.position > 0)
                chestTarget.transform.position = chestSensor.position;
        }
        #endregion
    }
}
#endif