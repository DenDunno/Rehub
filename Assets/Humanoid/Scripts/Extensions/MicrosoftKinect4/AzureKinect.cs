#if hKINECT4
using System;
using System.Runtime.InteropServices;
#endif
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    public class AzureKinect : BodySkeleton {
#if hKINECT4
        public Kinect4Device device;

        #region Manage

        public static AzureKinect Find(Transform parentTransform) {
            AzureKinect kinect = parentTransform.GetComponentInChildren<AzureKinect>();
            if (kinect != null)
                return kinect;

            kinect = FindObjectOfType<AzureKinect>();
            return kinect;
        }

        public static AzureKinect Get(Transform parentTransform, Vector3 position, Quaternion rotation) {
            AzureKinect kinect = Find(parentTransform);
            if (kinect != null)
                return kinect;

            if (Application.isPlaying) {
                Debug.LogError("Azure Kinect is missing");
                return null;
            }
#if UNITY_EDITOR
            GameObject trackerObj = new GameObject("Azure Kinect");
            Transform trackerTransform = trackerObj.transform;

            trackerTransform.parent = parentTransform;
            trackerTransform.position = position;
            trackerTransform.rotation = rotation;

            kinect = trackerObj.AddComponent<AzureKinect>();
            kinect.realWorld = parentTransform;
#endif
            return kinect;
        }

        public override void ShowSkeleton(bool shown) {
            this.show = shown;
        }

        #endregion Manage

        #region Start

        protected Dictionary<Bone, Bone> skeletonStructure = new Dictionary<Bone, Bone>()
        {
            { Bone.None, Bone.None },
            { Bone.Hips, Bone.None },
            { Bone.Spine, Bone.Hips },
            { Bone.Chest, Bone.Spine },
            { Bone.Neck, Bone.Chest },
            { Bone.Head, Bone.Neck },

            //{ Bone.LeftShoulder, Bone.Chest },
            { Bone.LeftUpperArm, Bone.Chest },
            { Bone.LeftForearm, Bone.LeftUpperArm },
            { Bone.LeftHand, Bone.LeftForearm },

            //{ Bone.RightShoulder, Bone.Chest },
            { Bone.RightUpperArm, Bone.Chest },
            { Bone.RightForearm, Bone.RightUpperArm },
            { Bone.RightHand, Bone.RightForearm },

            { Bone.LeftUpperLeg, Bone.Hips },
            { Bone.LeftLowerLeg, Bone.LeftUpperLeg },
            { Bone.LeftFoot, Bone.LeftLowerLeg },

            { Bone.RightUpperLeg, Bone.Hips },
            { Bone.RightLowerLeg, Bone.RightUpperLeg },
            { Bone.RightFoot, Bone.RightLowerLeg },
        };
        protected Bone[] kinectBones =
        {
            Bone.Hips,
            Bone.Spine,
            Bone.Chest,
            Bone.Neck,
            Bone.Head,

            //Bone.LeftShoulder,
            Bone.LeftUpperArm,
            Bone.LeftForearm,
            Bone.LeftHand,

            Bone.LeftUpperLeg,
            Bone.LeftLowerLeg,
            Bone.LeftFoot,

            //Bone.RightShoulder,
            Bone.RightUpperArm,
            Bone.RightForearm,
            Bone.RightHand,

            Bone.RightUpperLeg,
            Bone.RightLowerLeg,
            Bone.RightFoot,
        };

        protected int BoneIndex(Bone bone) {
            for (int i = 0; i < kinectBones.Length; i++) {
                if (kinectBones[i] == bone)
                    return i;
            }
            return -1;
        }

        protected override void InitializeSkeleton() {
            status = Tracker.Status.Unavailable;

            bones = new List<TrackedBone>(new TrackedBone[kinectBones.Length]);

            Transform parent;
            for (int i = 0; i < kinectBones.Length; i++) {
                Bone bone = kinectBones[i];
                Bone parentBone = skeletonStructure[bone];
                //Debug.Log(bone + " " + parentBone + " " + BoneIndex(parentBone));
                if (parentBone == Bone.None)
                    parent = this.transform;
                else
                    parent = bones[BoneIndex(parentBone)].transform;

                bones[i] = TrackedBone.Create(bone.ToString(), parent);
            }
        }


        protected override void Start() {

            //Kinect4Device.LoadDlls();

            device = new Kinect4Device();
            device.Init();

            //headSensor = device.GetBone(0, Bone.Head);
        }

        #endregion

        public virtual void OnDestroy() {
            device.Stop();
        }

        #region Update

        protected override void Update() {
            if (bones == null)
                InitializeSkeleton();
            if (bones == null)
                status = Tracker.Status.Unavailable;

            device.position = this.transform.position;
            device.rotation = this.transform.rotation;
            status = device.status;

            UpdateSkeletonBones();

            UpdateSkeletonRender();
        }

        protected void UpdateSkeletonBones() {
            for (int i = 0; i < kinectBones.Length; i++) {
                TrackingDevice.SensorTransformC sensorTransform;

                // GetBoneData does not yet support Sidebones...
                if (kinectBones[i] >= Bone.LeftShoulder && kinectBones[i] <= Bone.LeftFoot) {
                    SideBone sideBone = BoneReference.HumanoidSideBone(kinectBones[i]);
                    sensorTransform = device.GetBoneData(0, Side.Left, sideBone);
                }
                else if (kinectBones[i] >= Bone.RightShoulder && kinectBones[i] <= Bone.RightFoot) {
                    SideBone sideBone = BoneReference.HumanoidSideBone(kinectBones[i]);
                    sensorTransform = device.GetBoneData(0, Side.Right, sideBone);
                }
                else
                    sensorTransform = device.GetBoneData(0, kinectBones[i]);

                bones[i].positionConfidence = sensorTransform.positionConfidence;
                if (sensorTransform.positionConfidence > 0)
                    bones[i].transform.position = sensorTransform.position.Vector3;
                bones[i].rotationConfidence = sensorTransform.rotationConfidence;
                if (sensorTransform.rotationConfidence > 0) {
                    if (kinectBones[i] >= Bone.LeftShoulder && kinectBones[i] <= Bone.LeftHand) {
                        Quaternion rot = sensorTransform.rotation.Quaternion;
                        Vector3 angles = rot.eulerAngles;
                        bones[i].transform.rotation = Quaternion.Euler(-angles.x, angles.y, -angles.z) * Quaternion.Euler(90, 90, -90);
                    }
                    else if (kinectBones[i] >= Bone.RightShoulder && kinectBones[i] <= Bone.RightHand) {
                        Quaternion rot = sensorTransform.rotation.Quaternion;
                        Vector3 angles = rot.eulerAngles;
                        bones[i].transform.rotation = Quaternion.Euler(-angles.x, angles.y, -angles.z) * Quaternion.Euler(-90, 0, 180); 
                    }

                    else {
                        Quaternion rot = sensorTransform.rotation.Quaternion * Quaternion.Euler(-90, 90, 0);
                        Vector3 angles = rot.eulerAngles;
                        bones[i].transform.rotation = Quaternion.Euler(-angles.x, angles.y, -angles.z);
                    }
                }
            }
        }

        #endregion Update

        #region API

        public Transform GetBoneTransform(Bone boneId) {
            int boneIx = BoneIndex(boneId);
            if (boneIx == -1)
                return null;

            return bones[boneIx].transform;
        }

        public TrackedBone GetBone(Bone boneId) {
            int boneIx = BoneIndex(boneId);
            if (boneIx == -1)
                return null;

            return bones[boneIx];
        }

        #endregion API
#endif
    }

    #region Device

