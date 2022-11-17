using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if hLEAP
using Leap.Unity;
#endif

namespace Passer.Humanoid {
	using Passer.Tracking;
	using Passer.Humanoid.Tracking;

	public class LeapHandSkeleton : HandSkeleton {
#if hLEAP
		//public bool showSkeleton;

		private LeapProvider leapProvider;

        #region Start

        protected override void InitializeSkeleton() {

			if (leapProvider == null)
				leapProvider = Leap.Unity.Hands.Provider;
			if (leapProvider == null)
				return;

			status = Tracker.Status.Unavailable;
			leapProvider.OnUpdateFrame -= OnUpdateFrame;
			leapProvider.OnUpdateFrame += OnUpdateFrame;

			//boneWhite = new Material(Shader.Find("Standard")) {
			//	color = new Color(1, 1, 1)
			//};

			bones = new List<TrackedBone>(new TrackedBone[(int)LeapDevice.BoneId.Count]);

			for (int i = 0; i < (int)Finger.Count; i++) {
				Transform parent = this.transform; // bones[(int)LeapDevice.BoneId.Hand];
				for (int j = (int)FingerBone.Proximal; j <= (int)FingerBone.Tip; j++) {
					LeapDevice.BoneId boneId = LeapDevice.BoneId.ThumbProximal + i * 4 + (j - 1);
					bones[(int)boneId] = TrackedBone.Create(boneId.ToString(), parent);
					parent = bones[(int)boneId].transform;
				}
			}
		}

        #endregion

        #region Update

        public override void UpdateComponent() {
			//base.UpdateComponent();

			if (bones == null)
				InitializeSkeleton();
			if (bones == null)
				status = Tracker.Status.Unavailable;

			//if (showSkeleton)
				UpdateSkeletonRender();
		}

		private void OnUpdateFrame(Leap.Frame frame) {
			positionConfidence = 0;
			rotationConfidence = 0;
			status = Tracker.Status.Present;

			if (frame == null)
				return;

			for (int i = 0; i < frame.Hands.Count; i++) {
				Leap.Hand curHand = frame.Hands[i];
				if (isLeft == curHand.IsLeft) {
					UpdateSkeletonBones(curHand);
					return;
				}
			}
		}

		protected void UpdateSkeletonBones(Leap.Hand leapHand) {
			if (leapHand == null)
				return;

			this.transform.position = leapHand.WristPosition.ToVector3();
			this.transform.rotation = leapHand.Rotation.ToQuaternion() * Quaternion.AngleAxis(isLeft ? 90 : -90, Vector3.up);

			UpdateLeapFingers(leapHand);

			if (leapHand.PalmVelocity.Magnitude > 0) {
				positionConfidence = leapHand.Confidence * 0.9F;
				rotationConfidence = leapHand.Confidence * 0.9F;

				status = Tracker.Status.Tracking;
				//Debug.Log(leapHand.PalmVelocity.Magnitude);
			}
		}

		private void UpdateLeapFingers(Leap.Hand leapHand) {
			for (int i = 0; i < (int)Finger.Count; i++)
				UpdateLeapFinger(leapHand, i);
		}

		private void UpdateLeapFinger(Leap.Hand leapHand, int fingerIx) {
			LeapDevice.BoneId boneId = LeapDevice.BoneId.ThumbProximal + fingerIx * 4;

			Leap.Finger leapFinger = leapHand.Fingers[fingerIx];

			bones[(int)boneId].transform.position = leapFinger.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).NextJoint.ToVector3();
			Quaternion proximalRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Rotation.ToQuaternion();
			bones[(int)boneId].transform.rotation = proximalRotation * Quaternion.AngleAxis(isLeft ? 90 : -90, Vector3.up);
	
			bones[(int)boneId + 1].transform.position = leapFinger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).NextJoint.ToVector3();
			Quaternion intermediateRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Rotation.ToQuaternion();
			bones[(int)boneId + 1].transform.rotation = intermediateRotation * Quaternion.AngleAxis(isLeft ? 90 : -90, Vector3.up);

			bones[(int)boneId + 2].transform.position = leapFinger.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).NextJoint.ToVector3();
			Quaternion distalRotation = leapFinger.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Rotation.ToQuaternion();
			bones[(int)boneId + 2].transform.rotation = distalRotation * Quaternion.AngleAxis(isLeft ? 90 : -90, Vector3.up);

			bones[(int)boneId + 3].transform.position = leapFinger.Bone(Leap.Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
		}

		public override Transform GetBone(Finger finger, FingerBone fingerBone) {
			if (bones == null)
				return null;

			LeapDevice.BoneId boneId = LeapDevice.GetBoneId(finger, fingerBone);
			if (boneId == LeapDevice.BoneId.Invalid)
				return null;

			return bones[(int)boneId].transform;
		}

        #endregion

#endif
    }

}