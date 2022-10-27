using UnityEngine;

public class  VRRig : MonoBehaviourWrapper, IScenarioComponent
{
    [SerializeField] private HeadBodyOffset _headBodyOffset;
    [SerializeField] private VRRigMapping _vrRigMapping;
    
    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        _vrRigMapping.Init(scenarioConfig);
        
        SetDependencies(new object[]
        {
            _headBodyOffset,
            _vrRigMapping
        });
    }
}