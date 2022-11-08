using System;
using Passer;
using Passer.Humanoid;
using Passer.Humanoid.Tracking;
using Passer.Tracking;
using UnityEngine;

public class VirtualHand : OpenVRHand
{
    private readonly HandTarget _realHand;

    public VirtualHand(HandTarget realHand)
    {
        _realHand = realHand;
    }

    public override void Update()
    {
        if (tracker == null || !tracker.enabled || !enabled)
            return;

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
        
        UpdateFingers();
    }

    private void UpdateFingers()
    {
        UpdateFinger(handTarget.fingers.thumb, Finger.Thumb);
        UpdateFinger(handTarget.fingers.index, Finger.Index);
        UpdateFinger(handTarget.fingers.middle, Finger.Middle);
        UpdateFinger(handTarget.fingers.ring, Finger.Ring);
        UpdateFinger(handTarget.fingers.little, Finger.Little);
    }
    
    private void UpdateFinger(FingersTarget.TargetedFinger finger, Finger index) 
    {
        UpdateFingerBone(finger.proximal.target.transform, index, FingerBone.Proximal);
        UpdateFingerBone(finger.intermediate.target.transform, index, FingerBone.Intermediate);
        UpdateFingerBone(finger.distal.target.transform, index, FingerBone.Distal);
    }

    private void UpdateFingerBone(Transform targetTransform, Finger finger, FingerBone fingerBone) 
    {
        HandSkeleton realHandSkeleton = OpenVRHandSkeleton.Get(_realHand.humanoid.openVR.tracker.transform, _realHand.isLeft, _realHand.showRealObjects);

        try
        {
            Transform thisBoneTransform = realHandSkeleton.GetBone(finger, fingerBone);
            targetTransform.localRotation = Quaternion.Inverse(thisBoneTransform.localRotation);
        }
        catch { }
    }
}