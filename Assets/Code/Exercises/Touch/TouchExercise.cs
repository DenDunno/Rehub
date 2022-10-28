using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TouchExercise : Exercise, IScenarioComponent
{
    [SerializeField] [AssetsOnly] private LightBulb _lightBulb;
    
    public void Init(ScenarioConfig scenarioConfig)
    {
        var obstacles = new List<Obstacle>() { _lightBulb };
        
        // var obstacleTouchFeedback = new ObstacleTouchFeedback();
        // var obstacleSpawnData = new ObstacleSpawnData(transform, scenarioConfig.VirtualHand, obstacleTouchFeedback.OnTouch, _spawnPositionOffset);
        // var obstacleSpawner = new ObstacleSpawner(obstacles, obstacleSpawnData);

        SetDependencies(scenarioConfig, new object[]
        {
            
        });
    }
}