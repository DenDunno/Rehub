using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

[Serializable]
public class HandsInitialization : IScenarioComponent
{
    [SerializeField] private Hand _leftVRHand;
    [SerializeField] private Hand _rightVRHand;
    [SerializeField] private LeftRigHand _leftRigHand;
    [SerializeField] private RightRigHand _rightRigHand;

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
        
        _leftRigHand.Init(trackingVrHandForLeft.VrHand.Fingers);
        _rightRigHand.Init(trackingVrHandForRight.VrHand.Fingers);
    }
}