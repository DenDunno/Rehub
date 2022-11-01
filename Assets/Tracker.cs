using UnityEngine;
using Valve.VR;

public class Tracker : MonoBehaviour
{
    private void Start()
    {
        uint index = 0;
        var error = ETrackedPropertyError.TrackedProp_Success;
        for (uint i = 0; i < 16; i++)
        {
            var result = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
            if (result.ToString().Contains("tracker"))
            {
                index = i;
                break;
            }
        }
        
        GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)index;
    }
}