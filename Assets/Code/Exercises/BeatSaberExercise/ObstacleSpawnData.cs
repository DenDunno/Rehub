using System;
using UnityEngine;

public class ObstacleSpawnData
{
    public readonly Transform Parent;
    public readonly ValveControllerInput VirtualHand;
    public readonly Vector3[] Positions;
    public readonly Action TouchCallback;
    
    public ObstacleSpawnData(Transform parent, ValveControllerInput virtualHand, Action touchCallback, Vector3 offset)
    {
        Parent = parent;
        VirtualHand = virtualHand;
        TouchCallback = touchCallback;
        Positions = new[]
        {
            offset,
            Vector3.zero,
            -offset
        };;
    }
}