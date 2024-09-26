using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Call Dropped", CalloutProbability.Medium)]
internal class FakeCall : SuperCallout
{
    private Blip _cBlip;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Prank Call";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Emergency call dropped.";
        CalloutAdvisory = "Call dropped and dispatch is unable to reach caller back.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Call Dropped",
            "Caller disconnected from call quickly. Unable to reach them back. Last location recorded, respond to the last known location. ~r~CODE-2");

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Yellow, true, false, 50f);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        Game.DisplayHelp("Investigate the area.", 5000);
        GameFiber.Wait(10000);
        Game.DisplaySubtitle("~g~You~s~: Dispatch, not seeing anyone out here.", 4000);
        GameFiber.Wait(4000);
        Functions.PlayScannerAudioUsingPosition("REPORT_RESPONSE_COPY_02", SpawnPoint.Position);
        GameFiber.Wait(3500);
        CalloutEnd();
    }
}