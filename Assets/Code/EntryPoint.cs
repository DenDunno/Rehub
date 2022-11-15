using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;
    [SerializeField] private HandsInitialization _handsInitialization;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Vector4 _rot;
    public static Vector3 Rotation;

    private void Awake()
    {
        ScenarioConfig scenarioConfig = _scenarioConfigFactory.Create();
        
        _handsInitialization.Init(scenarioConfig);
        _exerciseProxy.Init(scenarioConfig);
        _exerciseProxy.Play<TouchExercise>();
    }

    private void Update()
    {
        Rotation = _rotation;
    }
}