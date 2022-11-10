using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;
    [SerializeField] private Vector3 _rotation;
    [SerializeField]private Vector3 _position;
    public static Vector3 Rotation;
    public static Vector3 Position;

    private void Awake()
    {
        ScenarioConfig scenarioConfig = _scenarioConfigFactory.Create();
        
        _exerciseProxy.Init(scenarioConfig);
        _exerciseProxy.Play<BeatSaberExercise>();
    }

    private void Update()
    {
        Rotation = _rotation;
        Position = _position;
    }
}