using UnityEngine;

public abstract class RigHand : MonoBehaviour, IMap
{
    [SerializeField] private Fingers _fingers;
    private Fingers _vrFingers;
    
    public void Init(Fingers vrFingers)
    {
        _fingers.Init();
        vrFingers.Init();   
        _vrFingers = vrFingers;
    }
    
    public void Map()
    {
        if (_vrFingers == null)
            return;

        for (int i = 0; i < _vrFingers.Value.Count; ++i)
        {
            _fingers.Value[i].localRotation = _vrFingers.Value[i].localRotation;
        }
    }
}