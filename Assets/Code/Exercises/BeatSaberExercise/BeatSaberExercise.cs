using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BeatSaberExercise : Exercise, IInitializable 
{
    [SerializeField] [AssetList] private List<Obstacle> _obstaclesPrefabs;
    [SerializeField] private float _coolDown = 2f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private Vector3 _spawnPositionOffset;

    void IInitializable.Init()
    {
        var obstacleTouchFeedback = new ObstacleTouchFeedback();
        var obstacleSpawnData = new ObstacleSpawnData(transform, obstacleTouchFeedback.OnTouch, _spawnPositionOffset);
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