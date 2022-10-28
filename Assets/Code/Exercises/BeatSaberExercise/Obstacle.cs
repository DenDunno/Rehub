using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour, IPoolableObject
{
    private Action _touchCallback;

    public void Init(ObstacleSpawnData obstacleSpawnData)
    {
        IsActive = true;
        _touchCallback = obstacleSpawnData.TouchCallback;
        transform.parent = obstacleSpawnData.Parent;
        
        int randomPositionIndex = Random.Range(0, obstacleSpawnData.Positions.Length);
        transform.localPosition = obstacleSpawnData.Positions[randomPositionIndex];
    }

    public bool IsActive { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ValveControllerInput>() != null)
        {
            IsActive = false;
            _touchCallback?.Invoke();
        }
        
        if (other.GetComponent<BeatSaberWall>() != null)
        {
            IsActive = false;
        }
    }
}