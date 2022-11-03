#if hOPENVR && hVIVETRACKER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace Passer.Humanoid {
    using Passer.Tracking;
    //using UnityEditor.Build;

    [System.Serializable]
    public class ViveTrackerArm : ArmSensor {
        public ViveTrackerArm() {
            enabled = false;
        }

        public override string name {
            get { return "Vive Tracker"; }
        }

        private ViveTrackerComponent viveTracker;

        public override Tracker.Status status {
            get {
                if (viveTracker == null)
                    return Tracker.Status.Unavailable;
                return viveTracker.status;
            }
            set { viveTracker.status = value; }
        }

        private static readonly Vector3 defaultLeftTrackerPosition = new Vector3(0, 0.05F, 0F);
        private static readonly Quaternion defaultLeftTrackerRotation = Quaternion.identity;
        private static readonly Vector3 defaultRightTrackerPosition = new Vector3(0, 0.05F, 0F);
        private static readonly Quaternion defaultRightTrackerRotation = Quaternion.identity;

#if hHI5
        private static readonly Vector3 hi5LeftTrackerPosition = new Vector3(0, 0.05F, -0.05F);
        private static readonly Quaternion hi5LeftTrackerRotation = Quaternion.Euler(15, -120, 35);
        private static readonly Vector3 hi5RightTrackerPosition = new Vector3(0, 0.05F, -0.05F);
        private static readonly Quaternion hi5RightTrackerRotation = Quaternion.Euler(15, -120, 35);
#endif

        public enum ArmBones {
            Hand,
            Forearm,
            UpperArm,
            Shoulder,
#if hHI5
            Hi5Glove,
#endif
        };
        public ArmBones attachedBone = ArmBones.Hand;

        #region Manage

        public override void SetSensor2Target() {
            if (sensorTransform == null)
                return;

            HumanoidTarget.TargetedBone targetBone = GetTargetBone();
            if (targetBone == null)
                return;

#if hHI5
            if (attachedBone == ArmBones.Hi5Glove) {
                if (enabled && sensorTransform != null) {
                    float xPos = handTarget.forearm.bone.length - 0.05F;
                    if (handTarget.isLeft) {
                        sensor2TargetPosition = -hi5LeftTrackerPosition - Vector3.left * xPos;
                        sensor2TargetRotation = Quaternion.Inverse(hi5LeftTrackerRotation);
                    }
                    else {
                        sensor2TargetPosition = -hi5RightTrackerPosition - Vector3.right * xPos;
                        sensor2TargetRotation = Quaternion.Inverse(hi5RightTrackerRotation);
                    }
                }
            }
            else
#endif
            {
                sensor2TargetRotation = Quaternion.Inverse(sensorTransform.rotation) * targetBone.target.transform.rotation;
                sensor2TargetPosition = -targetBone.target.transform.InverseTransformPoint(sensorTransform.position);
            }
        }

        protected HumanoidTarget.TargetedBone GetTargetBone() {
#if hHI5
            HumanoidTarget.TargetedBone targetBone;
            if (attachedBone == ArmBones.Hi5Glove)
                targetBone = handTarget.forearm;
            else
                targetBone = handTarget.GetTargetBone((Humanoid.ArmBones)attachedBone);
#else
            HumanoidTarget.TargetedBone targetBone = handTarget.GetTargetBone((Humanoid.ArmBones)attachedBone);
#endif
            return targetBone;
        }

        #endregion

        #region Start

        public override void Init(HandTarget _handTarget) {
            base.Init(_handTarget);
#if pUNITYXR
            tracker = handTarget.humanoid.unityXR;
#else
            tracker = handTarget.humanoid.openVR;
#endif
        }

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
#if pUNITYXR
            tracker = handTarget.humanoid.unityXR;
#else
            tracker = handTarget.humanoid.openVR;
#endif

            if (sensorTransform != null) {
                viveTracker = sensorTransform.GetComponent<ViveTrackerComponent>();
                if (viveTracker != null)
                    viveTracker.StartComponent(tracker.trackerTransform);
            }
        }

        protected override void CreateSensorTransform() {

            if (handTarget.isLeft)
                CreateSensorTransform("Vive Tracker", defaultLeftTrackerPosition, defaultLeftTrackerRotation);
            else
                CreateSensorTransform("Vive Tracker", defaultRightTrackerPosition, defaultRightTrackerRotation);

            ViveTrackerComponent viveTracker = sensorTransform.GetComponent<ViveTrackerComponent>();
            if (viveTracker == null)
                sensorTransform.gameObject.AddComponent<ViveTrackerComponent>();
        }

        #endregion

        #region Update

        public override void Update() {
#if UNITY_2017_2_OR_NEWER
            if (tracker == null || !tracker.enabled || !enabled || UnityVRDevice.xrDevice != UnityVRDevice.XRDeviceType.OpenVR)
#else
            if (tracker == null || !tracker.enabled || !enabled || VRSettings.loadedDeviceName != "OpenVR")
#endif
                return;

            HumanoidTarget.TargetedBone targetBone = GetTargetBone();

            if (viveTracker == null) {
                UpdateTarget(targetBone.target, sensorTransform);
                return;
            }

            if (viveTracker.trackerId < 0)
                viveTracker.trackerId = FindArmTracker(handTarget.isLeft);

            viveTracker.UpdateComponent();
            if (viveTracker.status != Tracker.Status.Tracking)
                return;

            base.UpdateTarget(targetBone.target, viveTracker);
        }

        protected int FindArmTracker(bool isLeft) {
#if hOPENVR
            List<ViveTrackerComponent> viveTrackers = handTarget.humanoid.openVR.viveTrackers;

            // We need the hmd to find the arm
            if (handTarget.humanoid.openVR.hmd == null)
                return -1;

            Transform hmdTransform = handTarget.humanoid.openVR.hmd.transform;
#endif
            ViveTrackerComponent foundTracker = null;
            // Finds the left or rightmost tracker, at least 0.2m left or right of the HMD
            Vector3 outermostLocalPos = new Vector3(isLeft ? -0.1F : 0.1F, 0, 0);
            foreach (ViveTrackerComponent viveTracker in viveTrackers) {
                // Is it tracking??
                if (viveTracker.positionConfidence <= 0)
                    continue;
                // Don't use trackers with specified hardwareIds
                if (viveTracker.useForBodyTracking == false)
                    continue;

                // Don't use trackers which are not used for body tracking
                if (viveTracker.useForBodyTracking == false)
                    continue;

                Vector3 sensorPos = viveTracker.transform.position;

                // Get HMD rotation projected on XZ plane
                Vector3 hmdForward = new Vector3(hmdTransform.forward.x, 0, hmdTransform.forward.z);
                Quaternion hmdFwdRotation = Quaternion.LookRotation(hmdForward);

                // Get Vive tracker local to the HMD position
                Vector3 sensorLocalPos = Quaternion.Inverse(hmdFwdRotation) * (sensorPos - hmdTransform.position);

                if ((isLeft && sensorLocalPos.x < outermostLocalPos.x && sensorLocalPos.x < -0.2F) ||
                    (!isLeft && sensorLocalPos.x > outermostLocalPos.x && sensorLocalPos.x > 0.2F)) {

                    foundTracker = viveTracker;
                    outermostLocalPos = sensorLocalPos;
                }
            }
            if (foundTracker != null) {
                int trackerId = foundTracker.trackerId;
                viveTrackers.Remove(foundTracker);
                Object.Destroy(foundTracker.gameObject);
                return trackerId;
            }
            else
                return -1;
        }

        public override void UpdateSensorTransformFromTarget(Transform _) {
            HumanoidTarget.TargetedBone targetBone = GetTargetBone();
            base.UpdateSensorTransformFromTarget(targetBone.target.transform);
        }

        #endregion

        public void ResetSensor() {
#if hOPENVR
            if (viveTracker != null) {
                List<ViveTrackerComponent> viveTrackers = handTarget.humanoid.openVR.viveTrackers;
                viveTrackers.Add(ViveTracker.NewViveTracker(handTarget.humanoid.openVR.tracker, (uint)viveTracker.trackerId));
            }
            viveTracker.trackerId = -1;
#endif
        }
    }
}
#endif