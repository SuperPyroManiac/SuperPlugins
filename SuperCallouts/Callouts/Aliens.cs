#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Aliens", CalloutProbability.VeryLow, "Alien sighting - possible prank", "Code 2")]
internal class Aliens : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(350f));
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Aliens";
    private Ped _alien1;
    private Ped _alien2;
    private Ped _alien3;
    private Blip _cBlip1;
    private Vehicle _cVehicle1;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of strange people.";
        CalloutAdvisory = "Caller says they're not human. Possibly a prank call.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Alien Sighting",
            "Caller claims that the subjects are aliens. Low priority, respond ~y~CODE-2");

        _cVehicle1 = new Vehicle("DUNE2", SpawnPoint);
        _cVehicle1.IsPersistent = true;
        _cVehicle1.IsEngineOn = true;
        EntitiesToClear.Add(_cVehicle1);

        _alien1 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien1.SetVariation(0, 0, 0);
        _alien1.SetVariation(3, 0, 0);
        _alien1.SetVariation(4, 0, 0);
        _alien1.SetVariation(5, 0, 0);
        _alien1.IsPersistent = true;
        EntitiesToClear.Add(_alien1);

        _alien2 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien2.SetVariation(0, 0, 0);
        _alien2.SetVariation(3, 0, 0);
        _alien2.SetVariation(4, 0, 0);
        _alien2.SetVariation(5, 0, 0);
        _alien2.IsPersistent = true;
        EntitiesToClear.Add(_alien2);

        _alien3 = new Ped("S_M_M_MOVALIEN_01", _cVehicle1.Position.Around(5f), 0f);
        _alien3.SetVariation(0, 0, 0);
        _alien3.SetVariation(3, 0, 0);
        _alien3.SetVariation(4, 0, 0);
        _alien3.SetVariation(5, 0, 0);
        _alien3.IsPersistent = true;
        EntitiesToClear.Add(_alien3);

        var searcharea = SpawnPoint.Around2D(40f, 75f);
        _cBlip1 = new Blip(searcharea, 80f) { Color = Color.Yellow, Alpha = .5f };
        _cBlip1.EnableRoute(Color.Yellow);
        BlipsToClear.Add(_cBlip1);
    }

    internal override void CalloutOnScene()
    {
        //RUN TOWARDS PED NATIVE x6A071245EB0D1882
        NativeFunction.Natives.x6A071245EB0D1882(_alien1, Game.LocalPlayer.Character, -1, 2f, 2f,
            0, 0);
        NativeFunction.Natives.x6A071245EB0D1882(_alien2, Game.LocalPlayer.Character, -1, 2f, 2f,
            0, 0);
        NativeFunction.Natives.x6A071245EB0D1882(_alien3, Game.LocalPlayer.Character, -1, 2f, 2f,
            0, 0);

        _cBlip1.DisableRoute();
        GameFiber.Wait(4000);
        _alien1.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien2.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _alien3.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        _cVehicle1.Velocity = new Vector3(0, 0, 70);
        GameFiber.Wait(500);
        Game.DisplaySubtitle("~g~Me:~s~ The hell was that? I think I need a nap..");
        CalloutEnd(true);
    }
}