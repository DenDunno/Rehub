using Passer.Humanoid;
using UnityEngine;

public class VirtualTrackerLeg : ViveTrackerLeg
{
    private bool _isVirtual;
    
    public void MarkAsVirtual()
    {
        _isVirtual = true;
    }

    public override void Update()
    {
        if (_isVirtual)
        {
            UpdateTrackedLeg();
        }
        else
        {
            base.Update();
        }
    }

    private void UpdateTrackedLeg()
    {
    }
}