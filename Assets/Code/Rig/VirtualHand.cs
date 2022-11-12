﻿using System;
using Passer.Humanoid;
using Passer.Humanoid.Tracking;
using Passer.Tracking;
using UnityEngine;

[Serializable]
public class VirtualHand : OpenVRHand
{
    [SerializeField] private WristMovementType _wristMovementType;
    private HandTarget _realHand;
    private GameObject _gam;

    public void SetRealHand(HandTarget realHand)
    {
        _realHand = realHand;
        _gam = new GameObject();
    }
    
    public void SetMovementType(WristMovementType wristMovementType)
    {
        _wristMovementType = wristMovementType;
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
        UpdateWrist();
        UpdateFingers();
    }

    private void UpdateWrist()
    {
        if (_wristMovementType == WristMovementType.Fixed)
        {
            Vector3 rotation = handTarget.isLeft ? new Vector3(-90, 0, 0) : new Vector3(90, 0, 0);
            handTarget.hand.target.transform.localRotation = Quaternion.Euler(rotation);
        }
        else if (_wristMovementType == WristMovementType.Symmetric)
        {
            handTarget.hand.target.transform.rotation = _realHand.hand.target.transform.rotation * Quaternion.Euler(0, 0, 180);
        }
        else if (_wristMovementType == WristMovementType.Mirror)
        {
            Quaternion localRotation = _realHand.hand.target.transform.localRotation;
            handTarget.hand.target.transform.localRotation = new Quaternion(localRotation.x, localRotation.y, -localRotation.z, localRotation.w * -1.0f);
            handTarget.hand.target.transform.localRotation *= Quaternion.Euler(EntryPoint.Rotation);
        }
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