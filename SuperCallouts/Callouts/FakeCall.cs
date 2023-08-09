#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Call Dropped", CalloutProbability.Medium, "911 call dropped - conduct wellness check", "LOW")]
internal class FakeCall : SuperCallout
{
    private Blip _cBlip;
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Prank Call";

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out var tempSpawnPoint, out _);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Emergency call dropped.";
        CalloutAdvisory = "Call dropped and dispatch is unable to reach caller back.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Call Dropped",
            "Caller disconnected from call quickly. Unable to reach them back. Last location recorded, respond to the last known location. ~r~CODE-2");

        _cBlip = new Blip(SpawnPoint, 30f);
        _cBlip.Color = Color.Red;
        _cBlip.Alpha /= 2;
        _cBlip.Name = "Scene";
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        Game.DisplayHelp("Investigate the area.", 5000);
        GameFiber.Wait(10000);
        Game.DisplaySubtitle("~g~You~s~: Dispatch, not seeing anyone out here.", 4000);
        GameFiber.Wait(4000);
        Functions.PlayScannerAudioUsingPosition("REPORT_RESPONSE_COPY_02", SpawnPoint);
        GameFiber.Wait(3500);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Area has been checked, appears to be a fake call.");
        CalloutEnd();
    }
}