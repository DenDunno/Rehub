using System;
using Passer.Humanoid;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class HandsInitialization
{
    [SerializeField] private WristMovementType _wristMovementType;
    private ScenarioConfig _scenarioConfig;

    public void Init(ScenarioConfig scenarioConfig)
    {
        _scenarioConfig = scenarioConfig;
        SetWristMovementType();
        scenarioConfig.VirtualHand.viveTracker.enabled = true;
    }

    [Button]
    private void SetWristMovementType()
    {
        _scenarioConfig.VirtualHand.openVR = new VirtualHand(_scenarioConfig.VirtualHand, _scenarioConfig.RealHand, _wristMovementType);
    }
}