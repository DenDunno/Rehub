using System;
using UnityEngine;

[Serializable]
public class VRMap : IMap
{
    [SerializeField] private VRTarget _vrTarget;
    [SerializeField] private RigTarget _rigTarget;
    [SerializeField] private Vector3 _trackingPositionOffset;
    [SerializeField] private Vector3 _trackingRotationOffset;

    void IMap.Map()
    {
        _rigTarget.transform.position = _vrTarget.transform.TransformPoint(_trackingPositionOffset);
        _rigTarget.transform.rotation = _vrTarget.transform.rotation * Quaternion.Euler(_trackingRotationOffset);
    }
}