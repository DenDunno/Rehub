using Passer.Humanoid;
using UnityEngine;

public class VirtualTrackerLeg : ViveTrackerLeg
{
    private bool _isVirtual;
    
    public void MarkAsVirtual()
    {
        _isVirtual = true;
    }

    private void UpdateTrackedLeg()
    {
        Transform targetUpperLeg = footTarget.upperLeg.target.transform;
    }
}