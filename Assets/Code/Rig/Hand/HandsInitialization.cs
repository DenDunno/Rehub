using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

[Serializable]
public class HandsInitialization : IScenarioComponent
{
    [SerializeField] private VirtualHand _leftVRVirtualHand;
    [SerializeField] private VirtualHand _rightVRVirtualHand;
    [SerializeField] private LeftRigHand _leftRigHand;
    [SerializeField] private RightRigHand _rightRigHand;

    public void Init(ScenarioConfig scenarioConfig)
    {
        InitVRHands();
        InitRigHands(scenarioConfig.AmputatedBodyPart);
    }

    private void InitVRHands()
    {
        // _leftVRHand.Init();
        // _rightVRHand.Init();
    }

    private void InitRigHands(AmputatedBodyPart amputatedPart)
    {
        VirtualHand trackingVrVirtualHandForLeft = amputatedPart == AmputatedBodyPart.LeftArm ? _rightVRVirtualHand : _leftVRVirtualHand;
        VirtualHand trackingVrVirtualHandForRight = amputatedPart == AmputatedBodyPart.RightArm ? _leftVRVirtualHand : _rightVRVirtualHand;
        
        //_leftRigHand.Init(trackingVrHandForLeft.VrHand.Fingers);
        //_rightRigHand.Init(trackingVrHandForRight.VrHand.Fingers);
    }
}