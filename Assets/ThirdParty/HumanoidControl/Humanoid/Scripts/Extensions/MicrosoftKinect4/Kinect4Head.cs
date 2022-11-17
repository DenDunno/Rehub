#if hKINECT4
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    [System.Serializable]
    public class Kinect4Head : HeadSensor {
        public override string name {
            get { return Kinect4Device.name; }
        }

        private AzureKinect azureKinect;
        private TrackedBone kinectHead;
        private TrackedBone kinectNeck;

        public bool headTracking = true;
        public enum RotationTrackingAxis {
            XYZ,
            XY
        }

        public RotationTrackingAxis rotationTrackingAxis = RotationTrackingAxis.XY;

        #region Start

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            tracker = headTarget.humanoid.kinect4;
            azureKinect = headTarget.humanoid.kinect4.tracker;
        }

        #endregion

        #region Update

        protected bool calibrated = false;

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || !tracker.enabled ||
                    !enabled ||
                    azureKinect.status == Tracker.Status.Unavailable)
                return;

            status = Tracker.Status.Present;
            UpdateBones();

            if (!calibrated && tracker.humanoid.calibrateAtStart) {
                tracker.humanoid.Calibrate();
                calibrated = true;
            }
        }

        #region Bones
        protected void UpdateBones() {
            //if (headTarget.head.target.confidence.position > headSensor.positionConfidence) {
            //    if (headTarget.humanoid.leftHandTarget.hand.target.confidence.position > kinectTracker.device.GetBoneConfidence(0, Side.Left, SideBone.Hand) &&
            //        headTarget.humanoid.rightHandTarget.hand.target.confidence.position > kinectTracker.device.GetBoneConfidence(0, Side.Right, SideBone.Hand))
            //        kinectTracker.CalibrateWithHeadAndHands(headSensor, headTarget.humanoid.leftHandTarget.kinect2.handSensor, headTarget.humanoid.rightHandTarget.kinect2.handSensor);
            //    else
            //        kinectTracker.CalibrateWithHead(headSensor);
            //    return;
            //}

            UpdateNeck(headTarget.neck.target);
            UpdateHead(headTarget.head.target);
        }

        private void UpdateNeck(HumanoidTarget.TargetTransform neckTarget) {
            if (kinectNeck == null) {
                kinectNeck = azureKinect.GetBone(Bone.Neck);
                if (kinectNeck == null)
                    return;
            }

            if (kinectNeck.positionConfidence > neckTarget.confidence.position) {
                neckTarget.transform.position = kinectNeck.transform.position;
                neckTarget.confidence.position = kinectNeck.positionConfidence;
            }

            if (kinectNeck.rotationConfidence > neckTarget.confidence.rotation) {
                neckTarget.transform.rotation = kinectNeck.transform.rotation;
                neckTarget.confidence.rotation = kinectNeck.rotationConfidence;
            }
        }

        private void UpdateHead(HumanoidTarget.TargetTransform headTarget) {
            if (kinectHead == null) {
                kinectHead = azureKinect.GetBone(Bone.Head);
                if (kinectHead == null)
                    return;
            }

            if (kinectHead.positionConfidence > headTarget.confidence.position) {
                headTarget.transform.position = kinectHead.transform.position;
                headTarget.confidence.position = kinectHead.positionConfidence;
                status = Tracker.Status.Tracking;
            }

            if (kinectHead.rotationConfidence > headTarget.confidence.rotation) {
                headTarget.transform.rotation = kinectHead.transform.rotation;
                headTarget.confidence.rotation = kinectHead.rotationConfidence;
            }
            if (rotationTrackingAxis == RotationTrackingAxis.XY)
                headTarget.transform.rotation = Quaternion.LookRotation(headTarget.transform.rotation * Vector3.forward);
        }

        #endregion

        #endregion

    }
}
#endif