using System;

public abstract class Exercise : MonoBehaviourWrapper
{
    public void Play()
    {
        Toggle(true, OnPlay);
    }
    
    public void Stop()
    {
        Toggle(false, OnStop);
    }

    private void Toggle(bool activate, Action callBack)
    {
        gameObject.SetActive(activate);
        callBack();
    }

    protected virtual void OnPlay() {}

    protected virtual void OnStop() {}
}