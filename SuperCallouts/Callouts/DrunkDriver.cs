using LSPD_First_Response.Mod.Callouts;

namespace SuperCallouts.Callouts;

[CalloutInfo("DrunkDriver", CalloutProbability.Medium)]
internal class DrunkDriver : Callout
{
    //TODO
    public override bool OnBeforeCalloutDisplayed()
    {
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        base.Process();
    }

    public override void End()
    {
        base.End();
    }
}