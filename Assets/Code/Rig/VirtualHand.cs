using Passer.Humanoid;
using Passer.Humanoid.Tracking;
using Passer.Tracking;
using UnityEngine;

public class VirtualHand : OpenVRHand
{
    private readonly HandTarget _realHand;
    private WristMovement _wristMovement;

    public VirtualHand(HandTarget realHand)
    {
        _realHand = realHand;
    }

    public void SetMovementType(WristMovementType wristMovementType)
    {
        _wristMovement = new WristMovement(wristMovementType, handTarget, _realHand);
    }

    public override void Update()
    {
        if (tracker == null || !tracker.enabled || !enabled)
            return;

        if (useSkeletalInput) 
        {
            if (handSkeleton == null)
            {
                handSkeleton = OpenVRHandSkeleton.Get(handTarget.humanoid.openVR.tracker.transform, handTarget.isLeft, handTarget.showRealObjects);   
            }
            
            SetPoseForVirtualHand();

            if (openVRController != null)
            {
                UpdateInput();
                openVRController.UpdateComponent();
            }
                
        }
    }

    private void SetPoseForVirtualHand()
    {
        _wristMovement.Update();
        UpdateFingers();
    }
    
    private void UpdateFingers()
    {
        UpdateThumb();
        UpdateFinger(handTarget.fingers.index, Finger.Index);
        UpdateFinger(handTarget.fingers.middle, Finger.Middle);
        UpdateFinger(handTarget.fingers.ring, Finger.Ring);
        UpdateFinger(handTarget.fingers.little, Finger.Little);
    }

    private void MapPhalange(FingersTarget.TargetedPhalanges realPhalange, FingersTarget.TargetedPhalanges virtualPhalange)
    {
        virtualPhalange.target.transform.localRotation = Quaternion.Inverse(realPhalange.target.transform.localRotation);
    }

    private void UpdateThumb()
    {
        MapThumbFinger(_realHand.fingers.thumb.proximal, handTarget.fingers.thumb.proximal);
        MapThumbFinger(_realHand.fingers.thumb.intermediate, handTarget.fingers.thumb.intermediate);
        MapThumbFinger(_realHand.fingers.thumb.distal, handTarget.fingers.thumb.distal);
    }

    private void MapThumbFinger(FingersTarget.TargetedPhalanges realFinger, FingersTarget.TargetedPhalanges virtualFinger)
    {
        virtualFinger.target.transform.localRotation = Quaternion.Inverse(realFinger.target.transform.localRotation);
        virtualFinger.target.transform.localPosition = realFinger.target.transform.localPosition;
    }

    private void UpdateFinger(FingersTarget.TargetedFinger finger, Finger index) 
    {
        UpdateFingerBone(finger.proximal.target.transform, index, FingerBone.Proximal);
        UpdateFingerBone(finger.intermediate.target.transform, index, FingerBone.Intermediate);
        UpdateFingerBone(finger.distal.target.transform, index, FingerBone.Distal);
    }

    private void UpdateFingerBone(Transform targetTransform, Finger finger, FingerBone fingerBone) 
    {
        try
        {
            HandSkeleton realHandSkeleton = OpenVRHandSkeleton.Get(_realHand.humanoid.openVR.tracker.transform, _realHand.isLeft, _realHand.showRealObjects);
            Transform realBone = realHandSkeleton.GetBone(finger, fingerBone);
            
            if (handSkeleton.isLeft)
            {
                targetTransform.localRotation = Quaternion.Inverse(realBone.localRotation);
                
                if (finger == Finger.Middle)
                {
                    targetTransform.localRotation *= Quaternion.Euler(-10, 0, 0);
                }
            }
            else
            {
                targetTransform.localRotation = realBone.localRotation;
                
                if (finger == Finger.Index)
                {
                    targetTransform.localRotation *= Quaternion.Euler(0, -5, 0);
                }
            }
        }
        catch
        {
        }
    }
}