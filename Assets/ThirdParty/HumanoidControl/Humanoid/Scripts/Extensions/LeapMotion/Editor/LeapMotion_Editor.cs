using System.IO;
using UnityEngine;
using UnityEditor;

namespace Passer.Humanoid {

    public class LeapMotion_Editor : Tracker_Editor {

#if hLEAP

        #region Tracker
        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {

            private SerializedProperty headMountedProp;

            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, LeapTracker leapTracker)
                : base(serializedObject, targetObjs, leapTracker, "leapTracker") {

                headMountedProp = serializedObject.FindProperty("leapTracker.isHeadMounted");
            }

            public override void Inspector(HumanoidControl humanoid) {
                humanoid.leftHandTarget.leap.CheckSensor(humanoid.leftHandTarget, humanoid.leapTracker);
                humanoid.rightHandTarget.leap.CheckSensor(humanoid.rightHandTarget, humanoid.leapTracker);

                Inspector(humanoid, "LeapMotion");

                bool wasHeadMounted = humanoid.leapTracker.isHeadMounted;
                if (enabledProp.boolValue) {

                    EditorGUI.indentLevel++;
                    {
                        GUIContent label = new GUIContent(
                            "Tracker Transform",
                            "The leap camera position in the real world"
                            );
                        trackerTransfromProp.objectReferenceValue = (Transform)EditorGUILayout.ObjectField(label, humanoid.leapTracker.trackerTransform, typeof(Transform), true);
                    }

#if pUNITYXR
                    if (true) { 
#else
                    if (PlayerSettings.virtualRealitySupported && humanoid.headTarget.unity.enabled) {
#endif
                        GUIContent label = new GUIContent(
                            "HMD mounted",
                            "Puts the leap camera on the Headset"
                            );
                        headMountedProp.boolValue = EditorGUILayout.Toggle(label, humanoid.leapTracker.isHeadMounted, GUILayout.MinWidth(80));
                        if (!Application.isPlaying)
                            humanoid.leapTracker.SetTrackerToTarget();
                    }
                    else
                        headMountedProp.boolValue = false;
                    EditorGUI.indentLevel--;
                }
                else {
                    headMountedProp.boolValue = false;
                }
                if (wasHeadMounted != headMountedProp.boolValue)
                    humanoid.leapTracker.PlaceTrackerTransform(headMountedProp.boolValue);
            }
        }

        private bool hmdMounted(HumanoidControl humanoid) {
            return (
#if pUNITYXR
                humanoid.unity.enabled &&
#else
                PlayerSettings.virtualRealitySupported &&
                humanoid.headTarget.unity.enabled &&
#endif
                humanoid.leapTracker.isHeadMounted
                );
        }
        #endregion

        #region Hand

        public class HandTargetProps : HandTarget_Editor.TargetProps {

            SerializedProperty skeletonProp;

            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.leap, handTarget, "leap") {

                skeletonProp = serializedObject.FindProperty("leap.handSkeleton");
            }

            public override void Inspector() {
                if (!handTarget.humanoid.leapTracker.enabled)
                    return;

                if (handTarget.humanoid.leapTracker.enabled)
                    enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(sensor, handTarget);

                //CheckSkeletonComponent(handTarget);
                handTarget.leap.CheckSensor(handTarget, handTarget.humanoid.leapTracker);

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    skeletonProp.objectReferenceValue = (LeapHandSkeleton)EditorGUILayout.ObjectField("Skeleton", skeletonProp.objectReferenceValue, typeof(LeapHandSkeleton), true);
                    EditorGUI.indentLevel--;
                }
            }

            //protected static void CheckSkeletonComponent(HandTarget handTarget) {
            //    if (handTarget.leap.handSkeleton == null) {
            //        handTarget.leap.handSkeleton = handTarget.leap.FindHandSkeleton(handTarget.isLeft);
            //        if (handTarget.leap.handSkeleton == null)
            //            handTarget.leap.handSkeleton = handTarget.leap.CreateHandSkeleton(handTarget.isLeft, handTarget.showRealObjects);
            //    }

            //}

        }
        #endregion

#endif
    }
}
