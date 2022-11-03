#if hOPENVR && hVIVETRACKER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)

using UnityEditor;
using UnityEngine;

namespace Passer {
    using Humanoid;
    using Tracking;

    public class ViveTracker_Editor : Tracker_Editor {

        #region Tracker
        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {
#if hOPENVR
            private OpenVRHumanoidTracker steamTracker;

            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, OpenVRHumanoidTracker _steamTracker)
#endif
                : base(serializedObject, targetObjs, _steamTracker, "steam") {
                steamTracker = _steamTracker;
                tracker = steamTracker;
            }

            public override void Inspector(HumanoidControl humanoid) { }

            public override void InitControllers() {
                HumanoidControl humanoid = steamTracker.humanoid;

                steamTracker.headSensorVive.InitController(headSensorProp, humanoid.headTarget);
                steamTracker.leftHandSensorVive.InitController(leftHandSensorProp, humanoid.leftHandTarget);
                steamTracker.rightHandSensorVive.InitController(rightHandSensorProp, humanoid.rightHandTarget);
                steamTracker.hipsSensorVive.InitController(hipsSensorProp, humanoid.hipsTarget);
                steamTracker.leftFootSensorVive.InitController(leftFootSensorProp, humanoid.leftFootTarget);
                steamTracker.rightFootSensorVive.InitController(rightFootSensorProp, humanoid.rightFootTarget);
            }

            public override void RemoveControllers() {
                RemoveTransform(steamTracker.headSensorVive.sensorTransform);
                RemoveTransform(steamTracker.leftHandSensorVive.sensorTransform);
                RemoveTransform(steamTracker.rightHandSensorVive.sensorTransform);
                RemoveTransform(steamTracker.hipsSensorVive.sensorTransform);
                RemoveTransform(steamTracker.leftFootSensorVive.sensorTransform);
                RemoveTransform(steamTracker.rightFootSensorVive.sensorTransform);
            }

            public override void SetSensors2Target() {
                steamTracker.headSensorVive.SetSensor2Target();
                steamTracker.leftHandSensorVive.SetSensor2Target();
                steamTracker.rightHandSensorVive.SetSensor2Target();
                steamTracker.hipsSensorVive.SetSensor2Target();
                steamTracker.leftFootSensorVive.SetSensor2Target();
                steamTracker.rightFootSensorVive.SetSensor2Target();
            }
        }
        #endregion

        #region Head
        public class HeadTargetProps : HeadTarget_Editor.TargetProps {
            public HeadTargetProps(SerializedObject serializedObject, HeadTarget headTarget)
                : base(serializedObject, headTarget.viveTracker, headTarget, "viveTracker") {

                if (headTarget.viveTracker.enabled)
                    sensor.SetSensor2Target();
            }

            public override void Inspector() {
#if hOPENVR
#if UNITY_2020_1_OR_NEWER
                sensor.tracker = headTarget.humanoid.unityXR;

                if (!headTarget.humanoid.openVR.enabled)
#else
                if (!PlayerSettings.virtualRealitySupported || !headTarget.humanoid.openVR.enabled)
#endif
                    return;
#endif                

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, headTarget);
                sensor.enabled = enabledProp.boolValue;

