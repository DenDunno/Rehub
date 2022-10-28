using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BeatSaberExercise : Exercise, IScenarioComponent 
{
    [SerializeField] [AssetList] private List<Obstacle> _obstaclesPrefabs;
    [SerializeField] private float _coolDown = 2f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private Vector3 _spawnPositionOffset;

    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        var obstacleTouchFeedback = new ObstacleTouchFeedback();
        var obstacleSpawnData = new ObstacleSpawnData(transform, scenarioConfig.VirtualHand, obstacleTouchFeedback.OnTouch, _spawnPositionOffset);
        var obstacleSpawner = new ObstacleSpawner(_obstaclesPrefabs, obstacleSpawnData);
        var obstaclesMovement = new ObstaclesMovement(_direction, _speed);
        var obstacleCooldownSpawning = new ObstacleCooldownSpawning(obstacleSpawner, obstaclesMovement, _coolDown);

        SetDependencies(new object[]
        {
            obstaclesMovement,
            obstacleSpawner,
            obstacleCooldownSpawning
        });
    }
}