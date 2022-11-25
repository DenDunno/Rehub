using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : IInitializable, IUpdate
{
    private readonly List<ObstaclePool> _obstaclePools = new();
    private readonly List<Obstacle> _obstaclesPrefabs;
    private readonly ObstacleSpawnData _obstacleSpawnData;

    public ObstacleSpawner(List<Obstacle> obstaclesPrefabs, ObstacleSpawnData obstacleSpawnData)
    {
        _obstaclesPrefabs = obstaclesPrefabs;
        _obstacleSpawnData = obstacleSpawnData;
    }

    void IInitializable.Init()
    {
        foreach (Obstacle obstacle in _obstaclesPrefabs)
        {
            _obstaclePools.Add(new ObstaclePool(obstacle));
        }
    }

    public Obstacle Create()
    {
        int randomObstacleIndex = Random.Range(0, _obstaclePools.Count);
        
        ObstaclePool obstaclePool = _obstaclePools[randomObstacleIndex]; 
        Obstacle obstacle = obstaclePool.Create();
        obstacle.Init(_obstacleSpawnData);
        
        return obstacle;
    }

    void IUpdate.Update()
    {
        _obstaclePools.UpdateForEach();
    }
}