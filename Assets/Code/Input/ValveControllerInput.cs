using System;
using UnityEngine;
using Valve.VR;

public class ValveControllerInput : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Skeleton _skeletonAction;
    private const float _fullyCurled = 1f;
    private const float _epsilon = 0.001f;

    public event Action IndexFingerClick;

    private bool _fingerCurled;
    private bool _wasClick;
    
    private void Update()
    {
        TrackFingers();
    }

    private void TrackFingers()
    {
        _fingerCurled = _fullyCurled - _skeletonAction.indexCurl < _epsilon;
        
        if (_fingerCurled)
        {
            if (_wasClick == false)
            {
                _wasClick = true;
                IndexFingerClick?.Invoke();    
            }
        }
        else
        {
            _wasClick = false;
        }
    }
}