using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    /// <summary>
    /// A hand from Oculus hand tracking
    /// </summary>
    public class OculusHandSkeleton : HandSkeleton {

#if hOCULUS

        private OculusDevice.HandState handState = new();

        #region Start

        protected override void InitializeSkeleton() {
            OculusDevice.Skeleton skeleton;
            if (OculusDevice.GetSkeleton(isLeft, out skeleton)) {

                //boneWhite = new Material(Shader.Find("Standard")) {
                //    color = new Color(1, 1, 1)
                //};

                bones = new List<TrackedBone>(new TrackedBone[skeleton.NumBones]);

                // pre-populate bones list before attempting to apply bone hierarchy
                for (int i = 0; i < skeleton.NumBones; ++i) {
                    OculusDevice.BoneId id = (OculusDevice.BoneId)skeleton.Bones[i].Id;
                    Vector3 pos = skeleton.Bones[i].Pose.Position.ToVector3();
                    Quaternion rot = skeleton.Bones[i].Pose.Orientation.ToQuaternion();

                    bones[i] = TrackedBone.Create(id.ToString(), null);
                    bones[i].transform.localPosition = pos;
                    bones[i].transform.localRotation = rot;
                }

                // Now apply bone hierarchy
                for (int i = 0; i < skeleton.NumBones; i++) {
                    if (((OculusDevice.BoneId)skeleton.Bones[i].ParentBoneIndex) == OculusDevice.BoneId.Invalid)
                        bones[i].transform.SetParent(this.transform, false);
                    else
                        bones[i].transform.SetParent(bones[skeleton.Bones[i].ParentBoneIndex].transform, false);
                }
            }
        }

        #endregion

        #region Update

        public override void UpdateComponent() {
            base.UpdateComponent();

            if (bones == null)
                InitializeSkeleton();
            if (bones == null) {
                status = Tracker.Status.Unavailable;
                DisableRenderer();
                return;
            }


            if (OculusDevice.GetHandState(OculusDevice.Step.Render, isLeft ? OculusDevice.Hand.HandLeft : OculusDevice.Hand.HandRight, ref handState)) {
                if ((handState.Status & OculusDevice.HandStatus.HandTracked) == 0) {
                    status = Tracker.Status.Present;
                    DisableRenderer();
                    return;
                }
                else {
                    status = Tracker.Status.Tracking;
#if pUNITYXR
                    this.transform.position = trackerTransform.TransformPoint(handState.RootPose.Position.ToVector3()); // + Vector3.up * OculusDevice.eyeHeight;
#else
                    this.transform.position = trackerTransform.TransformPoint(handState.RootPose.Position.ToVector3()) + Vector3.up * OculusDevice.eyeHeight;
#endif
                    this.transform.rotation = trackerTransform.rotation * handState.RootPose.Orientation.ToQuaternion();
                    this.positionConfidence = 0.9F;
                    this.rotationConfidence = 0.9F;
                }
                for (int i = 0; i < bones.Count; i++)
                    bones[i].transform.localRotation = handState.BoneRotations[i].ToQuaternion();
            }
            else {
                status = Tracker.Status.Present;
                DisableRenderer();
                return;
            }

            EnableRenderer();
            UpdateSkeletonRender();
        }

        public override int GetBoneId(Finger finger, FingerBone fingerBone) {
            OculusDevice.BoneId boneId = OculusDevice.GetBoneId(finger, fingerBone);
            return (int)boneId;
        }

        #endregion
#endif
    }
}