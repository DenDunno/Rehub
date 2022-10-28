using UnityEngine;

public class  VRRig : MonoBehaviourWrapper, IScenarioComponent
{
    [SerializeField] private HeadBodyOffset _headBodyOffset;
    [SerializeField] private VRRigMapping _vrRigMapping;
    [SerializeField] private SymmetricMovement _symmetricMovement;
    [SerializeField] private HandsInitialization _handsInitialization;
    
    public void Init(ScenarioConfig scenarioConfig)
    {
        SetDependencies(scenarioConfig, new object[]
        {
            _handsInitialization,
            _symmetricMovement,
            _headBodyOffset,
            _vrRigMapping
        });
    }
}