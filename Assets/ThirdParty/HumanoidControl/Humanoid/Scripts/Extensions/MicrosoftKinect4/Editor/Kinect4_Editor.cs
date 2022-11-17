using UnityEngine;
using UnityEditor;

namespace Passer.Humanoid {

    public class Kinect4_Editor : Tracker_Editor {

#if hKINECT4

        #region Tracker

        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {
            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, Kinect4Tracker kinectTracker)
                : base(serializedObject, targetObjs, kinectTracker, "kinect4") {
            }

            public override void Inspector(HumanoidControl humanoid) {
                humanoid.kinect4.CheckTracker(humanoid);

                Inspector(humanoid, "Azure Kinect");
                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    humanoid.kinect4.trackerTransform = (Transform)EditorGUILayout.ObjectField("Tracker Transform", humanoid.kinect4.trackerTransform, typeof(Transform), true);
#if hOPENVR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
                    if (humanoid.openVR.enabled)
                        EditorGUILayout.HelpBox("Kinect interferes with SteamVR tracking", MessageType.Warning);
#endif
                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion

        #region Head

        public class HeadTargetProps : HeadTarget_Editor.TargetProps {
            public HeadTargetProps(SerializedObject serializedObject, HeadTarget headTarget)
                : base(serializedObject, headTarget.kinect4, headTarget, "kinect4") {
            }

            public override void Inspector() {
                if (headTarget.humanoid.kinect4.enabled) {
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, headTarget);
                    if (enabledProp.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        headTarget.kinect4.headTracking = EditorGUILayout.ToggleLeft("Head Tracking", headTarget.kinect4.headTracking, GUILayout.MinWidth(80));
                        if (headTarget.kinect4.headTracking)
                            headTarget.kinect4.rotationTrackingAxis = (Kinect4Head.RotationTrackingAxis)EditorGUILayout.EnumPopup(headTarget.kinect4.rotationTrackingAxis);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }

        #endregion

        #region Arm

        public class HandTargetProps : HandTarget_Editor.TargetProps {
            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.kinect4, handTarget, "kinect4") {
            }

            public override void Inspector() {
                if (handTarget.humanoid.kinect4.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, handTarget);
            }
        }

        #endregion

        #region Torso

        public class HipsTargetProps : HipsTarget_Editor.TargetProps {
            public HipsTargetProps(SerializedObject serializedObject, HipsTarget hipsTarget)
                : base(serializedObject, hipsTarget.kinect4, hipsTarget, "kinect4") {
            }

            public override void Inspector() {
                if (hipsTarget.humanoid.kinect4.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, hipsTarget);
            }
        }

        #endregion

        #region Foot

        public class FootTargetProps : FootTarget_Editor.TargetProps {
            public FootTargetProps(SerializedObject serializedObject, FootTarget footTarget)
                : base(serializedObject, footTarget.kinect4, footTarget, "kinect4") {
            }

            public override void Inspector() {
                if (footTarget.humanoid.kinect4.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, footTarget);
            }
        }

        #endregion

#endif
    }
}