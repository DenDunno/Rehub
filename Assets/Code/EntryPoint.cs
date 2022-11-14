using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;

    private void Awake()
    {
        ScenarioConfig scenarioConfig = _scenarioConfigFactory.Create();
        
        _exerciseProxy.Init(scenarioConfig);
        _exerciseProxy.Play<BeatSaberExercise>();
    }
}