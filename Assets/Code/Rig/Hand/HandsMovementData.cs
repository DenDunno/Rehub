
public class HandsMovementData
{
    public readonly RigTarget Target;
    public readonly RigTarget Follower;
    public readonly float Offset;

    public HandsMovementData(RigTarget target, RigTarget follower, float offset)
    {
        Target = target;
        Follower = follower;
        Offset = offset;
    }
}