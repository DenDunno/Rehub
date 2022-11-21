using UnityEngine;

public class VRRig : MonoBehaviourWrapper
{
    [SerializeField] private WristMovementType _wristMovementType;
    [field: SerializeField] public HumanoidBodyParts HumanoidBodyParts { get; private set; }
    
    public void Init(ScenarioConfig scenarioConfig)
    {
        SetDependencies(scenarioConfig, new object[]
        {
            new HandsInitialization(_wristMovementType),
            new LegsInitialization(),
        });
    }
}