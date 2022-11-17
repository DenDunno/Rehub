using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    public class AstraDevice : TrackingDevice {
#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WSA_10_0)
        public static string name = "Orbbec Astra";
        private static IntPtr pAstra;

        public static void LoadDlls() {
            string path2 = Application.dataPath + "/Frameworks/Orbbec/Plugins/x86_64/astra.dll";
            if (File.Exists(path2)) { 
                LoadLibrary("Assets/Frameworks/Orbbec/Plugins/x86_64/astra_core_api.dll");
                LoadLibrary("Assets/Frameworks/Orbbec/Plugins/x86_64/astra_core.dll");
                LoadLibrary("Assets/Frameworks/Orbbec/Plugins/x86_64/astra.dll");
            } else {
                LoadLibrary("Assets/Plugins/x86_64/astra_core_api.dll");
                LoadLibrary("Assets/Plugins/x86_64/astra_core.dll");
                LoadLibrary("Assets/Plugins/x86_64/astra.dll");
            }

            LoadLibrary("Assets/Humanoid/Plugins/Humanoid.dll");
            LoadLibrary("Assets/Humanoid/Plugins/HumanoidAstra.dll");
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        public AstraDevice() {
            pAstra = Astra_Constructor();
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Astra_Constructor();

        ~AstraDevice() {
            Astra_Destructor(pAstra);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_Destructor(IntPtr pAstra);

        public override void Init() {
            Astra_Init(pAstra);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_Init(IntPtr pAstra);

        public override void Stop() {
            Astra_Stop(pAstra);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_Stop(IntPtr pAstra);

        public override void Update() {
            Astra_Update(pAstra);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_Update(IntPtr pAstra);

        #region Tracker
        public override Vector3 position {
            set {
                Astra_SetPosition(pAstra, new Vec3(value));
            }
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_SetPosition(IntPtr pAstra, Vec3 position);

        public override Quaternion rotation {
            set {
                Astra_SetRotation(pAstra, new Quat(value));
            }
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Astra_SetRotation(IntPtr pAstra, Quat rotation);

        public override TrackerTransformC GetTrackerData() {
            return Astra_GetTrackerData(pAstra);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern TrackerTransformC Astra_GetTrackerData(IntPtr astra);
        #endregion

        #region Bones
        public override SensorTransformC GetBoneData(uint actorId, Bone boneId) {
            return Astra_GetBoneData(actorId, boneId);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Astra_GetBoneData(uint actorId, Bone boneId);

        public override SensorTransformC GetBoneData(uint actorId, Side side, SideBone boneId) {
            return Astra_GetSideBoneData(actorId, side, boneId);
        }
        [DllImport("HumanoidAstra", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Astra_GetSideBoneData(uint actorId, Side side, SideBone boneId);
        #endregion
#endif
    }
}
