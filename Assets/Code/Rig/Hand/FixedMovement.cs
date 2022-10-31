using System;
using UnityEngine;

[Serializable]
public class FixedMovement : IUpdate
{
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Vector3 _offset;
    private HandsMovementData _data;

    public void Init(HandsMovementData data)
    {
        _data = data;
    }

    public void Update()
    {
        _data.Follower.transform.localRotation = Quaternion.Euler(_rotation);
        _data.Follower.transform.localPosition = _offset;   
    }
}