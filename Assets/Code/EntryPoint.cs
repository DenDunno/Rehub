using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private VRRig _vrRig;
    [SerializeField] private ExerciseProxy _exerciseProxy;
    [SerializeField] private BodyPart _amputatedBodyPart;

    private void Awake()
    {
        ScenarioConfig scenarioConfig = ScenarioConfigFactory.Create(_vrRig.HumanoidBodyParts, _amputatedBodyPart);
        
        _exerciseProxy.Init(scenarioConfig);
        _vrRig.Init(scenarioConfig);
        
        _exerciseProxy.Play<BeatSaberExercise>();
    }
}