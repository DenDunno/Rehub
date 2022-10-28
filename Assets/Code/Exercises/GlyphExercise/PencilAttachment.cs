using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PencilAttachment : MonoBehaviour
{
    private Transform _virtualHandTransform;
    private bool _isAttached;
    private readonly float _distanceToAttach = 0.15f;
    private Rigidbody _rigidbody;

    public void Init(Transform virtualHandTransform)
    {
        _virtualHandTransform = virtualHandTransform;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isAttached)
            return;

        if (Vector3.Distance(_virtualHandTransform.transform.position, transform.position) <= _distanceToAttach)
        {
            transform.parent = _virtualHandTransform;
            _isAttached = true;
            _rigidbody.useGravity = false;
        }
    }
}