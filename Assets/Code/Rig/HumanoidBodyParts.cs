using System;
using Passer.Humanoid;
using UnityEngine;

[Serializable]
public class HumanoidBodyParts
{
    [SerializeField] private HumanoidControl _humanoidControl;

    public HandTarget RightHand => _humanoidControl.rightHandTarget;
    public HandTarget LeftHand => _humanoidControl.leftHandTarget;
    public FootTarget RightFoot => _humanoidControl.rightFootTarget;
    public FootTarget LeftFoot => _humanoidControl.leftFootTarget;
}