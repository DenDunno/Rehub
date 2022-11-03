#if hLEAP
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Passer.Humanoid.Tracking;
    using LeapInternal;

    [System.Serializable]
    public class LeapTracker : HumanoidTracker {

        public override string name {
            get { return LeapDevice.name; }
        }

        public LeapDevice device;
        public TrackerTransform leapTransform;
        public LeapMotionCamera[] leapCameras;

        public bool isHeadMounted = true;

        public bool useLeapPackage = false;
        private Leap.Unity.LeapProvider leapProvider;

        private readonly Vector3 defaultTrackerPosition = new Vector3(0, 1.2F, 0.3F);
        private readonly Quaternion defaultTrackerRotation = Quaternion.identity;

        private readonly Vector3 defaultHeadTrackerPosition = new Vector3(0, 0, 0.07F);
        private readonly Quaternion defaultHeadTrackerRotation = Quaternion.Euler(270, 0, 180);
        [System.NonSerialized]
        public Vector3 headTrackerPosition;
        [System.NonSerialized]
        public Quaternion headTrackerRotation;

        #region Manage

        public override bool AddTracker(HumanoidControl humanoid, string resourceName) {
            bool trackerAdded = base.AddTracker(humanoid, resourceName);
            if (trackerAdded) {
                PlaceTrackerTransform(isHeadMounted);
                //trackerTransform.transform.localPosition = defaultTrackerPosition;
                //trackerTransform.transform.localRotation = defaultTrackerRotation;
            }
            LeapMotionCamera leapCamera = trackerTransform.GetComponent<LeapMotionCamera>();
            if (leapCamera == null) {
                trackerTransform.gameObject.AddComponent<LeapMotionCamera>();
            }
            leapCameras = new LeapMotionCamera[] {
                leapCamera
            };
            return trackerAdded;
        }

        public override void ShowSkeleton(bool shown) {
            foreach (LeapMotionCamera leap in leapCameras) {
                if (leap != null)
                    leap.ShowSkeleton(shown);
            }
        }

        #endregion

        #region Start

        public override void StartTracker(HumanoidControl _humanoid) {
            humanoid = _humanoid;

            if (!enabled)
                return;

            if (isHeadMounted) {
                AddXRServiceProvider();

                if (leapProvider == null)
                    leapProvider = Leap.Unity.Hands.Provider;

                AddTracker(humanoid, "LeapMotion");
                foreach (LeapMotionCamera leapCamera in leapCameras)
                    leapCamera.StartComponent(trackerTransform);

                SetTrackerOnCamera();
            }
            else {
                AddServiceProvider();

                if (leapProvider == null)
                    leapProvider = Leap.Unity.Hands.Provider;

                AddTracker(humanoid, "LeapMotion");
                foreach (LeapMotionCamera leapCamera in leapCameras)
                    leapCamera.StartComponent(trackerTransform);
            }
        }

        #endregion

        #region Update

        private bool policySet = false;

        // This is a (strange) correction factor to get the tracking of the leap hands at the right position
        // The values seem to depend on the tracking system
        // SteamVR = (0, 0, -0.05)
        // Oculus = (0, 0.05, 0)        
        // This needs more investigation
        public Vector3 delta = new Vector3(0, 0, 0);

        public override void UpdateTracker() {
            if (!enabled || device == null)
                return;

            if (!policySet && leapTransform.status != Status.Unavailable) {
                if (UnityVRDevice.xrDevice != UnityVRDevice.XRDeviceType.None && isHeadMounted)
                    device.SetPolicy(true);
                else
                    device.SetPolicy(false);
                policySet = true;
            }

            device.position = trackerTransform.position + trackerTransform.transform.rotation * delta;
            device.rotation = trackerTransform.rotation;
            device.Update();

            status = leapTransform.status;
            if (trackerTransform != null)
                trackerTransform.gameObject.SetActive(status != Status.Unavailable);

            foreach (LeapMotionCamera leapCamera in leapCameras)
                leapCamera.UpdateComponent();
        }

        #endregion

        public override void StopTracker() {
            if (device != null)
                device.Stop();
        }

        private void SetTrackerOnCamera() {
            if (trackerTransform == null)
                return;

#if pUNITYXR
            if (humanoid.headTarget.unity.unityCamera != null)
                trackerTransform.SetParent(humanoid.headTarget.unity.unityCamera.transform, true);
            else
#elif hOPENVR || hOCULUS
            if (humanoid.headTarget.unity.cameraTransform != null)
                trackerTransform.SetParent(humanoid.headTarget.unity.cameraTransform, true);
            else
#endif
            {
                Camera camera = humanoid.GetComponentInChildren<Camera>();
                if (camera != null)
                    trackerTransform.SetParent(camera.transform, true);
                else
                    return;
            }
        }

        public void PlaceTrackerTransform(bool isHeadMounted) {
            if (trackerTransform == null)
                return;

            if (isHeadMounted) {
                headTrackerPosition = defaultHeadTrackerPosition;
                headTrackerRotation = defaultHeadTrackerRotation;
                UpdateTrackerFromTarget(isHeadMounted);
            }
            else {
                trackerTransform.localPosition = defaultTrackerPosition;
                trackerTransform.localRotation = defaultTrackerRotation;
            }
        }

        public void SetTrackerToTarget() {
            if (isHeadMounted) {
                Transform cameraTransform;
#if pUNITYXR
                if (humanoid.headTarget.unity.unityCamera != null)
                    cameraTransform = humanoid.headTarget.unity.unityCamera.transform;
                else

#elif hOPENVR || hOCULUS
                if (humanoid.headTarget.unity.cameraTransform != null)
                    cameraTransform = humanoid.headTarget.unity.cameraTransform;
                else
#endif
                {
                    Camera camera = humanoid.GetComponentInChildren<Camera>();
                    if (camera != null)
                        cameraTransform = camera.transform;
                    else
                        return;
                }

                headTrackerRotation = Quaternion.Inverse(trackerTransform.rotation) * cameraTransform.rotation;
                headTrackerPosition = UnitySensor.InverseTransformPointUnscaled(cameraTransform, trackerTransform.position);
            }
        }

        public void UpdateTrackerFromTarget(bool isHeadMounted) {
            if (enabled && isHeadMounted) {
                Transform cameraTransform;
#if pUNITYXR
                if (humanoid.headTarget.unity == null) {
#if hOCULUS
                    humanoid.headTarget.unity = humanoid.headTarget.oculus.unityXRHmd;
#endif
                }
                if (humanoid.headTarget.unity.unityCamera != null)
                    cameraTransform = humanoid.headTarget.unity.unityCamera.transform;
                else
#elif hSTEAMVR || hOCULUS
                if (humanoid.headTarget.unity.cameraTransform != null)
                    cameraTransform = humanoid.headTarget.unity.cameraTransform;
                else
#endif
                    {
                    Camera camera = humanoid.GetComponentInChildren<Camera>();
                    if (camera != null)
                        cameraTransform = camera.transform;
                    else
                        return;
                }

                trackerTransform.rotation = cameraTransform.rotation * headTrackerRotation;
                trackerTransform.position = cameraTransform.position + cameraTransform.rotation * headTrackerPosition;
            }
        }

        public override void AdjustTracking(Vector3 v, Quaternion q) {
            // Disabled because calibration should not change the set position of the camera
            //if (isHeadMounted)
            //    return;

            //if (trackerTransform != null) {
            //    trackerTransform.position += v;
            //    trackerTransform.rotation *= q;
            //}
        }

        #region Leap Package Support

        private void AddXRServiceProvider() {
#if pUNITYXR
            Transform cameraTransform = humanoid.headTarget.unity.unityCamera.transform;
#else
            Transform cameraTransform = humanoid.headTarget.unity.GetCameraTransform(humanoid.headTarget);
#endif
            // Workaround for leap motion built-in tracking.
            // This only works when using just 1 leap motion camera
            // or when the first camera is mounted on the hmd
            if (cameraTransform != null) {
                Leap.Unity.LeapXRServiceProvider serviceProvider = cameraTransform.GetComponent<Leap.Unity.LeapXRServiceProvider>();
                if (serviceProvider == null)
                    serviceProvider = cameraTransform.gameObject.AddComponent<Leap.Unity.LeapXRServiceProvider>();
                serviceProvider.deviceOffsetMode = Leap.Unity.LeapXRServiceProvider.DeviceOffsetMode.Transform;
                leapCameras[0].deviceOrigin.transform.localEulerAngles = new Vector3(-90, 0, -180);
                serviceProvider.deviceOrigin = leapCameras[0].deviceOrigin;

                UnityEngine.SpatialTracking.TrackedPoseDriver trackedPoseDriver = cameraTransform.GetComponent<UnityEngine.SpatialTracking.TrackedPoseDriver>();
                if (trackedPoseDriver != null)
                    Object.Destroy(trackedPoseDriver);
            }
        }

        private void AddServiceProvider() {
            Leap.Unity.LeapServiceProvider serviceProvider = trackerTransform.GetComponent<Leap.Unity.LeapServiceProvider>();
            if (serviceProvider == null)
                serviceProvider = trackerTransform.gameObject.AddComponent<Leap.Unity.LeapServiceProvider>();

            serviceProvider.editTimePose = Leap.TestHandFactory.TestHandPose.DesktopModeA;
        }

        //private void OnUpdateFrame(Leap.Frame frame) {
        //    if (frame == null)
        //        return;

        //    humanoid.leftHandTarget.leap.SetHand(null);
        //    humanoid.rightHandTarget.leap.SetHand(null);

        //    for (int i = 0; i < frame.Hands.Count; i++) {
        //        Leap.Hand curHand = frame.Hands[i];
        //        if (curHand.IsLeft)
        //            humanoid.leftHandTarget.leap.SetHand(curHand);
        //        if (curHand.IsRight)
        //            humanoid.rightHandTarget.leap.SetHand(curHand);
        //    }
        //}

        #endregion
    }
}
#endif