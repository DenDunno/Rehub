using Passer;
using Passer.Humanoid;
using Passer.Tracking;
using UnityEngine;

public class VirtualHand : OpenVRHand
{
    public override void Update()
    {
        if (tracker == null || !tracker.enabled || !enabled)
            return;

#if hVIVEHAND
            if (openVRTracker.handTracking) {
                if (handSkeleton == null)
                    handSkeleton = ViveHandSkeleton.Get(handTarget.humanoid.openVR.tracker.transform, handTarget.isLeft, handTarget.showRealObjects);
                useSkeletalInput = true;
            }
#endif
        if (useSkeletalInput) {
            if (handSkeleton == null)
                handSkeleton = OpenVRHandSkeleton.Get(handTarget.humanoid.openVR.tracker.transform, handTarget.isLeft, handTarget.showRealObjects);

            if (handSkeleton != null) {
                SetPoseForVirtualHand();
                handTarget.hand.target.confidence.position = 0.9F;
                handTarget.hand.target.confidence.rotation = 0.9F;
                //openVRController.show = false;
            }
            else {
                handTarget.hand.target.confidence.position = 0.0F;
                handTarget.hand.target.confidence.rotation = 0.0F;
            }
            if (openVRController != null)
                UpdateInput();
            return;
        }

        if (openVRController == null) {
            UpdateTarget(handTarget.hand.target, sensorTransform);
            return;
        }

        if (openVRController.isLeft != handTarget.isLeft) {
            // Reassign controller when the left and right have swapped
            openVRController.trackerId = -1;
        }

        openVRController.UpdateComponent();
        if (openVRController.status != Tracker.Status.Tracking)
            return;
        //#endif
        UpdateTarget(handTarget.hand.target, openVRController);
        UpdateInput();
        UpdateHand();
    }

    private void SetPoseForVirtualHand()
    {
        Vector3 rotation = handTarget.isLeft ? new Vector3(-90, 0, 0) : new Vector3(90, 0, 0);
        handTarget.hand.target.transform.localRotation = Quaternion.Euler(rotation);
    }
}