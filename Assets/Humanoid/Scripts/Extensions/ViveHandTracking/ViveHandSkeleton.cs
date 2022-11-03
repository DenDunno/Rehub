using System.Collections.Generic;
using UnityEngine;
#if hVIVEHAND
using ViveHandTracking;
#endif

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    /// <summary>
    /// A HTC Vive hand tracking skeleton
    /// </summary>
    public class ViveHandSkeleton : HandSkeleton {
#if hOPENVR && hVIVEHAND && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)

        public SensorComponent hmd;

        #region Manage

        public static void CheckGestureProvider(Passer.Humanoid.OpenVRHumanoidTracker tracker) {
#if UNITY_2020_1_OR_NEWER
            // Make sure th UnityXR HMD is present
            tracker.humanoid.headTarget.unityXR.CheckSensor(tracker.humanoid.headTarget);

            GestureProvider provider = Camera.main.transform.GetComponent<GestureProvider>();

            if (tracker.handTracking) {
                if (provider != null)
                    return;

                Camera.main.gameObject.AddComponent<GestureProvider>();
            }
#else
            // Make sure th OpenVR HMD is present
            tracker.humanoid.headTarget.openVR.CheckSensor(tracker.humanoid.headTarget);

            if (tracker.humanoid.headTarget.openVR.sensorTransform == null) {
                // Cannot use hand tracking without a openVR hmd
                Debug.LogWarning("Vive Hand Tracking requires an tracked SteamVR HMD");
                return;
            }

            GestureProvider provider = tracker.humanoid.headTarget.openVR.sensorTransform.GetComponent<GestureProvider>();

            if (tracker.handTracking) {
                if (provider != null)
                    return;

                tracker.humanoid.headTarget.openVR.sensorTransform.gameObject.AddComponent<GestureProvider>();
            }
#endif
            else {
#if UNITY_EDITOR
                if (provider != null && !Application.isPlaying)
                    DestroyImmediate(provider, true);
#endif
            }
        }

        public static ViveHandSkeleton Find(Transform openVRTransform, bool isLeft) {
            ViveHandSkeleton[] handSkeletons = openVRTransform.GetComponentsInChildren<ViveHandSkeleton>();
            foreach (ViveHandSkeleton handSkeleton in handSkeletons) {
                if (handSkeleton.isLeft == isLeft)
                    return handSkeleton;
            }
            return null;
        }

        public static ViveHandSkeleton Get(Transform openVRTransform, bool isLeft, bool showRealObjects) {
            ViveHandSkeleton skeleton = Find(openVRTransform, isLeft);
            if (skeleton == null) {
                GameObject skeletonObj = new GameObject(isLeft ? "Left Hand Skeleton" : "Right Hand Skeleton");
                skeletonObj.transform.parent = openVRTransform;
                skeletonObj.transform.localPosition = Vector3.zero;
                skeletonObj.transform.localRotation = Quaternion.identity;

                skeleton = skeletonObj.AddComponent<ViveHandSkeleton>();
                skeleton.isLeft = isLeft;
                skeleton.show = showRealObjects;
            }
            return skeleton;
        }

        #endregion Manage

        #region Start

        protected override void InitializeSkeleton() {
            //boneWhite = new Material(Shader.Find("Standard")) {
            //    color = new Color(1, 1, 1)
            //};

            bones = new List<TrackedBone>(new TrackedBone[21]);
            bones[0] = TrackedBone.Create("Hand", null);

            bones[0].transform = this.transform;
            //AddBoneRenderer(this.gameObject);

            Transform parent = bones[0].transform;
            for (int j = (int)FingerBone.Proximal; j <= (int)FingerBone.Tip; j++) {
                int boneId = 1 + j - 1;
                bones[boneId] = TrackedBone.Create(boneId.ToString(), parent);
                parent = bones[boneId].transform;
            }

            for (int i = 1; i < (int)Finger.Count; i++) {
                parent = bones[0].transform;
                for (int j = (int)FingerBone.Proximal; j <= (int)FingerBone.Tip; j++) {
                    int boneId = i * 4 + j;
                    bones[boneId] = TrackedBone.Create(boneId.ToString(), parent);
                    parent = bones[boneId].transform;
                }
            }
        }

        #endregion

        #region Update

        public override void UpdateComponent() {
            base.UpdateComponent();

            //if (!GestureProvider.UpdatedInThisFrame)
            //    return;

            if (GestureProvider.Status == GestureStatus.NotStarted ||
                GestureProvider.HaveSkeleton == false)
                return;

            if (bones == null)
                InitializeSkeleton();
            if (bones == null)
                status = Tracker.Status.Unavailable;

            //GestureInterface.SetCameraTransform(hmd.transform.position, hmd.transform.rotation);
            GestureResult gestureResult = isLeft ? GestureProvider.LeftHand : GestureProvider.RightHand;
            if (gestureResult == null) {
                status = Tracker.Status.Present;
                DisableRenderer();
                return;
            }

            status = Tracker.Status.Tracking;
            EnableRenderer();
            UpdateSkeleton(gestureResult);
            UpdateSkeletonRender();
        }

        protected void UpdateSkeleton(GestureResult gestureResult) {
            bones[0].transform.rotation = gestureResult.rotation * (isLeft ? Quaternion.Euler(-90, 0, -90) : Quaternion.Euler(-90, 0, 90));
            for (int i = 0; i < gestureResult.points.Length; i++) {
                if (bones[i] != null) {
                    // Gesture result has the X and Z orientation of the HMD, but not the HMD height :-/

                    //if (hmd != null)
                    //     bones[i].transform.position = hmd.transform.TransformPoint(gestureResult.points[i]); // - trackerTransform.position;
                    ////bones[i].transform.position = gestureResult.points[i];// hmd.transform.TransformPoint(gestureResult.points[i]); // - trackerTransform.position;
                    //else
                    bones[i].transform.position = gestureResult.points[i];
                }
            }
        }

        //public Transform GetBone(Finger finger, FingerBone fingerBone) {
        //    if (bones == null)
        //        return null;

        //    int boneId = GetBoneId(finger, fingerBone);
        //    if (boneId == -1)
        //        return null;

        //    return bones[boneId];
        //}

        public override int GetBoneId(Finger finger, FingerBone fingerBone) {
            if (finger == Finger.Thumb)
                return (int)fingerBone;
            else
                return (int)finger * 4 + (int)fingerBone;
        }

        #endregion

#endif
    }
}