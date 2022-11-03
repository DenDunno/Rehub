#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WSA_10_0)
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Passer.Humanoid.Tracking;

    [System.Serializable]
    public class AstraTorso : TorsoSensor {
        public override string name {
            get { return AstraDevice.name; }
        }

        private AstraTracker astraTracker;
        private SensorBone hipsSensor;
        private SensorBone spineSensor;
        private SensorBone chestSensor;
        private SensorBone neckSensor;

        protected HumanoidControl humanoid;

        #region Start
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
            humanoid = _humanoid;

            astraTracker = hipsTarget.humanoid.astra;
            tracker = astraTracker;

            if (astraTracker.device == null)
                return;

            hipsSensor = astraTracker.device.GetBone(0, Bone.Hips);
            spineSensor = astraTracker.device.GetBone(0, Bone.Spine);
            chestSensor = astraTracker.device.GetBone(0, Bone.Chest);
            neckSensor = astraTracker.device.GetBone(0, Bone.Neck);
        }
        #endregion

        #region Update

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || !tracker.enabled || !enabled || astraTracker.device == null || tracker.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;
            if (hipsSensor.positionConfidence == 0)
                return;

            UpdateHips(hipsTarget.hips.target);
            UpdateSpine(hipsTarget.spine.target);
            UpdateChest(hipsTarget.chest.target);

            status = Tracker.Status.Tracking;
        }

        private void UpdateHips(HumanoidTarget.TargetTransform hipsTarget) {
            hipsTarget.confidence.position = hipsSensor.positionConfidence;
            if (hipsTarget.confidence.position > 0)
                hipsTarget.transform.position = hipsSensor.position;

            hipsTarget.confidence.rotation = (hipsSensor.positionConfidence + spineSensor.positionConfidence) / 2 * 0.8F;
            if (hipsTarget.confidence.rotation > 0) {
                Vector3 toSpine = spineSensor.position - hipsSensor.position;
                Quaternion rotation =
                    Quaternion.LookRotation(toSpine, -humanoid.transform.forward) *
                    Quaternion.AngleAxis(90, humanoid.transform.right);
                hipsTarget.transform.rotation = rotation;
            }
        }

        private void UpdateSpine(HumanoidTarget.TargetTransform spineTarget) {
            spineTarget.confidence.position = spineSensor.positionConfidence;
            if (spineTarget.confidence.position > 0)
                spineTarget.transform.position = spineSensor.position;

            spineTarget.confidence.rotation = (spineSensor.positionConfidence + chestSensor.positionConfidence) / 2 * 0.8F;
            if (spineTarget.confidence.rotation > 0) {
                Vector3 toChest = chestSensor.position - spineSensor.position;
                Quaternion rotation =
                    Quaternion.LookRotation(toChest, -humanoid.transform.forward) *
                    Quaternion.AngleAxis(90, humanoid.transform.right);
                spineTarget.transform.rotation = rotation;
            }
        }

        private void UpdateChest(HumanoidTarget.TargetTransform chestTarget) {
            chestTarget.confidence.position = chestSensor.positionConfidence;
            if (chestTarget.confidence.position > 0)
                chestTarget.transform.position = chestSensor.position;

            chestTarget.confidence.rotation = (chestSensor.positionConfidence + neckSensor.positionConfidence) / 2 * 0.8F;
            if (chestTarget.confidence.rotation > 0) {
                Vector3 toNeck = neckSensor.position - chestSensor.position;
                    Quaternion rotation =
                    Quaternion.LookRotation(toNeck, -humanoid.transform.forward) *
                    Quaternion.AngleAxis(90, humanoid.transform.right);
                chestTarget.transform.rotation = rotation;
            }

        }

        #endregion
    }
}
#endif