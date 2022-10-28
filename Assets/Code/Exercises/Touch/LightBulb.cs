using System;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    private Action _onTouch;
    
    public void Init(Action onTouch)
    {
        _onTouch = onTouch;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<ValveControllerInput>() != null)
        {
            _onTouch?.Invoke();
        }
    }
}