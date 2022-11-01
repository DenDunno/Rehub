﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VRRigMapping : IUpdate, IScenarioComponent
{
    [SerializeField] private VRMap _headMapping;
    [SerializeField] private VRMap _leftArmMapping;
    [SerializeField] private VRMap _rightArmMapping;
    [SerializeField] private RightRigFingers _rightRigFingers;
    [SerializeField] private LeftRigFingers _leftRigFingers;
    private List<IMap> _vrMaps;
    
    public void Init(ScenarioConfig scenarioConfig)
    {
        _vrMaps = new List<IMap>() { _headMapping, _rightRigFingers, _leftRigFingers};

        TryAddBodyPartToMap(scenarioConfig, AmputatedBodyPart.LeftArm, _leftArmMapping);
        TryAddBodyPartToMap(scenarioConfig, AmputatedBodyPart.RightArm, _rightArmMapping);
    }

    private void TryAddBodyPartToMap(ScenarioConfig scenarioConfig, AmputatedBodyPart target, IMap map)
    {
        if (scenarioConfig.AmputatedBodyPart != target)
        {
            _vrMaps.Add(map);
        }
    }

    void IUpdate.Update()
    {
        _vrMaps.ForEach(vrMap => vrMap.Map());
    }
}