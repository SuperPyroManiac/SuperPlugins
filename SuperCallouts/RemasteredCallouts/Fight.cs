using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInterface("[SC] Fight", CalloutProbability.Medium, "Reports of a fight on the street, limited details", "Code 3")]
internal class Fight : SuperCallout
{
    private Ped _victim;
    private Ped _suspect;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fight";

    internal override void CalloutPrep()
    {
        //TODO: TASK_AGITATED_ACTION_CONFRONT_RESPONSE(Ped ped, Ped ped2) // 0x19D1B791CB3670FE b877
    }

    internal override void CalloutAccepted()
    {
    }

    internal override void CalloutRunning()
    {
    }

    internal override void CalloutOnScene()
    {
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        base.CalloutEnd(forceCleanup);
    }
}