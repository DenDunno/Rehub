using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayerHands : IScenarioComponent, IUpdate
{
    [SerializeField] private MirrorMovement _mirrorMovement;
    [SerializeField] private SymmetricMovement _symmetricMovement;
    [SerializeField] private FixedMovement _fixedMovement;
    [SerializeField] private HandsInitialization _handsInitialization;
    [SerializeField] private RigTarget _rightTarget;
    [SerializeField] private RigTarget _leftLeft;
    [SerializeField] private float _offset;
    private IUpdate _handsMovement;
    private bool _isHandAmputated;
    
    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        HandsMovementData data = CreateHandsMovementData(scenarioConfig.AmputatedBodyPart);

        _handsInitialization.Init(scenarioConfig);
        _symmetricMovement.Init(data);
        _mirrorMovement.Init(data);
        _fixedMovement.Init(data);
        
        _isHandAmputated = scenarioConfig.IsHandAmputated;
        _handsMovement = _mirrorMovement;
    }
    
    private HandsMovementData CreateHandsMovementData(AmputatedBodyPart amputatedBodyPart)
    {
        RigTarget target = _leftLeft;
        RigTarget follower = _rightTarget;

        if (amputatedBodyPart == AmputatedBodyPart.LeftArm)
        {
            Algorithms.Swap(ref target, ref follower);
        }
        if (amputatedBodyPart == AmputatedBodyPart.RightArm)
        {
            _offset = -_offset;
        }

        return new HandsMovementData(target, follower, _offset);
    }

    [Button]
    public void UseMirrorMovement()
    {
        _handsMovement = _mirrorMovement;
    }
    
    [Button]
    public void UseSymmetricMovement()
    {
        _handsMovement = _symmetricMovement;
    }
    
    [Button]
    public void UseFixedMovement()
    {
        _handsMovement = _fixedMovement;
    }
    
    void IUpdate.Update()
    {
        if (_isHandAmputated)
        {
            _handsMovement.Update();
        }
    }
}