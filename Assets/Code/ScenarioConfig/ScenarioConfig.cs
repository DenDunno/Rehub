
using Passer.Humanoid;

public enum AmputatedBodyPart
{
    LeftArm,
    RightArm,
    LeftLeg
}

public class ScenarioConfig
{
    public readonly AmputatedBodyPart AmputatedBodyPart;
    public HandTarget RealHand { get; private set; }
    public HandTarget VirtualHand { get; private set; }

    public ScenarioConfig(AmputatedBodyPart amputatedBodyPart)
    {
        AmputatedBodyPart = amputatedBodyPart;
    }

    public bool IsHandAmputated => AmputatedBodyPart == AmputatedBodyPart.LeftArm ||
                                   AmputatedBodyPart == AmputatedBodyPart.RightArm;

    public void SetRealAndVirtualHands(HandTarget realHand, HandTarget virtualHand)
    {
        RealHand = realHand;
        VirtualHand = virtualHand;
    }
}