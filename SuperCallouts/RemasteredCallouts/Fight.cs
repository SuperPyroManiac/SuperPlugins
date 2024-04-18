using PyroCommon.API;
using Rage;

namespace SuperCallouts.RemasteredCallouts;

internal class Fight : SuperCallout
{
    private float _heading;
    private Ped _victim;
    private Ped _suspect;
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fight";

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out var tempSpawnPoint, out _heading);
        SpawnPoint = tempSpawnPoint;
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