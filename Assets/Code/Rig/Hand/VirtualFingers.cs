using Passer.Humanoid;
using Passer.Humanoid.Tracking;
using Passer.Tracking;
using UnityEngine;

public class VirtualFingers
{
    private readonly HandTarget _virtualHand;
    private readonly HandTarget _realHand;

    public VirtualFingers(HandTarget virtualHand, HandTarget realHand)
    {
        _virtualHand = virtualHand;
        _realHand = realHand;
    }
    
    public void Update()
    {
        UpdateThumb();
        UpdateFinger(_virtualHand.fingers.index, Finger.Index);
        UpdateFinger(_virtualHand.fingers.middle, Finger.Middle);
        UpdateFinger(_virtualHand.fingers.ring, Finger.Ring);
        UpdateFinger(_virtualHand.fingers.little, Finger.Little);
    }

    private void UpdateThumb()
    {
        MapThumbFinger(_realHand.fingers.thumb.proximal, _virtualHand.fingers.thumb.proximal);
        MapThumbFinger(_realHand.fingers.thumb.intermediate, _virtualHand.fingers.thumb.intermediate);
        MapThumbFinger(_realHand.fingers.thumb.distal, _virtualHand.fingers.thumb.distal);
    }

    private void UpdateFinger(FingersTarget.TargetedFinger finger, Finger index) 
    {
        UpdateFingerBone(finger.proximal.target.transform, index, FingerBone.Proximal);
        UpdateFingerBone(finger.intermediate.target.transform, index, FingerBone.Intermediate);
        UpdateFingerBone(finger.distal.target.transform, index, FingerBone.Distal);
    }

    private void MapThumbFinger(FingersTarget.TargetedPhalanges realFinger, FingersTarget.TargetedPhalanges virtualFinger)
    {
        virtualFinger.target.transform.localRotation = Quaternion.Inverse(realFinger.target.transform.localRotation);
        virtualFinger.target.transform.localPosition = realFinger.target.transform.localPosition;
    }

    private void UpdateFingerBone(Transform targetTransform, Finger finger, FingerBone fingerBone) 
    {
        try
        {
            HandSkeleton realHandSkeleton = OpenVRHandSkeleton.Get(_realHand.humanoid.openVR.tracker.transform, _realHand.isLeft, _realHand.showRealObjects);
            Transform realBone = realHandSkeleton.GetBone(finger, fingerBone);
            
            if (_virtualHand.isLeft)
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