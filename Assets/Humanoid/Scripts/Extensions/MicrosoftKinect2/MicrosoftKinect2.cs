using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    [Serializable]
    public class Kinect2Tracker : HumanoidTracker {
#if hKINECT2
        public override string name {
            get { return Kinect2Device.name; }
        }

        /// <summary>The interface to the Kinect2 Library functions</summary>
        public Kinect2Device device;
        /// <summary>The tracker transform of the kinect</summary>
        public TrackerTransform kinect2Transform;

        /// <summary>The default tracker transform position relative to the player</summary>
        private readonly Vector3 defaultTrackerPosition = new Vector3(0, 1.2F, 1);
        /// <summary>The default tracker transform rotation relative to the player</summary>
        private readonly Quaternion defaultTrackerRotation = Quaternion.Euler(0, 180, 0);

        public int bodyID;

        public Kinect2Tracker() {
            deviceView = new DeviceView();
        }

        public override void Enable() {
            base.Enable();
            AddTracker(humanoid);
        }

        public override bool AddTracker(HumanoidControl humanoid, string resourceName) {
            bool trackerAdded = base.AddTracker(humanoid, resourceName);
            if (trackerAdded) {
                trackerTransform.transform.localPosition = defaultTrackerPosition;
                trackerTransform.transform.localRotation = defaultTrackerRotation;
            }
            return trackerAdded;
        }

        public static GameObject AddTracker(HumanoidControl humanoid) {
            GameObject realWorld = HumanoidControl.GetRealWorld(humanoid.transform);

            humanoid.kinect2.trackerTransform = FindTrackerObject(realWorld, Kinect2Device.name);
            if (humanoid.kinect2.trackerTransform == null) {
                humanoid.kinect2.trackerTransform = CreateTracker();
                humanoid.kinect2.trackerTransform.transform.parent = realWorld.transform;
            }
            return humanoid.kinect2.trackerTransform.gameObject;
        }

        public static Transform CreateTracker() {
            GameObject kinect2Model = Resources.Load("Kinect2") as GameObject;

            GameObject trackerObject = UnityEngine.Object.Instantiate(kinect2Model);
            trackerObject.name = Kinect2Device.name;
            return trackerObject.transform;
        }

        public static void RemoveTracker(HumanoidControl humanoid) {
            UnityEngine.Object.DestroyImmediate(humanoid.kinect2.trackerTransform, true);
        }

        #region Start

        public override void StartTracker(HumanoidControl _humanoid) {
            humanoid = _humanoid;

            if (!enabled)
                return;

            device = new Kinect2Device();
            device.Init();

            trackerDevice = Kinect2Device.pKinect;

            kinect2Transform = device.GetTracker();

            AddTracker(humanoid, name);
        }
        
        #endregion

        #region Stop

        public override void StopTracker() {
            if (device != null)
                device.Stop();
        }

        #endregion

        #region Update
        public override void UpdateTracker() {
            if (!enabled ||
                    device == null ||
                    trackerTransform == null)
                return;

            device.position = trackerTransform.position;
            device.rotation = trackerTransform.rotation;
            device.Update();

            status = kinect2Transform.status;
            status = Status.Present;
            trackerTransform.gameObject.SetActive(status != Status.Unavailable);
        }
        #endregion

        #region Sensor Fusion
        public void CalibrateWithHead(SensorBone headSensor) {
            Vector3 delta = humanoid.headTarget.head.target.transform.position - headSensor.position;
            trackerTransform.transform.position += (delta * 0.01F);

            // Rotation calibration is not reliable enough and is therefore not implemented
            // Rotation needs to be set manually.
            // Kinect head rotation based on body tracking is not accurate enough
            // Kinect head rotation based on face tracking does not work with HMDs
        }

        public void CalibrateWithHeadAndHands(SensorBone headSensor, SensorBone leftHandSensor, SensorBone rightHandSensor) {
            Vector3 trackingNormal = TrackingNormal(humanoid.headTarget.head.target.transform.position, humanoid.leftHandTarget.transform.position, humanoid.rightHandTarget.transform.position);

            Vector3 TrackingSensorsNormal = TrackingNormal(headSensor.position, leftHandSensor.position, rightHandSensor.position);

            Quaternion rotation = Quaternion.FromToRotation(TrackingSensorsNormal, trackingNormal);
            float rotY = Angle.Normalize(rotation.eulerAngles.y);
            float rotX = Angle.Normalize(rotation.eulerAngles.x);

            trackerTransform.RotateAround(humanoid.headTarget.head.target.transform.position, humanoid.up, rotY * 0.01F);
            trackerTransform.RotateAround(humanoid.headTarget.head.target.transform.position, humanoid.transform.right, rotX * 0.01F);

            Vector3 delta = humanoid.headTarget.head.target.transform.position - headSensor.position;
            trackerTransform.transform.position += (delta * 0.01F);
        }

        public void CalibrateWithHands(SensorBone leftHandSensor, SensorBone rightHandSensor) {
            Vector3 avgHandPosition = (leftHandSensor.position + rightHandSensor.position) / 2;

            Vector3 targetLeftHandPosition = humanoid.leftHandTarget.hand.target.transform.position;
            Vector3 targetRightHandPosition = humanoid.rightHandTarget.hand.target.transform.position;
            Vector3 targetAvgHandPosition = (targetLeftHandPosition + targetRightHandPosition) / 2;

            Vector3 delta = targetAvgHandPosition - avgHandPosition;

            trackerTransform.position += (delta * 0.01F);

            // Just positional calibration for now
        }

        private Vector3 TrackingNormal(Vector3 neckPosition, Vector3 leftHandPosition, Vector3 rightHandPosition) {
            Vector3 neck2leftHand = leftHandPosition - neckPosition;
            Vector3 neck2rightHand = rightHandPosition - neckPosition;

            Vector3 trackingNormal = Vector3.Cross(neck2leftHand, neck2rightHand);
            return trackingNormal;
        }
        #endregion

        public override void Calibrate() {
            //if (kinectTransform.status != Status.Unavailable)
            //    ;//KinectDevice.Calibrate();
        }

        #region Smoothing
        public static Vector3 SmoothPosition(Vector3 lastTargetPosition, Vector sensorPosition) {
            return SmoothPosition(lastTargetPosition, HumanoidTarget.ToVector3(sensorPosition));
        }
        public static Vector3 SmoothPosition(Vector3 lastTargetPosition, Vector3 sensorPosition) {
            // complementary filter
            return Vector3.Lerp(lastTargetPosition, sensorPosition, Time.deltaTime * 20);
        }

        public static Quaternion SmoothRotation(Quaternion lastTargetRotation, Rotation sensorRotation) {
            return SmoothRotation(lastTargetRotation, HumanoidTarget.ToQuaternion(sensorRotation));
        }
        public static Quaternion SmoothRotation(Quaternion lastTargetRotation, Quaternion sensorRotation) {
            // complementary filter
            return Quaternion.Slerp(lastTargetRotation, sensorRotation, Time.deltaTime * 20);
        }
        #endregion

        #region Device
#if hFACE
        public override Vector3 GetBonePosition(uint actorId, FacialBone boneId) {
            base.GetBonePosition(actorId, boneId);
            return Kinect_GetFaceBonePosition(trackerDevice, actorId, boneId).Vector3;
        }
        [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
        private static extern Vec3 Kinect_GetFaceBonePosition(IntPtr pKinect, uint actorId, FacialBone faceBone);
#endif
        #endregion
    }

    namespace Tracking {
        public class Kinect2Device : TrackingDevice {
            public static string name = "Microsoft Kinect 2";
            public static IntPtr pKinect;

            public Kinect2Device() {
                pKinect = Kinect_Constructor();
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr Kinect_Constructor();

            ~Kinect2Device() {
                Kinect_Destructor(pKinect);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_Destructor(IntPtr pKinect);

            public override void Init() {
                Kinect_Init(pKinect);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_Init(IntPtr pKinect);

            public override void Stop() {
                Kinect_Stop(pKinect);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_Stop(IntPtr pKinect);

        #region Tracker
            public override void Update() {
                Kinect_Update(pKinect);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_Update(IntPtr pKinect);

            //public override Status status {
            //    get {
            //        return Kinect_GetStatus(pKinect);
            //    }
            //}
            //[DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            //private static extern Status Kinect_GetStatus(IntPtr pKinect);

            public override Vector3 position {
                set {
                    Kinect_SetPosition(pKinect, new Vec3(value));
                }
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_SetPosition(IntPtr pKinect, Vec3 position);

            public override Quaternion rotation {
                set {
                    Kinect_SetRotation(pKinect, new Quat(value));
                }
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern void Kinect_SetRotation(IntPtr pKinect, Quat rotation);
        #endregion

        #region Space
            public Vector3 GetSpacePoint(int i) {
                Vector3 spacePoint = Kinect_GetSpacePoint(pKinect, i).Vector3;
                return spacePoint;
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern Vec3 Kinect_GetSpacePoint(IntPtr pKinect, int i);

            Vector3[] spacePoints = new Vector3[512 * 424];
            public Vector3[] GetSpace() {
                IntPtr pSpacePoints = Kinect_GetSpace(pKinect);
                if (pSpacePoints == IntPtr.Zero)
                    return null;

                for (int i = 0; i < 512 * 424; i++) {
                    spacePoints[i] = GetSpacePoint(pSpacePoints, i);
                }
                return spacePoints;
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr Kinect_GetSpace(IntPtr pKinect);

            private Vector3 GetSpacePoint(IntPtr ptr, int i) {
                int vector3size = Marshal.SizeOf(typeof(Vector3));
                IntPtr data = new IntPtr(ptr.ToInt64() + vector3size * i);
                Vector3 spacePoint = (Vector3)Marshal.PtrToStructure(data, typeof(Vector3));
                return spacePoint;
            }
        #endregion

        #region Bones
            //public override Vector3 GetBonePosition(uint actorId, Bone boneId) {
            //    return Kinect_GetBonePosition(pKinect, actorId, boneId).Vector3;
            //}
            //[DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            //private static extern Vec3 Kinect_GetBonePosition(IntPtr pKinect, uint actorId, Bone boneId);

            //public override Vector3 GetBonePosition(uint actorId, Side side, SideBone boneId) {
            //    return Kinect_GetSideBonePosition(pKinect, actorId, side, boneId).Vector3;
            //}
            //[DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            //private static extern Vec3 Kinect_GetSideBonePosition(IntPtr pKinect, uint actorId, Side side, SideBone boneId);

            //public override Quaternion GetBoneRotation(uint actorId, Bone boneId) {
            //    if (boneId != Bone.Head)
            //        return Quaternion.identity;

            //    Vector3 neckPos = GetBonePosition(actorId, Bone.Neck);
            //    Vector3 headPos = GetBonePosition(actorId, Bone.Head);

            //    Vector3 direction = headPos - neckPos;
            //    if (direction.sqrMagnitude == 0)
            //        return Quaternion.identity;

            //    direction = Quaternion.AngleAxis(180, Vector3.up) * direction;
            //    Quaternion neckRotation = Quaternion.LookRotation(direction, Vector3.back);
            //    neckRotation *= Quaternion.AngleAxis(90, Vector3.right);
            //    return neckRotation;
            //}

            //public override float GetBoneConfidence(uint actorId, Bone boneId) {
            //    return Kinect_GetBoneConfidence(pKinect, actorId, boneId);
            //}
            //[DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            //private static extern float Kinect_GetBoneConfidence(IntPtr pKinect, uint actorId, Bone boneId);

            //public override float GetBoneConfidence(uint actorId, Side side, SideBone boneId) {
            //    return Kinect_GetSideBoneConfidence(pKinect, actorId, side, boneId);
            //}
            //[DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            //private static extern float Kinect_GetSideBoneConfidence(IntPtr pKinect, uint actorId, Side side, SideBone boneId);

#if hUNSAFE
        unsafe public override SensorBone GetBone(uint actorId, Bone boneId) {
            SensorTransformC* pTargetTransform = Kinect_GetBone(actorId, boneId);
            return new SensorBone(pTargetTransform);
        }
        [DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern SensorTransformC* Kinect_GetBone(uint actorId, Bone boneId);
#else
            public override SensorTransformC GetBoneData(uint actorId, Bone boneId) {
                SensorTransformC sensorTransform = Kinect_GetBoneData(actorId, boneId);
                return sensorTransform;
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern SensorTransformC Kinect_GetBoneData(uint actorId, Bone boneId);
#endif

#if hUNSAFE
        unsafe public override SensorBone GetBone(uint actorId, Side side, SideBone boneId) {
            SensorTransformC* pTargetTransform = Kinect_GetSideBone(actorId, side, boneId);
            return new SensorBone(pTargetTransform);
        }
        [DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern SensorTransformC* Kinect_GetSideBone(uint actorId, Side side, SideBone boneId);
#else
            public override SensorTransformC GetBoneData(uint actorId, Side side, SideBone boneId) {
                SensorTransformC sensorTransform = Kinect_GetSideBoneData(actorId, side, boneId);
                return sensorTransform;
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern SensorTransformC Kinect_GetSideBoneData(uint actorId, Side side, SideBone boneId);
#endif
        #endregion

        #region Hands
            public enum HandPose {
                Unknown,
                Open,
                Closed,
                Lasso,
            }

            public HandPose GetHandPose(uint actorId, bool isLeft) {
                return Kinect_GetHandPose(pKinect, actorId, isLeft);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern HandPose Kinect_GetHandPose(IntPtr pKinect, uint actorId, bool isLeft);
        #endregion

        #region Face

            public enum FaceExpression {
                JawOpen = 0,
                LipPucker = 1,
                JawSlideRight = 2,
                LipStretcherRight = 3,
                LipStretcherLeft = 4,
                LipCornerPullerLeft = 5,
                LipCornerPullerRight = 6,
                LipCornerDepressorLeft = 7,
                LipCornerDepressorRight = 8,
                LeftCheekPuff = 9,
                RightCheekPuff = 10,
                EyeClosedLeft = 11,
                EyeClosedRight = 12,
                BrowLowererRight = 13,
                BrowLowererLeft = 14,
                LowerlipDepressorLeft = 15,
                LowerlipDepressorRight = 16,
            }

            public float GetFaceExpression(uint actorId, FaceExpression expresssion) {
                return Kinect_GetFaceExpression(pKinect, actorId, expresssion);
            }
            [DllImport("HumanoidKinect2", CallingConvention = CallingConvention.Cdecl)]
            private static extern float Kinect_GetFaceExpression(IntPtr pKinect, uint actorId, FaceExpression expression);

            public Vector3 GetFaceBone(uint actorId, FaceBone faceBone) {
                return Kinect_GetFaceBone(pKinect, actorId, faceBone).Vector3;
            }
            [DllImport("HumanoidKinect", CallingConvention = CallingConvention.Cdecl)]
            private static extern Vec3 Kinect_GetFaceBone(IntPtr pKinect, uint actorId, FaceBone faceBone);
            
        #endregion
        }
#endif
    }
}
