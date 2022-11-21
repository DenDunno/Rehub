
public class HandsInitialization : IScenarioComponent
{
    private readonly WristMovementType _wristMovementType;

    public HandsInitialization(WristMovementType wristMovementType)
    {
        _wristMovementType = wristMovementType;
    }

    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        if (scenarioConfig.AmputatedBodyPart.IsHandAmputated)
        {
            scenarioConfig.VirtualHand.openVR = new VirtualHand(scenarioConfig.VirtualHand, scenarioConfig.RealHand, _wristMovementType);
            scenarioConfig.VirtualHand.viveTracker.enabled = true;
        }
    }
}