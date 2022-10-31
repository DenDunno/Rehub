using System;
using UnityEngine;

[Serializable]
public class MirrorMovement : IUpdate
{
    [SerializeField] private Vector3 _rotation;
    private HandsMovementData _data;

    public void Init(HandsMovementData data)
    {
        _data = data;
    }
    
    public void Update()
    {
        Quaternion localRotation = _data.Target.transform.localRotation;
        
        _data.Follower.transform.position = _data.Target.transform.position + _data.Target.transform.root.transform.right * _data.Offset;
        _data.Follower.transform.localRotation = new Quaternion(localRotation.x * -1.0f, localRotation.y, localRotation.z, localRotation.w * -1.0f);
        _data.Follower.transform.localRotation *= Quaternion.Euler(_rotation);
    }
}