using Passer.Humanoid;

public static class ScenarioConfigFactory
{
    public static ScenarioConfig Create(HumanoidBodyParts humanoidBodyParts, BodyPart bodyPart)
    {
        AmputatedBodyPart amputatedBodyPart = new(bodyPart);
        
        (HandTarget realHand, HandTarget virtualHand) = GetRealAndVirtualHand(amputatedBodyPart, humanoidBodyParts);
        (FootTarget realFoot, FootTarget virtualFoot) = GetRealAndVirtualFoot(amputatedBodyPart, humanoidBodyParts);

        return new ScenarioConfig(amputatedBodyPart, virtualHand, realHand, virtualFoot, realFoot);
    }

    private static (HandTarget, HandTarget) GetRealAndVirtualHand(AmputatedBodyPart amputatedBodyPart, HumanoidBodyParts humanoidBodyParts)
    {
        HandTarget virtualHand = humanoidBodyParts.LeftHand;
        HandTarget realHand = humanoidBodyParts.RightHand;

        if (amputatedBodyPart.Value == BodyPart.RightArm)
        {
            Algorithms.Swap(ref virtualHand, ref realHand);
        }

        return (realHand, virtualHand);
    }

    private static (FootTarget realFoot, FootTarget virtualFoot) GetRealAndVirtualFoot(AmputatedBodyPart amputatedBodyPart, HumanoidBodyParts humanoidBodyParts)
    {
        FootTarget virtualFoot = humanoidBodyParts.LeftFoot;
        FootTarget realFoot = humanoidBodyParts.RightFoot;

        if (amputatedBodyPart.Value == BodyPart.RightLeg)
        {
            Algorithms.Swap(ref virtualFoot, ref realFoot);
        }

        return (realFoot, virtualFoot);
    }
}