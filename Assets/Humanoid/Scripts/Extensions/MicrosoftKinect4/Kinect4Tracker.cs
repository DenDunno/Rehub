#if hKINECT4
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    [Serializable]
    public class Kinect4Tracker : HumanoidTracker {
#if hKINECT4
        public override string name {
            get { return Kinect4Device.name; }
        }

        public AzureKinect tracker;


        /// <summary>
        /// The default tracker transform position relative to the player
        /// </summary>
        //private readonly Vector3 defaultTrackerPosition = new Vector3(0, 1.2F, 1);
        /// <summary>
        /// The default tracker transform rotation relative to the player
        /// </summary>
        //private readonly Quaternion defaultTrackerRotation = Quaternion.Euler(0, 180, 0);

        public int bodyID;

        #region Manage

        public override void CheckTracker(HumanoidControl humanoid) {
            base.CheckTracker(humanoid);

            if (humanoid == null)
                return;

            if (enabled) {
                if (tracker == null) {
                    Transform realWorld = humanoid.realWorld;

                    Vector3 position = realWorld.position;
                    Quaternion rotation = realWorld.rotation;
                    tracker = AzureKinect.Get(realWorld, position, rotation);
                }
                if (tracker == null)
                    return;

                trackerTransform = tracker.transform;
            }
            else {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    if (tracker != null)
                        UnityEngine.Object.DestroyImmediate(tracker.gameObject, true);
                }
#endif
                tracker = null;
                trackerTransform = null;
            }
        }

        public Kinect4Tracker() {
            deviceView = new DeviceView();
        }

        public override void Enable() {
            base.Enable();
            AddTracker(humanoid);
        }

        public override bool AddTracker(HumanoidControl humanoid, string resourceName) {
            //bool trackerAdded = base.AddTracker(humanoid, resourceName);
            //if (trackerAdded) {
            //    trackerTransform.transform.localPosition = defaultTrackerPosition;
            //    trackerTransform.transform.localRotation = defaultTrackerRotation;
            //}
            //return trackerAdded;
            return false;
        }

        public static GameObject AddTracker(HumanoidControl humanoid) {
            GameObject realWorld = HumanoidControl.GetRealWorld(humanoid.transform);

            humanoid.kinect4.trackerTransform = FindTrackerObject(realWorld, Kinect4Device.name);
            if (humanoid.kinect4.trackerTransform == null) {
                humanoid.kinect4.trackerTransform = CreateTracker();
                humanoid.kinect4.trackerTransform.transform.parent = realWorld.transform;
            }
            return humanoid.kinect4.trackerTransform.gameObject;
        }

        public static Transform CreateTracker() {
            GameObject kinect2Model = Resources.Load("Kinect2") as GameObject;

            GameObject trackerObject = UnityEngine.Object.Instantiate(kinect2Model);
            trackerObject.name = Kinect4Device.name;
            return trackerObject.transform;
        }

        public static void RemoveTracker(HumanoidControl humanoid) {
            UnityEngine.Object.DestroyImmediate(humanoid.kinect4.trackerTransform, true);
        }

        public override void ShowSkeleton(bool shown) {
            if (tracker != null)
                tracker.ShowSkeleton(shown);
        }

        #endregion Manage

        #region Start

        public override void StartTracker(HumanoidControl _humanoid) {
            humanoid = _humanoid;

            if (!enabled)
                return;

            AddTracker(humanoid, name);
        }

        #endregion

        #region Stop

        public override void StopTracker() {
        }

        #endregion

        #region Update

        public override void UpdateTracker() {
            if (!enabled)
                return;

            status = tracker.status;
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

        public override void AdjustTracking(Vector3 v, Quaternion q) {
            if (trackerTransform != null) {
                trackerTransform.position += v;
                trackerTransform.rotation *= q;
            }
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

#endif
    }

}
#endif