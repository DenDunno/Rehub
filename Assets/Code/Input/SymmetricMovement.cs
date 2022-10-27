using System;
using UnityEngine;

[Serializable]
public class SymmetricMovement : IUpdate, IScenarioComponent
{
    [SerializeField] private RigTarget _rightTarget;
    [SerializeField] private RigTarget _leftLeft;
    [SerializeField] private float _offset;
    [SerializeField] private Vector3 _rotation;
    private RigTarget _target;
    private RigTarget _follower;
    private bool _isHandAmputated;
    
    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        _isHandAmputated = scenarioConfig.IsHandAmputated;
        SetUpTargetAndFollower(scenarioConfig.AmputatedBodyPart);
        SetUpOffset(scenarioConfig);
    }

    private void SetUpTargetAndFollower(AmputatedBodyPart amputatedBodyPart)
    {
        _target = _leftLeft;
        _follower = _rightTarget;

        if (amputatedBodyPart == AmputatedBodyPart.LeftArm)
        {
            Algorithms.Swap(ref _target, ref _follower);
        }
    }

    private void SetUpOffset(ScenarioConfig scenarioConfig)
    {
        if (scenarioConfig.AmputatedBodyPart == AmputatedBodyPart.RightArm)
        {
            _offset = -_offset;
        }
    }

    public void Update()
    {
        if (_isHandAmputated)
        {
            _follower.transform.position = _target.transform.position + _target.transform.root.transform.right * _offset;
            _follower.transform.rotation = _target.transform.rotation * Quaternion.Euler(_rotation);
        }
    }
}