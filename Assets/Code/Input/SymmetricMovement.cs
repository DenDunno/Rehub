using System;
using UnityEngine;

[Serializable]
public class SymmetricMovement : IUpdate
{
    [SerializeField] private Vector3 _rotation;
    private HandsMovementData _data;

    public void Init(HandsMovementData data)
    {
        _data = data;
    }

    public void Update()
    {
        _data.Follower.transform.position = _data.Target.transform.position + _data.Target.transform.root.transform.right * _data.Offset;
        _data.Follower.transform.rotation = _data.Target.transform.rotation * Quaternion.Euler(_rotation);
    }
}