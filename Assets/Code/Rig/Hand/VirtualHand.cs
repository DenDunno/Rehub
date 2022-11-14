using Passer.Humanoid;

public class VirtualHand : OpenVRHand
{
    private readonly VirtualFingers _virtualFingers;
    private readonly VirtualWrist _virtualWrist;

    public VirtualHand(HandTarget virtualHand, HandTarget realHand, WristMovementType wristMovementType)
    {
        _virtualFingers = new VirtualFingers(virtualHand, realHand);
        _virtualWrist = new VirtualWrist(wristMovementType, virtualHand, realHand);
    }

    public override void Update()
    {
        if (tracker == null || !tracker.enabled || !enabled)
            return;

        if (useSkeletalInput) 
        {
            if (handSkeleton == null)
            {
                handSkeleton = OpenVRHandSkeleton.Get(handTarget.humanoid.openVR.tracker.transform, handTarget.isLeft, handTarget.showRealObjects);   
            }
            
            SetPoseForVirtualHand();

            if (openVRController != null)
            {
                UpdateInput();
                openVRController.UpdateComponent();
            }
                
        }
    }

    private void SetPoseForVirtualHand()
    {
        _virtualWrist.Update();
        _virtualFingers.Update();
    }
}