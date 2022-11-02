using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private VRRig _vrRig;
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;

    private void Awake()
    {
        ScenarioConfig scenarioConfig = _scenarioConfigFactory.Create();
        
        _vrRig.Init(scenarioConfig);
        _exerciseProxy.Init(scenarioConfig);
    }
}