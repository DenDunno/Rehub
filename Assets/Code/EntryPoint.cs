using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector4 _rot;
    public static Vector3 Rotation;
    public static Vector3 Position;
    public static Vector4 Rot;

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
        Rot = _rot;
    }
}