#if hKINECT4
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    [System.Serializable]
    public class Kinect4Arm : ArmSensor {
        public override string name {
            get { return Kinect4Device.name; }
        }

        private AzureKinect azureKinect;
        private TrackedBone kinectUpperArm;
        private TrackedBone kinectForearm;
        private TrackedBone kinectHand;

        public bool handTracking = true;

        public override void Start(HumanoidControl humanoid, Transform targetTransform) {
            base.Start(humanoid, targetTransform);

            tracker = handTarget.humanoid.kinect4;
            azureKinect = handTarget.humanoid.kinect4.tracker;
        }

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || !tracker.enabled ||
                    !enabled ||
                    azureKinect.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;

            UpdateArm();
            //if (handTracking)
            //    UpdateFingers(kinectArm);

            status = Tracker.Status.Tracking;
        }

        private void UpdateArm() {
            UpdateUpperArm(handTarget.upperArm.target);
            UpdateForearm(handTarget.forearm.target);
            UpdateHand(handTarget.hand.target);
        }

        private void UpdateUpperArm(HumanoidTarget.TargetTransform upperArmTarget) {
            if (kinectUpperArm == null) {
                kinectUpperArm = azureKinect.GetBone(BoneReference.HumanoidBone(handTarget.side, SideBone.UpperArm));
                if (kinectUpperArm == null)
                    return;
            }

            if (kinectUpperArm.positionConfidence > upperArmTarget.confidence.position) {
                upperArmTarget.transform.position = kinectUpperArm.transform.position;
                upperArmTarget.confidence.position = kinectUpperArm.positionConfidence;
            }

            //if (kinectUpperArm.rotationConfidence > upperArmTarget.confidence.rotation) {
            //    upperArmTarget.transform.rotation = kinectUpperArm.transform.rotation;
            //    upperArmTarget.confidence.rotation = 0.8F; // upperArmSensor.rotationConfidence;
            //}
        }

        private void UpdateForearm(HumanoidTarget.TargetTransform forearmTarget) {
            if (kinectForearm == null) {
                kinectForearm = azureKinect.GetBone(BoneReference.HumanoidBone(handTarget.side, SideBone.Forearm));
                if (kinectForearm == null)
                    return;
            }

            if (kinectForearm.positionConfidence > forearmTarget.confidence.position) {
                forearmTarget.transform.position = kinectForearm.transform.position;
                forearmTarget.confidence.position = kinectForearm.positionConfidence;
            }
            if (kinectForearm.rotationConfidence > forearmTarget.confidence.rotation) {
                forearmTarget.transform.rotation = kinectForearm.transform.rotation;
                forearmTarget.confidence.rotation = kinectForearm.rotationConfidence;
            }

            //Vector3 upperArmForward = forearmSensor.position - upperArmSensor.position;
            //Vector3 forearmForward = handSensor.position - forearmSensor.position;
            //Vector3 forearmUp = Vector3.Cross(upperArmForward, forearmForward);
            //if (!handTarget.isLeft)
            //    forearmUp = -forearmUp;

            //forearmTarget.transform.rotation = Quaternion.LookRotation(forearmForward, forearmUp);
            ////forearmTarget.transform.rotation =  forearmSensor.rotation; is not good enough
            //forearmTarget.confidence.rotation = forearmSensor.rotationConfidence;
        }

        private void UpdateHand(HumanoidTarget.TargetTransform handTargetBone) {
            if (kinectHand == null) {
                kinectHand = azureKinect.GetBone(BoneReference.HumanoidBone(handTarget.side, SideBone.Hand));
                if (kinectHand == null)
                    return;
            }

            if (kinectHand.positionConfidence > handTargetBone.confidence.position) {
                handTargetBone.transform.position = kinectHand.transform.position;
                handTargetBone.confidence.position = kinectHand.positionConfidence;
            }
            if (kinectHand.rotationConfidence > handTargetBone.confidence.rotation) {
                handTargetBone.transform.rotation = kinectHand.transform.rotation;
                handTargetBone.confidence.rotation = kinectHand.rotationConfidence;
            }
        }

        protected override void UpdateFingers(Tracking.ArmSensor armSensor) {
            for (int i = 0; i < (int)Finger.Count; i++)
                handTarget.SetFingerCurl((Finger)i, armSensor.fingers[i].curl);
        }

        protected void UpdateFingers() {
            //Kinect4Device.HandPose handPose = kinectTracker.device.GetHandPose(0, handTarget.isLeft);
            //bool handLasso = (handPose == Kinect4Device.HandPose.Lasso);
            //bool handClosed = (handPose == Kinect4Device.HandPose.Closed);

            //handTarget.SetFingerCurl(Finger.Thumb, (handLasso || handClosed) ? 1 : 0);
            //handTarget.SetFingerCurl(Finger.Index, handClosed ? 1 : 0);
            //handTarget.SetFingerCurl(Finger.Middle, handClosed ? 1 : 0);
            //handTarget.SetFingerCurl(Finger.Ring, (handLasso || handClosed) ? 1 : 0);
            //handTarget.SetFingerCurl(Finger.Little, (handLasso || handClosed) ? 1 : 0);
        }
    }
}
#endif