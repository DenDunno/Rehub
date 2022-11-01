using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

[Serializable]
public class HandsInitialization : IScenarioComponent
{
    [SerializeField] private Hand _leftVRHand;
    [SerializeField] private Hand _rightVRHand;
    [SerializeField] private LeftRigFingers _leftRigFingers;
    [SerializeField] private RightRigFingers _rightRigFingers;

    public void Init(ScenarioConfig scenarioConfig)
    {
        InitVRHands();
        InitRigHands(scenarioConfig.AmputatedBodyPart);
    }

    private void InitVRHands()
    {
        _leftVRHand.Init();
        _rightVRHand.Init();
    }

    private void InitRigHands(AmputatedBodyPart amputatedPart)
    {
        Hand trackingVrHandForLeft = amputatedPart == AmputatedBodyPart.LeftArm ? _rightVRHand : _leftVRHand;
        Hand trackingVrHandForRight = amputatedPart == AmputatedBodyPart.RightArm ? _leftVRHand : _rightVRHand;
        
        _leftRigFingers.Init(trackingVrHandForLeft.VrHand.Fingers);
        _rightRigFingers.Init(trackingVrHandForRight.VrHand.Fingers);
    }
}