#if hKINECT4
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class Kinect4Device : TrackingDevice {
        public static string name = "Azure Kinect";
        public static IntPtr pKinect;

        private static string bodyTrackingSDKPath = "C:/Program Files/Azure Kinect Body Tracking SDK/tools";

#if UNITY_EDITOR
        static Kinect4Device() {
            CheckExternalDlls();
            LoadDlls();
        }

        private static void CheckExternalDlls() {
            string sdkPath = bodyTrackingSDKPath;
            if (!string.IsNullOrEmpty(sdkPath) && sdkPath[sdkPath.Length - 1] != '/' && sdkPath[sdkPath.Length - 1] != '\\')
                sdkPath += "/";

            if (!string.IsNullOrEmpty(sdkPath) && !sdkPath.EndsWith("/tools/") && !sdkPath.EndsWith("\\tools\\") && !sdkPath.EndsWith("\\tools/"))
                sdkPath += "tools/";

            if (!Directory.Exists(sdkPath)) {
                Debug.LogWarning("Body Tracking SDK not found at " + sdkPath);
            }

            string projectPath = Application.dataPath + "/../";

            CheckExternalDll(sdkPath, projectPath, "vcomp140.dll");
            CheckExternalDll(sdkPath, projectPath, "onnxruntime.dll");
            CheckExternalDll(sdkPath, projectPath, "cublas64_100.dll");
            CheckExternalDll(sdkPath, projectPath, "cudart64_100.dll");
            CheckExternalDll(sdkPath, projectPath, "cudnn64_7.dll");
            CheckExternalDll(sdkPath, projectPath, "dnn_model_2_0.onnx");


            //CheckExternalDll(projectPath, "vcomp140.dll");
            //CheckExternalDll(projectPath, "onnxruntime.dll");
            //CheckExternalDll(projectPath, "cublas64_100.dll");
            //CheckExternalDll(projectPath, "cudart64_100.dll");
            //CheckExternalDll(projectPath, "cudnn64_7.dll");
            //CheckExternalDll(projectPath, "dnn_model_2_0.onnx");
        }

        private static void CheckExternalDll(string projectPath, string dllName) {
            string filePath = projectPath + dllName;
            if (!File.Exists(filePath)) {
                File.Copy(Application.dataPath + "/Humanoid/Plugins/Kinect/" + dllName, filePath);
            }

        }

        private static void CheckExternalDll(string sdkPath, string projectPath, string dllName) {
            string filePath = projectPath + dllName;
            //Debug.Log(filePath + " check");
            if (!File.Exists(filePath)) {
                Debug.Log("Copying " + sdkPath + dllName);
                File.Copy(sdkPath + dllName, filePath);
            }

        }

#endif

        public static void LoadDlls() {
            //Debug.Log("Load Kinect4 dlls");
            LoadLibrary("Assets/Humanoid/Plugins/Kinect/k4a.dll");
            LoadLibrary("Assets/Humanoid/Plugins/Kinect/k4abt.dll");
            LoadLibrary("Assets/Humanoid/Plugins/Kinect/depthengine_2_0.dll");
            LoadLibrary("Assets/Humanoid/Plugins/Humanoid.dll");
            LoadLibrary("Assets/Humanoid/Plugins/HumanoidKinect4.dll");
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        public Kinect4Device() {
            LoadDlls();
            //Debug.Log("Kinect4_Constructor");
            pKinect = Kinect4_Constructor();
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Kinect4_Constructor();

        ~Kinect4Device() {
            //Debug.Log("Kinect4_Destructor");
            Kinect4_Destructor(pKinect);
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Kinect4_Destructor(IntPtr pKinect);

        protected new void LogError(int errorIndex, string[] errorMsgs) {
            if (errorIndex >= errorMsgs.Length - 1)
                Debug.LogError(errorMsgs[errorMsgs.Length - 1] + errorIndex);
            else
                Debug.LogError(errorMsgs[errorIndex]);
        }


        public override void Init() {
            int status = Kinect4_Init(pKinect);
            if (status < 0)
                LogError(-status, initErrorMsgs);
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Kinect4_Init(IntPtr pKinect);
        private static readonly string[] initErrorMsgs = {
            "Kinect4_Init: Device not available",
            "Kinect4_Init: Failed to open device",
            "Kinect4_Init: Failed to start camera",
            "Kinect4_Init: Failed to get calibration",
            "Kinect4_Init: Failed to create body tracker",
            "Kinect4_Init: Unknown error ",
        };

        public override void Stop() {
            //Debug.Log("Kinect4_Stop");
            Kinect4_Stop(pKinect);
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Kinect4_Stop(IntPtr pKinect);

    #region Tracker

        public override void Update() {
            Kinect4_Update(pKinect);
            //Status status = (Status)Kinect4_Update(pKinect);
            //Debug.Log("Kinect4_Update: " + status);
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Kinect4_Update(IntPtr pKinect);

        public override Tracker.Status status {
            get {
                return (Tracker.Status)Kinect4_Update(pKinect);
            }
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tracker.Status Kinect_GetStatus(IntPtr pKinect);

        public override Vector3 position {
            set {
                Kinect4_SetPosition(pKinect, new Vec3(value));
            }
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Kinect4_SetPosition(IntPtr pKinect, Vec3 position);

        public override Quaternion rotation {
            set {
                Kinect4_SetRotation(pKinect, new Quat(value));
            }
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Kinect4_SetRotation(IntPtr pKinect, Quat rotation);

    #endregion

    #region Bones

        public override SensorTransformC GetBoneData(uint actorId, Bone boneId) {
            SensorTransformC sensorTransform = Kinect4_GetBoneData(actorId, boneId);
            return sensorTransform;
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Kinect4_GetBoneData(uint actorId, Bone boneId);

        public override SensorTransformC GetBoneData(uint actorId, Side side, SideBone boneId) {
            SensorTransformC sensorTransform = Kinect4_GetSideBoneData(actorId, side, boneId);
            return sensorTransform;
        }
        [DllImport("HumanoidKinect4", CallingConvention = CallingConvention.Cdecl)]
        private static extern SensorTransformC Kinect4_GetSideBoneData(uint actorId, Side side, SideBone boneId);

    #endregion
    }
#endif

    #endregion Device
}