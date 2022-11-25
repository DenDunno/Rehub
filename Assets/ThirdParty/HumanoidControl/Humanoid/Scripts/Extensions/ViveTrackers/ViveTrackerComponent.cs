using System.Runtime.InteropServices;
using UnityEngine;

namespace Passer.Tracking {
    using Humanoid;

    public class ViveTrackerComponent : SensorComponent {
#if hOPENVR && hVIVETRACKER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
        public int trackerId = -1;

#if UNITY_2020_1_OR_NEWER
        public TrackerComponent tracker;
#else
        public Passer.Tracking.OpenVR tracker;
#endif

        public string hardwareId;
        public bool useForBodyTracking = true;

        public bool pogo3;
        public bool pogo4;
        public bool pogo5;
        public bool pogo6;

        //[System.NonSerialized]
        //private ulong hapticsActionHandle;

        public override void StartComponent(Transform trackerTransform) {
            base.StartComponent(trackerTransform);

#if UNITY_2020_1_OR_NEWER
            if (tracker == null)
                tracker = GetComponentInParent<TrackerComponent>();
#else
            if (tracker == null)
                tracker = GetComponentInParent<Passer.Tracking.OpenVR>();

#endif

            if (Passer.OpenVR.Input != null) {
                //string actionName = "/actions/default/out/haptic";
                //EVRInputError err = Passer.OpenVR.Input.GetActionHandle(actionName, ref hapticsActionHandle);
                //if (err != EVRInputError.None)
                //    Debug.LogError("OpenVR.Input.GetActionHandle error: " + err.ToString());

                GetInputActionHandles();
            }
        }

        public override void UpdateComponent() {

#if hOPENVR
            if (OpenVRDevice.status == Tracker.Status.Unavailable)
                status = Tracker.Status.Unavailable;

            if (OpenVRDevice.GetConfidence(trackerId) == 0) {
                status = OpenVRDevice.IsPresent(trackerId) ? Tracker.Status.Present : Tracker.Status.Unavailable;
                positionConfidence = 0;
                rotationConfidence = 0;
                gameObject.SetActive(false);
                return;
            }

            status = Tracker.Status.Tracking;
            hardwareId = OpenVRDevice.GetHardwareId((uint)trackerId);
            Vector3 localSensorPosition = HumanoidTarget.ToVector3(OpenVRDevice.GetPosition(trackerId));
            Quaternion localSensorRotation = HumanoidTarget.ToQuaternion(OpenVRDevice.GetRotation(trackerId)) * Quaternion.Euler(270, 0, 180);
            transform.position = tracker.transform.TransformPoint(localSensorPosition);
            transform.rotation = tracker.transform.rotation * localSensorRotation;

            positionConfidence = OpenVRDevice.GetConfidence(trackerId);
            rotationConfidence = OpenVRDevice.GetConfidence(trackerId);
#endif

            gameObject.SetActive(true);

            //VRControllerState_t controllerState = new VRControllerState_t();
            //var system = Passer.OpenVR.System;
            //if (system != null) {
            //    uint controllerStateSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t));
            //    bool newControllerState = system.GetControllerState((uint)trackerId, ref controllerState, controllerStateSize);
            //    UpdateInput(controllerState);
            //}
        }

        [System.NonSerialized]
        protected ulong actionHandlePogo3;
        [System.NonSerialized]
        protected ulong actionHandlePogo4;
        [System.NonSerialized]
        protected ulong actionHandlePogo5;
        [System.NonSerialized]
        protected ulong actionHandlePogo6;

        protected void GetInputActionHandles() {
            //GetInputActionHandle("pogo3", ref actionHandlePogo3);
            //GetInputActionHandle("pogo4", ref actionHandlePogo4);
            //GetInputActionHandle("pogo5", ref actionHandlePogo5);
            //GetInputActionHandle("pogo6", ref actionHandlePogo6);
        }

        protected static void GetInputActionHandle(string name, ref ulong actionHandle) {
            Passer.EVRInputError err;
            string path = "/actions/default/in/" + name;
            err = Passer.OpenVR.Input.GetActionHandle(path, ref actionHandle);
            if (err != Passer.EVRInputError.None) {
                Debug.LogError("OpenVR.Input.GetActionHandle error: " + err.ToString());
            }
        }

        public void UpdateInput(VRControllerState_t controllerState) {
            //pogo3 = GetPress(controllerState, EVRButtonId.k_EButton_Grip);
            //pogo4 = GetPress(controllerState, EVRButtonId.k_EButton_SteamVR_Trigger);
            //pogo5 = GetPress(controllerState, EVRButtonId.k_EButton_SteamVR_Touchpad);
            //pogo6 = GetPress(controllerState, EVRButtonId.k_EButton_ApplicationMenu);
            pogo3 = GetBoolean(actionHandlePogo3);
            //Debug.Log("Pogo3 " + pogo3);
            pogo4 = GetBoolean(actionHandlePogo4);
            //Debug.Log("pogo4 " + pogo4);
            pogo5 = GetBoolean(actionHandlePogo5);
            //Debug.Log("pogo5 " + pogo5);
            pogo6 = GetBoolean(actionHandlePogo6);
            //Debug.Log("Pogo6 " + pogo6);
        }

        private bool GetPress(VRControllerState_t controllerState, EVRButtonId button) {
            return (controllerState.ulButtonPressed & ButtonMaskFromId(button)) != 0;
        }

        private ulong ButtonMaskFromId(EVRButtonId id) {
            return (ulong)1 << (int)id;
        }

        [System.NonSerialized]
        protected Passer.InputDigitalActionData_t digitalActionData = new();
        [System.NonSerialized]
        protected readonly uint digitalActionDataSize = (uint)Marshal.SizeOf(typeof(Passer.InputDigitalActionData_t));

        protected bool GetBoolean(ulong actionHandle) {
            Passer.EVRInputError err;

            err = Passer.OpenVR.Input.GetDigitalActionData(actionHandle, ref digitalActionData, digitalActionDataSize, 0);
            if (err != Passer.EVRInputError.None) {
                Debug.Log(err);
                return false;
            }

            return digitalActionData.bState;
        }
#endif
    }
}