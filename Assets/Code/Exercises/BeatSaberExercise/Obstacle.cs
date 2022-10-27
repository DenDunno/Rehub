using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour, IPoolableObject
{
    private ValveControllerInput _virtualHand;
    private Action _touchCallback;
    
    public bool IsActive { get; private set; }

    public void Init(ObstacleSpawnData obstacleSpawnData)
    {
        IsActive = true;
        _virtualHand = obstacleSpawnData.VirtualHand;
        _touchCallback = obstacleSpawnData.TouchCallback;
        transform.parent = obstacleSpawnData.Parent;
        
        int randomPositionIndex = Random.Range(0, obstacleSpawnData.Positions.Length);
        transform.localPosition = obstacleSpawnData.Positions[randomPositionIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ValveControllerInput controller))
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