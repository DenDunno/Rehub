using UnityEngine;

public class  VRRig : MonoBehaviourWrapper, IScenarioComponent
{
    [SerializeField] private HeadBodyOffset _headBodyOffset;
    [SerializeField] private VRRigMapping _vrRigMapping;
    [SerializeField] private PlayerHands _playerHands;
    
    public void Init(ScenarioConfig scenarioConfig)
    {
        SetDependencies(scenarioConfig, new object[]
        {
            _playerHands,
            _headBodyOffset,
            _vrRigMapping
        });
    }
}