using System;
using Passer.Humanoid;
using UnityEngine;

[Serializable]
public class ScenarioConfigFactory
{
    [SerializeField] private HandTarget _leftHandTarget;
    [SerializeField] private HandTarget _rightHandTarget;
    [SerializeField] private AmputatedBodyPart _amputatedBodyPart;
    [SerializeField] private WristMovementType _wristMovementType;
    
    public ScenarioConfig Create()
    {
        AmputatedBodyPart amputatedBodyPart = GetAmputatedBodyPart();
        var scenarioConfig = new ScenarioConfig(amputatedBodyPart);
        
        TrySetUpRealAndVirtualHands(scenarioConfig);

        return scenarioConfig; 
    }

    private void TrySetUpRealAndVirtualHands(ScenarioConfig scenarioConfig)
    {
        if (scenarioConfig.IsHandAmputated == false)
        {
            return;
        }

        (HandTarget realHand, HandTarget virtualHand) = GetRealAndVirtualHand(scenarioConfig.AmputatedBodyPart);

        scenarioConfig.SetRealAndVirtualHands(realHand, virtualHand);
    }

    private (HandTarget, HandTarget) GetRealAndVirtualHand(AmputatedBodyPart amputatedBodyPart)
    {
        HandTarget virtualHand = _leftHandTarget;
        HandTarget realHand = _rightHandTarget;
        bool isRightHandVirtual = amputatedBodyPart == AmputatedBodyPart.RightArm;

        if (isRightHandVirtual)
        {
            Algorithms.Swap(ref virtualHand, ref realHand);
        }

        VirtualHand virtualHandLogic = new(realHand);
        virtualHandLogic.SetMovementType(_wristMovementType);
        
        virtualHand.openVR = virtualHandLogic;
        virtualHand.viveTracker.enabled = true;

        return (realHand, virtualHand);
    }

    private AmputatedBodyPart GetAmputatedBodyPart()
    {
        return _amputatedBodyPart;
    }
}