
public class LegsInitialization : IScenarioComponent
{
    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        if (scenarioConfig.AmputatedBodyPart.IsLegAmputated)
        {
            scenarioConfig.VirtualFoot.viveTracker.enabled = true;
            ((VirtualTrackerLeg)scenarioConfig.VirtualFoot.viveTracker).MarkAsVirtual();
        }
    }
}