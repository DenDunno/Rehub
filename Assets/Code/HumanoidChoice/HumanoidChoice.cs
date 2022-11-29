using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum Sex
{
    Male,
    Female
}

[Serializable]
public class HumanoidChoice
{
    [AssetsOnly] [SerializeField] private VRRig _maleThin;
    [AssetsOnly] [SerializeField] private VRRig _maleMiddle;
    [AssetsOnly] [SerializeField] private VRRig _maleFat;
    [AssetsOnly] [SerializeField] private VRRig _femaleThin;
    [AssetsOnly] [SerializeField] private VRRig _femaleMiddle;
    [AssetsOnly] [SerializeField] private VRRig _femaleFat;

    public VRRig DetermineHumanoid(float mass, float height, Sex sex)
    {
        float bodyMassIndex = mass / height;

        if (sex == Sex.Male)
        {
            return GetMale(bodyMassIndex);
        }

        if (sex == Sex.Female)
        {
            return GetFemale(bodyMassIndex);
        }

        throw new Exception("Invalid sex");
    }

    private VRRig GetMale(float bodyMassIndex)
    {
        if (bodyMassIndex < 18)
            return _maleThin;

        if (bodyMassIndex is >= 18 and < 24)
            return _maleMiddle;

        return _maleFat;
    }
    
    private VRRig GetFemale(float bodyMassIndex)
    {
        if (bodyMassIndex < 19)
            return _femaleThin;

        if (bodyMassIndex is >= 19 and < 25)
            return _femaleMiddle;

        return _femaleFat;
    }
}