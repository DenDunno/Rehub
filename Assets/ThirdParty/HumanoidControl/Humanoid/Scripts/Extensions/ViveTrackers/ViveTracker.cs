#if hOPENVR && hVIVETRACKER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    public static class ViveTracker {
        private const string resourceName = "Vive Tracker";

        private static Transform AddViveTracker(TrackerComponent tracker, int trackerId = -1) {
            GameObject trackerPrefab = Resources.Load(resourceName) as GameObject;
            GameObject trackerObject = (trackerPrefab == null) ? new GameObject(resourceName) : Object.Instantiate(trackerPrefab);

            trackerObject.name = resourceName;

            ViveTrackerComponent trackerComponent = trackerObject.GetComponent<ViveTrackerComponent>();
            if (trackerComponent == null)
                trackerComponent = trackerObject.AddComponent<ViveTrackerComponent>();

            if (trackerId != -1)
                trackerComponent.trackerId = trackerId;
#if hOPENVR
            trackerObject.transform.parent = tracker.transform;

            trackerComponent.StartComponent(tracker.transform);
#endif
            return trackerObject.transform;
        }

        public static void ShowTracker(HumanoidControl humanoid, bool shown) {
            if (humanoid.headTarget.viveTracker != null && humanoid.headTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.headTarget.viveTracker.sensorTransform.gameObject, shown);
            if (humanoid.leftHandTarget.viveTracker != null && humanoid.leftHandTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.leftHandTarget.viveTracker.sensorTransform.gameObject, shown);
            if (humanoid.rightHandTarget.viveTracker != null && humanoid.rightHandTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.rightHandTarget.viveTracker.sensorTransform.gameObject, shown);
            if (humanoid.hipsTarget.viveTracker != null && humanoid.hipsTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.hipsTarget.viveTracker.sensorTransform.gameObject, shown);
            if (humanoid.leftFootTarget.viveTracker != null && humanoid.leftFootTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.leftFootTarget.viveTracker.sensorTransform.gameObject, shown);
            if (humanoid.rightFootTarget.viveTracker != null && humanoid.rightFootTarget.viveTracker.sensorTransform != null)
                HumanoidTracker.ShowTracker(humanoid.rightFootTarget.viveTracker.sensorTransform.gameObject, shown);
        }

        public static ViveTrackerComponent NewViveTracker(TrackerComponent tracker, uint trackerId) {
            Transform viveTrackerTransform = AddViveTracker(tracker, (int)trackerId);
            ViveTrackerComponent viveTracker = viveTrackerTransform.GetComponent<ViveTrackerComponent>();
            return viveTracker;
        }
    }
}
#endif