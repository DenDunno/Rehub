using Sirenix.OdinInspector;
using UnityEngine;

public class TouchExercise : Exercise, IInitializable
{
    [SerializeField] [ChildGameObjectsOnly] private LightBulb _lightBulb;
    [SerializeField] private BoxCollider _boxCollider;
    
    void IInitializable.Init()
    {
        _lightBulb.Init(OnTouch);
    }

    private void OnTouch()
    {
        Vector3 randomPosition = _boxCollider.bounds.GetRandomPositionInBox();
        _lightBulb.transform.position = randomPosition;
    }
}