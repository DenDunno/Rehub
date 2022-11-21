
public enum BodyPart
{
    LeftArm,
    RightArm,
    LeftLeg,
    RightLeg
}

public class AmputatedBodyPart
{
    public readonly BodyPart Value;

    public AmputatedBodyPart(BodyPart value)
    {
        Value = value;
    }

    public bool IsHandAmputated => Value == BodyPart.LeftArm ||
                                   Value == BodyPart.RightArm;

    public bool IsLegAmputated => Value == BodyPart.LeftLeg ||
                                  Value == BodyPart.RightLeg;
}