using Passer.Humanoid;

public class ScenarioConfig
{
    public readonly AmputatedBodyPart AmputatedBodyPart;
    public readonly HandTarget RealHand;
    public readonly HandTarget VirtualHand;
    public readonly FootTarget VirtualFoot;
    public readonly FootTarget RealFoot;

    public ScenarioConfig(AmputatedBodyPart amputatedBodyPart, HandTarget virtualHand, HandTarget realHand, FootTarget virtualFoot, FootTarget realFoot)
    {
        AmputatedBodyPart = amputatedBodyPart;
        RealHand = realHand;
        VirtualHand = virtualHand;
        RealFoot = realFoot;
        VirtualFoot = virtualFoot;
    }
}