                //sensor.CheckSensorTransform();
                //if (!Application.isPlaying) {
                //    sensor.SetSensor2Target();
                //    sensor.ShowSensor(headTarget.humanoid.showRealObjects && headTarget.showRealObjects);
                //}

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensor.sensorTransform == null) {
                        // Tracker does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Tracker", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
#if pUNITYXR
                                headTarget.humanoid.unityXR.CheckTracker(headTarget.humanoid);
                                headTarget.viveTracker.tracker = headTarget.humanoid.unityXR;
#else
                                headTarget.humanoid.openVR.CheckTracker(headTarget.humanoid);
                                headTarget.viveTracker.tracker = headTarget.humanoid.openVR;
#endif
                                headTarget.viveTracker.CheckSensorTransform();
                                CheckSensorComponent(headTarget);
                            }
                        }
                    }
                    else
                        sensorTransformProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Tracker Transform", sensor.sensorTransform, typeof(Transform), true);
                    EditorGUI.indentLevel--;
                }
            }

            private static void CheckSensorComponent(HeadTarget headTarget) {
                if (headTarget.viveTracker.sensorTransform == null)
                    return;

                ViveTrackerComponent sensorComponent = headTarget.viveTracker.sensorTransform.GetComponent<ViveTrackerComponent>();
                if (sensorComponent == null)
                    sensorComponent = headTarget.viveTracker.sensorTransform.gameObject.AddComponent<ViveTrackerComponent>();
            }
        }
        #endregion

        #region Hand

        public class HandTargetProps : HandTarget_Editor.TargetProps {
            private static SerializedProperty attachedBoneProp;

            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.viveTracker, handTarget, "viveTracker") {

                attachedBoneProp = serializedObject.FindProperty("viveTracker.attachedBone");

                if (handTarget.viveTracker.enabled)
                    sensor.SetSensor2Target();
            }

            public override void Inspector() {
#if hOPENVR
#if UNITY_2020_1_OR_NEWER
                sensor.tracker = handTarget.humanoid.unityXR;

                if (!handTarget.humanoid.openVR.enabled && !handTarget.humanoid.unity.enabled)
#else
                if (!PlayerSettings.virtualRealitySupported || !handTarget.humanoid.openVR.enabled)
#endif
                    return;
#endif
                //CheckSensorComponent(handTarget);

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, handTarget);
                //sensor.enabled = enabledProp.boolValue;
                //sensor.CheckSensorTransform();
                //if (!Application.isPlaying) {
                //    sensor.SetSensor2Target();
                //    sensor.ShowSensor(handTarget.humanoid.showRealObjects && handTarget.showRealObjects);
                //}


                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensor.sensorTransform == null) {
                        // Tracker does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Tracker", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
#if pUNITYXR
                                handTarget.humanoid.unityXR.CheckTracker(handTarget.humanoid);
                                handTarget.viveTracker.tracker = handTarget.humanoid.unityXR;
#else
                                handTarget.humanoid.openVR.CheckTracker(handTarget.humanoid);
                                handTarget.viveTracker.tracker = handTarget.humanoid.openVR;
#endif
                                handTarget.viveTracker.CheckSensorTransform();
                                CheckSensorComponent(handTarget);
                            }
                        }
                    }
                    else {
                        sensorTransformProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Tracker Transform", sensor.sensorTransform, typeof(Transform), true);
                        attachedBoneProp.intValue = (int)(ViveTrackerArm.ArmBones)EditorGUILayout.EnumPopup("Bone", (ViveTrackerArm.ArmBones)attachedBoneProp.intValue);

#if hOPENVR
#if UNITY_2020_1_OR_NEWER
                        if ((ArmBones)attachedBoneProp.intValue == ArmBones.Hand && handTarget.unityXR.enabled)
                            EditorGUILayout.HelpBox("UnityXR Controller and Vive Tracker are both on the hand", MessageType.Warning);
#else
                        if ((ArmBones)attachedBoneProp.intValue == ArmBones.Hand && handTarget.openVR.enabled)
                            EditorGUILayout.HelpBox("OpenVR Controller and Vive Tracker are both on the hand", MessageType.Warning);
#endif
#endif
                    }
                    EditorGUI.indentLevel--;
                }
            }

            private static void CheckSensorComponent(HandTarget handTarget) {
                if (handTarget.viveTracker.sensorTransform == null)
                    return;

                ViveTrackerComponent sensorComponent = handTarget.viveTracker.sensorTransform.GetComponent<ViveTrackerComponent>();
                if (sensorComponent == null)
                    sensorComponent = handTarget.viveTracker.sensorTransform.gameObject.AddComponent<ViveTrackerComponent>();
            }
        }

        #endregion

        #region Hips
        public class HipsTargetProps : HipsTarget_Editor.TargetProps {
            public HipsTargetProps(SerializedObject serializedObject, HipsTarget hipsTarget)
                : base(serializedObject, hipsTarget.viveTracker, hipsTarget, "viveTracker") {

                if (hipsTarget.viveTracker.enabled)
                    sensor.SetSensor2Target();
            }

            public override void Inspector() {
#if hOPENVR
#if UNITY_2020_1_OR_NEWER
                sensor.tracker = hipsTarget.humanoid.unityXR;

                if (!hipsTarget.humanoid.openVR.enabled && !hipsTarget.humanoid.unity.enabled)
#else
                if (!PlayerSettings.virtualRealitySupported || !hipsTarget.humanoid.openVR.enabled)
#endif
                    return;
#endif
                //CheckSensorComponent(hipsTarget);

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, hipsTarget);
                //sensor.enabled = enabledProp.boolValue;
                //sensor.CheckSensorTransform();
                //if (!Application.isPlaying) {
                //    sensor.SetSensor2Target();
                //    sensor.ShowSensor(hipsTarget.humanoid.showRealObjects && hipsTarget.showRealObjects);
                //}


                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensor.sensorTransform == null) {
                        // Tracker does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Tracker", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
#if pUNITYXR
                                hipsTarget.humanoid.unityXR.CheckTracker(hipsTarget.humanoid);
                                hipsTarget.viveTracker.tracker = hipsTarget.humanoid.unityXR;
#else
                                hipsTarget.humanoid.openVR.CheckTracker(hipsTarget.humanoid);
                                hipsTarget.viveTracker.tracker = hipsTarget.humanoid.openVR;
#endif
                                hipsTarget.viveTracker.CheckSensorTransform();
                                CheckSensorComponent(hipsTarget);
                            }
                        }
                    }
                    else
                        sensorTransformProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Tracker Transform", sensor.sensorTransform, typeof(Transform), true);
                    EditorGUI.indentLevel--;
                }
            }

            private static void CheckSensorComponent(HipsTarget hipsTarget) {
                if (hipsTarget.viveTracker.sensorTransform == null)
                    return;

                ViveTrackerComponent sensorComponent = hipsTarget.viveTracker.sensorTransform.GetComponent<ViveTrackerComponent>();
                if (sensorComponent == null)
                    sensorComponent = hipsTarget.viveTracker.sensorTransform.gameObject.AddComponent<ViveTrackerComponent>();
            }

        }
        #endregion

        #region Foot
        public class FootTargetProps : FootTarget_Editor.TargetProps {
            public FootTargetProps(SerializedObject serializedObject, FootTarget footTarget)
                : base(serializedObject, footTarget.viveTracker, footTarget, "viveTracker") {

                if (footTarget.viveTracker.enabled)
                    sensor.SetSensor2Target();
            }

            public override void Inspector() {
#if hOPENVR
#if UNITY_2020_1_OR_NEWER
                sensor.tracker = footTarget.humanoid.unityXR;

                if (!footTarget.humanoid.openVR.enabled && !footTarget.humanoid.unity.enabled)
#else
                if (!PlayerSettings.virtualRealitySupported || !footTarget.humanoid.openVR.enabled)
#endif
                    return;
#endif
                //CheckSensorComponent(footTarget);

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, footTarget);
                //sensor.enabled = enabledProp.boolValue;
                //sensor.CheckSensorTransform();
                //if (!Application.isPlaying) {
                //    sensor.SetSensor2Target();
                //    sensor.ShowSensor(footTarget.humanoid.showRealObjects && footTarget.showRealObjects);
                //}

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensor.sensorTransform == null) {
                        // Tracker does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Tracker", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
#if pUNITYXR
                                footTarget.humanoid.unityXR.CheckTracker(footTarget.humanoid);
                                footTarget.viveTracker.tracker = footTarget.humanoid.unityXR;
#else
                                footTarget.humanoid.openVR.CheckTracker(footTarget.humanoid);
                                footTarget.viveTracker.tracker = footTarget.humanoid.openVR;
#endif
                                footTarget.viveTracker.CheckSensorTransform();
                                CheckSensorComponent(footTarget);
                            }
                        }
                    }
                    else
                        sensorTransformProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Tracker Transform", sensor.sensorTransform, typeof(Transform), true);
                    EditorGUI.indentLevel--;
                }
            }

            private static void CheckSensorComponent(FootTarget footTarget) {
                if (footTarget.viveTracker.sensorTransform == null)
                    return;

                ViveTrackerComponent sensorComponent = footTarget.viveTracker.sensorTransform.GetComponent<ViveTrackerComponent>();
                if (sensorComponent == null)
                    sensorComponent = footTarget.viveTracker.sensorTransform.gameObject.AddComponent<ViveTrackerComponent>();
            }
        }
        #endregion

        #region Object
        /*
        private static SerializedProperty objectEnabledProp;
        private static SerializedProperty objectSensorTransformProp;
        private static SerializedProperty objectSensor2TargetPositionProp;
        private static SerializedProperty objectSensor2TargetRotationProp;

        public static void InitObject(ObjectTarget objectTarget, SerializedObject serializedObject) {
            objectEnabledProp = serializedObject.FindProperty("viveTracker.enabled");
            objectSensorTransformProp = serializedObject.FindProperty("viveTracker.sensorTransform");
            objectSensor2TargetPositionProp = serializedObject.FindProperty("viveTracker.sensor2TargetPosition");
            objectSensor2TargetRotationProp = serializedObject.FindProperty("viveTracker.sensor2TargetRotation");

            objectTarget.viveTracker.Init(objectTarget);
        }

        public static void ObjectInspector(ViveTrackerSensor viveTracker) {
            objectEnabledProp.boolValue = Target_Editor.ControllerInspector(viveTracker);
            viveTracker.CheckSensorTransform();

            if (objectEnabledProp.boolValue) {
                EditorGUI.indentLevel++;
                viveTracker.trackerId = EditorGUILayout.IntField("Tracker Id", viveTracker.trackerId);
                objectSensorTransformProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Tracker Transform", viveTracker.sensorTransform, typeof(Transform), true);
                EditorGUI.indentLevel--;
            }
        }

        public static void SetSensor2Target(ViveTrackerSensor viveTracker) {
            viveTracker.SetSensor2Target();
            objectSensor2TargetRotationProp.quaternionValue = viveTracker.sensor2TargetRotation;
            objectSensor2TargetPositionProp.vector3Value = viveTracker.sensor2TargetPosition;
        }
        */
        #endregion

        #region Sensor Component

        [CustomEditor(typeof(ViveTrackerComponent))]
        public class ViveTrackerComponent_Editor : Editor {
            ViveTrackerComponent sensorComponent;

            private void OnEnable() {
                sensorComponent = (ViveTrackerComponent)target;
            }

            public override void OnInspectorGUI() {
                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup("Status", sensorComponent.status);
                EditorGUILayout.FloatField("Position Confidence", sensorComponent.positionConfidence);
                EditorGUILayout.FloatField("Rotation Confidence", sensorComponent.rotationConfidence);

                EditorGUI.EndDisabledGroup();
                sensorComponent.hardwareId = EditorGUILayout.TextField("Hardware Id", sensorComponent.hardwareId);
                sensorComponent.useForBodyTracking = EditorGUILayout.Toggle("Use For Body Tracking", sensorComponent.useForBodyTracking);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField("Tracker Id", sensorComponent.trackerId);


                //EditorGUILayout.Toggle("Pogo Pin 3", sensorComponent.pogo3);
                //EditorGUILayout.Toggle("Pogo Pin 4", sensorComponent.pogo4);
                //EditorGUILayout.Toggle("Pogo Pin 5", sensorComponent.pogo5);
                //EditorGUILayout.Toggle("Pogo Pin 6", sensorComponent.pogo6);
                EditorGUI.EndDisabledGroup();

                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion

    }
}
#endif
