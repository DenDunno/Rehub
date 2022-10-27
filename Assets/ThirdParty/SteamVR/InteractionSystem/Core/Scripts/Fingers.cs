using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fingers
{
    [SerializeField] private Transform _thumb0;
    [SerializeField] private Transform _thumb1;
    [SerializeField] private Transform _index0;
    [SerializeField] private Transform _index1;
    [SerializeField] private Transform _index2;
    [SerializeField] private Transform _middle0;
    [SerializeField] private Transform _middle1;
    [SerializeField] private Transform _middle2;
    [SerializeField] private Transform _ring0;
    [SerializeField] private Transform _ring1;
    [SerializeField] private Transform _ring2;
    [SerializeField] private Transform _pinky0;
    [SerializeField] private Transform _pinky1;
    [SerializeField] private Transform _pinky2;

    public List<Transform> Value;

    public void Init()
    {
        Value = new List<Transform>()
        {
            _thumb0, _thumb1,
            _index0, _index1, _index2,
            _middle0, _middle1, _middle2, 
            _ring0, _ring1, _ring2,
            _pinky0, _pinky1, _pinky2
        };
    }
}