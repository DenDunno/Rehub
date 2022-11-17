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

    [System.Serializable]
    public class ViveTrackerHead : HeadSensor {
        public ViveTrackerHead() {
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

        private static readonly Vector3 defaultLocalTrackerPosition = new Vector3(0, 0.1F, 0.17F);
        private static readonly Quaternion defaultLocalTrackerRotation = Quaternion.Euler(270, 180, 0);

        #region Start

        public override void Init(HeadTarget _headTarget) {
            base.Init(_headTarget);
            if (headTarget.humanoid != null)
#if pUNITYXR
                tracker = headTarget.humanoid.unityXR;
#else
                tracker = headTarget.humanoid.openVR;
#endif
        }

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);
#if pUNITYXR
            tracker = headTarget.humanoid.unityXR;
#else
            tracker = headTarget.humanoid.openVR;
#endif

            if (sensorTransform != null) {
                viveTracker = sensorTransform.GetComponent<ViveTrackerComponent>();
                if (viveTracker != null)
                    viveTracker.StartComponent(tracker.trackerTransform);
            }
        }

        protected override void CreateSensorTransform() {
            CreateSensorTransform("Vive Tracker", defaultLocalTrackerPosition, defaultLocalTrackerRotation);

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

            if (viveTracker == null) {
                UpdateTarget(headTarget.head.target, sensorTransform);
                return;
            }

            if (viveTracker.trackerId < 0)
                viveTracker.trackerId = FindHeadTracker();

            viveTracker.UpdateComponent();
            if (viveTracker.status != Tracker.Status.Tracking)
                return;

            UpdateTarget(headTarget.head.target, viveTracker);
            UpdateNeckTargetFromHead();
        }

        public int FindHeadTracker() {
#if hOPENVR
            List<ViveTrackerComponent> viveTrackers = headTarget.humanoid.openVR.viveTrackers;
#endif

            ViveTrackerComponent foundTracker = null;
            // Finds a tracker at least 1.2m above the ground
            foreach (ViveTrackerComponent viveTracker in viveTrackers) {
                // Is it tracking??
                if (viveTracker.positionConfidence <= 0)
                    continue;

                // Don't use trackers which are not used for body tracking
                if (viveTracker.useForBodyTracking == false)
                    continue;

                Vector3 sensorPos = viveTracker.transform.position;
                float sensorTrackingHeight = sensorPos.y - tracker.trackerTransform.position.y;

                if (sensorTrackingHeight > 1.2F) // head is more than 1.2 meter above the ground
                    foundTracker = viveTracker;
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
        #endregion
    }
}
#endif