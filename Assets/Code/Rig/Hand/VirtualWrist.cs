using System;
using System.Collections.Generic;
using Passer.Humanoid;
using UnityEngine;

public class VirtualWrist
{
    private readonly WristMovementType _wristMovementType;
    private readonly HandTarget _virtualHand;
    private readonly HandTarget _realHand;
    private readonly Dictionary<WristMovementType, Action> _wristMovementStrategy;

    public VirtualWrist(WristMovementType wristMovementType, HandTarget virtualHand, HandTarget realHand)
    {
        _wristMovementType = wristMovementType;
        _virtualHand = virtualHand;
        _realHand = realHand;
        _wristMovementStrategy = new Dictionary<WristMovementType, Action>()
        {
            { WristMovementType.Fixed, UpdateFixedPose},
            { WristMovementType.Symmetric, UpdateSymmetricMovement},
            { WristMovementType.Mirror, UpdateMirrorMovement}
        };
    }

    public void Update()
    {
        _virtualHand.hand.target.transform.position = _virtualHand.hand.bone.transform.position;
        _wristMovementStrategy[_wristMovementType]();
    }

    private void UpdateFixedPose()
    {
        _virtualHand.hand.target.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    private void UpdateSymmetricMovement()
    {
        _virtualHand.hand.target.transform.rotation = _realHand.hand.target.transform.rotation * Quaternion.Euler(0, 0, 180);
    }

    private void UpdateMirrorMovement()
    {
        UpdateSymmetricMovement();
    }

    private void ClampWrist()
    {
        Vector3 coneNormal = -_virtualHand.hand.bone.transform.parent.up;
        Vector3 wristDirection = _virtualHand.hand.target.transform.right;
        
        float targetAngle = 60;

        float angle = Vector3.Angle(coneNormal, wristDirection);
        
        if (angle > targetAngle)
        {
            Vector3 target = Quaternion.AngleAxis(angle - targetAngle, Vector3.Cross(wristDirection, coneNormal)) * wristDirection;
            
            _virtualHand.hand.target.transform.right = target;
            _virtualHand.hand.target.transform.localRotation *= Quaternion.Euler(-90, 0, 0);

            Debug.DrawRay(_virtualHand.transform.position, target, Color.green);
        }
    }
}