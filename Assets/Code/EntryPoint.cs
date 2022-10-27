using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntryPoint : MonoBehaviourWrapper
{
    [SerializeField] private VRRig _vrRig;
    [SerializeField] private BeatSaberExercise _beatSaberExercise;
    [SerializeField] private GlyphExercise _glyphExercise;
    [SerializeField] private ScenarioConfigFactory _scenarioConfigFactory;
    [SerializeField] private SymmetricMovement _symmetricMovement;
    [SerializeField] private HandsInitialization _handsInitialization;

    private void Awake()
    {
        var dependencies = new object[]
        {
            //_beatSaberExercise,
            _glyphExercise,
            _handsInitialization, 
            _symmetricMovement,
            _vrRig
        };
        
        InitScenarioComponents(dependencies);
        SetDependencies(dependencies);
    }

    private void InitScenarioComponents(object[] dependencies)
    {
        ScenarioConfig scenarioConfig = _scenarioConfigFactory.Create();
        IEnumerable<IScenarioComponent> scenarioComponents = dependencies.OfType<IScenarioComponent>();
        
        scenarioComponents.InitForEach(scenarioConfig);
    }

    
}