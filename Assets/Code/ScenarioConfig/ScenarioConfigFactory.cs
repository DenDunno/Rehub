using System;
using UnityEngine;

[Serializable]
public class ScenarioConfigFactory
{
    [SerializeField] private ValveControllerInput _rightController;
    [SerializeField] private ValveControllerInput _leftController;
    [SerializeField] private AmputatedBodyPart _amputatedBodyPart;
    
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

        (ValveControllerInput realHand, ValveControllerInput virtualHand) = (_rightController, _leftController);
            
        if (scenarioConfig.AmputatedBodyPart == AmputatedBodyPart.RightArm)
        {
            Algorithms.Swap(ref virtualHand, ref realHand);
        }
            
        scenarioConfig.SetRealAndVirtualHands(realHand, virtualHand);
    }

    private AmputatedBodyPart GetAmputatedBodyPart()
    {
        return _amputatedBodyPart;
    }
}