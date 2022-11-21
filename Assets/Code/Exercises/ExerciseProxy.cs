
public class ExerciseProxy : MonoBehaviourWrapper, IScenarioComponent
{
    private Exercise[] _exercises;

    public void Init(ScenarioConfig scenarioConfig)
    {
        _exercises = GetComponentsInChildren<Exercise>(true);
        
        SetDependencies(scenarioConfig, _exercises);
        Play<TouchExercise>();
    }

    public void Play<T>() where T : Exercise
    {
        Find<T>().Play();
    }

    public void Stop<T>() where T : Exercise
    {
        Find<T>().Stop();
    }

    private Exercise Find<T>() where T : Exercise
    {
        foreach (Exercise exercise in _exercises)
        {
            if (exercise is T)
            {
                return exercise;
            }
        }

        return null;
    }
}