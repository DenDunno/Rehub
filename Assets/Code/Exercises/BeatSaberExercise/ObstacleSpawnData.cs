using System;
using UnityEngine;

public class ObstacleSpawnData
{
    public readonly Transform Parent;
    public readonly Vector3[] Positions;
    public readonly Action TouchCallback;
    
    public ObstacleSpawnData(Transform parent, Action touchCallback, Vector3 offset)
    {
        Parent = parent;
        TouchCallback = touchCallback;
        Positions = new[]
        {
            offset,
            Vector3.zero,
            -offset
        };
    }
}