
public enum AmputatedBodyPart
{
    LeftArm,
    RightArm,
    LeftLeg
}

public class ScenarioConfig
{
    public readonly AmputatedBodyPart AmputatedBodyPart;
    public ValveControllerInput RealHand { get; private set; }
    public ValveControllerInput VirtualHand { get; private set; }

    public ScenarioConfig(AmputatedBodyPart amputatedBodyPart)
    {
        AmputatedBodyPart = amputatedBodyPart;
    }

    public bool IsHandAmputated => AmputatedBodyPart == AmputatedBodyPart.LeftArm ||
                                   AmputatedBodyPart == AmputatedBodyPart.RightArm;

    public void SetRealAndVirtualHands(ValveControllerInput realHand, ValveControllerInput virtualHand)
    {
        RealHand = realHand;
        VirtualHand = virtualHand;
    }
}