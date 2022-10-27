using System;
using UnityEngine;

[Serializable]
public class HeadBodyOffset : IUpdate, IInitializable
{
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _headConstraint;
    private Vector3 _headBodyOffset;
    
    void IInitializable.Init()
    {
        _headBodyOffset = _root.position - _headConstraint.position;
    }
        
    void IUpdate.Update()
    {
        _root.position = _headConstraint.position + _headBodyOffset;
        _root.forward = Vector3.ProjectOnPlane(_headConstraint.up, Vector3.up).normalized;
    }    
}