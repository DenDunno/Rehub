using System.Collections.Generic;
using UnityEngine;

public class ObstaclesMovement : IUpdate
{
    private readonly List<Obstacle> _activeObstacles = new();
    private readonly Vector3 _direction;
    private readonly float _speed;

    public ObstaclesMovement(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
    }
    
    public void TryAddObstacle(Obstacle obstacle)
    {
        if (_activeObstacles.Contains(obstacle) == false)
        {
            _activeObstacles.Add(obstacle);
        }
    }
    
    void IUpdate.Update()
    {
        foreach (Obstacle activeObstacle in _activeObstacles)
        {
            activeObstacle.transform.localPosition += _direction * (_speed * Time.deltaTime);
        }
    }
}