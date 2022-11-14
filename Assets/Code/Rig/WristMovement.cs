using System;
using System.Collections.Generic;
using Passer.Humanoid;
using UnityEngine;

public class WristMovement
{
    private readonly WristMovementType _wristMovementType;
    private readonly HandTarget _virtualHand;
    private readonly HandTarget _realHand;
    private readonly Dictionary<WristMovementType, Action> _wristMovementStrategy;
    
    public WristMovement(WristMovementType wristMovementType, HandTarget virtualHand, HandTarget realHand)
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
        _wristMovementStrategy[_wristMovementType]();
    }

    private void UpdateFixedPose()
    {
        Vector3 rotation = _virtualHand.isLeft ? new Vector3(-90, 0, 0) : new Vector3(90, 0, 0);
        _virtualHand.hand.target.transform.localRotation = Quaternion.Euler(rotation);
    }

    private void UpdateSymmetricMovement()
    {
        _virtualHand.hand.target.transform.rotation = _realHand.hand.target.transform.rotation * Quaternion.Euler(0, 0, 180);
    }

    private void UpdateMirrorMovement()
    {
    }
}