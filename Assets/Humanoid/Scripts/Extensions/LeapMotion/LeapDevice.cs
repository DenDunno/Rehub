using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    public class LeapDevice : TrackingDevice {
#if hLEAP
        public static string name = "Leap Motion";
        private static IntPtr pLeap;

        public static void LoadDlls() {
            string cDllPath = Application.dataPath + "/Plugins/LeapMotion/Core/Plugins/x86_64/LeapC.dll";
            if (File.Exists(cDllPath))
                LoadLibrary("Assets/Plugins/LeapMotion/Core/Plugins/x86_64/LeapC.dll");
            else
                LoadLibrary("Assets/LeapMotion/Core/Plugins/x86_64/LeapC.dll");
            LoadLibrary("Assets/Humanoid/Plugins/Humanoid.dll");
            LoadLibrary("Assets/Humanoid/Plugins/HumanoidLeap.dll");
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        public LeapDevice() {
            pLeap = Leap_Constructor();
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Leap_Constructor();

        ~LeapDevice() {
            Leap_Destructor(pLeap);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_Destructor(IntPtr pLeap);

        public override void Init() {
            Leap_Init(pLeap);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_Init(IntPtr pLeap);

        public override void Stop() {
            // Leap_Stop(pLeap);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_Stop(IntPtr pLeap);

        public override void Update() {
            Leap_Update(pLeap);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_Update(IntPtr pLeap);

        #region Tracker
        public uint GetTrackingDeviceCount() {
            return Leap_GetDeviceCount(pLeap);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint Leap_GetDeviceCount(IntPtr pLeap);

        public IntPtr GetTrackingDevice(uint deviceId) {
            return Leap_GetDevice(pLeap, deviceId);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Leap_GetDevice(IntPtr pLeap, uint deviceId);

        public void SetPolicy(bool isHmdMounted) {
            Leap_SetPolicy(pLeap, isHmdMounted);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_SetPolicy(IntPtr pLeap, bool isHmdMounted);

        public override Vector3 position {
            set {
                Leap_SetPosition(pLeap, new Vec3(value));
            }
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_SetPosition(IntPtr pLeap, Vec3 position);

        public override Quaternion rotation {
            set {
                Leap_SetRotation(pLeap, new Quat(value));
            }
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Leap_SetRotation(IntPtr pLeap, Quat rotation);

        public override TrackerTransformC GetTrackerData() {
            TrackerTransformC trackerTransform = Leap_GetTrackerData(pLeap);
            return trackerTransform;
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern TrackerTransformC Leap_GetTrackerData(IntPtr pLeap);

        #endregion

        #region Bones

        public enum BoneId {
            Invalid = -1,
            Hand = 0,
            ThumbProximal,
            ThumbIntermediate,
            ThumbDistal,
            ThumbTip,
            IndexProximal,
            IndexIntermediate,
            IndexDistal,
            IndexTip,
            MiddleProximal,
            MiddleIntermediate,
            MiddleDistal,
            MiddleTip,
            RingProximal,
            RingIntermediate,
            RingDistal,
            RingTip,
            LittleProximal,
            LittleIntermediate,
            LittleDistal,
            LittleTip,
            Count
        }

        public static BoneId GetBoneId(Finger finger, FingerBone fingerBone) {
            if (fingerBone == FingerBone.Metacarpal)
                return BoneId.Invalid;

            return BoneId.ThumbProximal + (int)finger * 4 + ((int)fingerBone - 1);
        }
#if hUNSAFE
        unsafe public override SensorBone GetBone(uint actorId, Side side, SideBone boneId) {
            SensorTransformC* pTargetTransform = Leap_GetSideBone(actorId, side, boneId);
            return new SensorBone(pTargetTransform);
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern SensorTransformC* Leap_GetSideBone(uint actorId, Side side, SideBone boneId);
#else
        public override SensorTransformC GetBoneData(uint actorId, Side side, SideBone boneId) {
            SensorTransformC sensorTransform = Leap_GetSideBoneData(actorId, side, boneId);
            return sensorTransform;
        }
        public static SensorTransformC GetBoneDataS(uint actorId, Side side, SideBone boneId) {
            SensorTransformC sensorTransform = Leap_GetSideBoneData(actorId, side, boneId);
            return sensorTransform;
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Leap_GetSideBoneData(uint actorId, Side side, SideBone boneId);
#endif

        public SensorTransformC GetBoneData(uint deviceId, uint actorId, Side side, SideBone boneId) {
            SensorTransformC sensorTransform = Leap_GetSideBoneDataN(deviceId, actorId, side, boneId);
            return sensorTransform;
        }
        [DllImport("HumanoidLeap", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Leap_GetSideBoneDataN(uint deviceId, uint actorId, Side side, SideBone boneId);
        #endregion
#endif
    }

}
