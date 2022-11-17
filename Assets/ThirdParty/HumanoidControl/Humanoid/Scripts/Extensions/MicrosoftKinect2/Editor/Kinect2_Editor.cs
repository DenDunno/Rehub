using UnityEngine;
using UnityEditor;

namespace Passer.Humanoid {    

    public class Kinect2_Editor : Tracker_Editor {
   
#if hKINECT2

        #region Tracker

        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {
            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, Kinect2Tracker kinectTracker)
                : base(serializedObject, targetObjs, kinectTracker, "kinect2") {
            }

            public override void Inspector(HumanoidControl humanoid) {
                Inspector(humanoid, "Kinect2");
                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    humanoid.kinect2.trackerTransform = (Transform)EditorGUILayout.ObjectField("Tracker Transform", humanoid.kinect2.trackerTransform, typeof(Transform), true);
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
                : base(serializedObject, headTarget.kinect2, headTarget, "kinect2") {
            }

            public override void Inspector() {
                if (headTarget.humanoid.kinect2.enabled) {
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, headTarget);
                    if (enabledProp.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        headTarget.kinect2.headTracking = EditorGUILayout.ToggleLeft("Head Tracking", headTarget.kinect2.headTracking, GUILayout.MinWidth(80));
                        if (headTarget.kinect2.headTracking)
                            headTarget.kinect2.rotationTrackingAxis = (Kinect2Head.RotationTrackingAxis)EditorGUILayout.EnumPopup(headTarget.kinect2.rotationTrackingAxis);
                        EditorGUILayout.EndHorizontal();
#if hFACE
                        headTarget.kinectFace.faceTracking = EditorGUILayout.ToggleLeft("Face Tracking", headTarget.kinectFace.faceTracking);
                        headTarget.kinectFace.audioInput = EditorGUILayout.ToggleLeft("Audio Input", headTarget.kinectFace.audioInput);
#endif
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }
        #endregion

        #region Arm
        public class HandTargetProps : HandTarget_Editor.TargetProps {
            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.kinect2, handTarget, "kinect2") {
            }

            public override void Inspector() {
                if (handTarget.humanoid.kinect2.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, handTarget);
            }
        }
        #endregion

        #region Torso
        public class HipsTargetProps : HipsTarget_Editor.TargetProps {
            public HipsTargetProps(SerializedObject serializedObject, HipsTarget hipsTarget)
                : base(serializedObject, hipsTarget.kinect2, hipsTarget, "kinect2") {
            }

            public override void Inspector() {
                if (hipsTarget.humanoid.kinect2.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, hipsTarget);
            }
        }
        #endregion

        #region Foot

        public class FootTargetProps : FootTarget_Editor.TargetProps {
            public FootTargetProps(SerializedObject serializedObject, FootTarget footTarget)
                : base(serializedObject, footTarget.kinect2, footTarget, "kinect2") {
            }

            public override void Inspector() {
                if (footTarget.humanoid.kinect2.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, footTarget);
            }
        }

        #endregion

#endif
    }
}
