using UnityEngine;

public class ObstacleCooldownSpawning : IUpdate
{
    private readonly ObstacleSpawner _spawner;
    private readonly ObstaclesMovement _movement;
    private readonly float _coolDown;
    private float _clock;
    
    public ObstacleCooldownSpawning(ObstacleSpawner spawner, ObstaclesMovement movement, float coolDown)
    {
        _spawner = spawner;
        _movement = movement;
        _coolDown = coolDown;
    }

    void IUpdate.Update()
    {
        TrySpawnObstacle();
    }

    private void TrySpawnObstacle()
    {
        if (Time.time > _clock + _coolDown)
        {
            _clock = Time.time;
            Obstacle obstacle = _spawner.Create();
            _movement.TryAddObstacle(obstacle);
        }
    }
}