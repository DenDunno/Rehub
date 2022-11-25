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
        ObstacleTouchFeedback obstacleTouchFeedback = new();
        ObstacleSpawnData obstacleSpawnData = new(transform, obstacleTouchFeedback.OnTouch, _spawnPositionOffset);
        ObstacleSpawner obstacleSpawner = new(_obstaclesPrefabs, obstacleSpawnData);
        ObstaclesMovement obstaclesMovement = new(_direction, _speed);
        ObstacleCooldownSpawning obstacleCooldownSpawning = new(obstacleSpawner, obstaclesMovement, _coolDown);

        SetDependencies(new object[]
        {
            obstaclesMovement,
            obstacleSpawner,
            obstacleCooldownSpawning
        });
    }
}