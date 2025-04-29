using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Armed Robbery", CalloutProbability.Medium)]
internal class Robbery : SuperCallout
{
    private readonly Random _rNd = new();
    private Blip _blip1;
    private Blip _blip2;
    private Blip _blip3;
    private Vehicle _cVehicle;
    private Vehicle _cVehicle2;
    private Ped _rude1;
    private Ped _rude2;
    private Ped _victim;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(450f)));
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Armed Robbery";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Person(s) being held at gunpoint.";
        CalloutAdvisory = "Caller reports people holding someone at gunpoint.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_ROBBERY_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Possible Robbery",
            "A " + Settings.EmergencyNumber + " report claims 2 armed people are holding 1 person at gunpoint. Respond ~r~CODE-3"
        );

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint.Position);
        _cVehicle.IsPersistent = true;
        _cVehicle.EngineHealth = 0;
        PyroFunctions.DamageVehicle(_cVehicle, 200, 200);
        EntitiesToClear.Add(_cVehicle);

        PyroFunctions.SpawnNormalCar(out _cVehicle2, _cVehicle.GetOffsetPositionFront(6f));
        _cVehicle2.IsPersistent = true;
        _cVehicle2.EngineHealth = 0;
        _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
        PyroFunctions.DamageVehicle(_cVehicle2, 200, 200);
        EntitiesToClear.Add(_cVehicle2);

        _rude1 = _cVehicle.CreateRandomDriver();
        _rude1.IsPersistent = true;
        _rude1.BlockPermanentEvents = true;
        _rude1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _rude1.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_rude1);

        _rude2 = new Ped();
        _rude2.IsPersistent = true;
        _rude2.BlockPermanentEvents = true;
        _rude2.WarpIntoVehicle(_cVehicle, 0);
        _rude2.Inventory.GiveNewWeapon("WEAPON_PUMPSHOTGUN", 500, true);
        _rude2.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_rude2);

        _victim = _cVehicle2.CreateRandomDriver();
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        _victim.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
        _victim.Health = 200;
        EntitiesToClear.Add(_victim);

        _blip1 = _victim.AttachBlip();
        _blip1.EnableRoute(Color.Yellow);
        _blip1.Scale = .75f;
        _blip1.Color = Color.Yellow;
        _blip2 = _rude1.AttachBlip();
        _blip2.Scale = .75f;
        _blip2.Color = Color.Red;
        _blip3 = _rude2.AttachBlip();
        _blip3.Scale = .75f;
        _blip3.Color = Color.Red;
        BlipsToClear.Add(_blip1);
        BlipsToClear.Add(_blip2);
        BlipsToClear.Add(_blip3);
    }

    internal override void CalloutOnScene()
    {
        if (!_victim || !_rude1 || !_rude2)
        {
            CalloutEnd(true);
            return;
        }

        _blip1?.Delete();
        _blip2?.Delete();
        _blip3?.Delete();
        var pursuit = Functions.CreatePursuit();
        var choices = _rNd.Next(1, 5);
        Game.DisplaySubtitle("~r~Suspect: ~w~What are the cops doing here?!", 5000);
        switch (choices)
        {
            case 1:
                GameFiber.StartNew(
                    delegate
                    {
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                        _victim.Tasks.PutHandsUp(-1, _rude1);
                        GameFiber.Wait(2000);
                        NativeFunction.Natives.xF166E48407BAC484(_rude1, _victim, 0, 1);
                        NativeFunction.Natives.xF166E48407BAC484(_rude2, _victim, 0, 1);
                        _victim.Tasks.Cower(-1);
                        GameFiber.Wait(3000);
                        NativeFunction.Natives.x72C896464915D1B1(_rude1, Game.LocalPlayer.Character);
                        NativeFunction.Natives.xF166E48407BAC484(_rude2, Game.LocalPlayer.Character, 0, 1);
                        Functions.AddPedToPursuit(pursuit, _rude1);
                        GameFiber.Wait(10000);
                        Functions.AddPedToPursuit(pursuit, _rude2);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    }
                );
                break;
            case 2:
                GameFiber.StartNew(
                    delegate
                    {
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                        _victim.Tasks.PutHandsUp(-1, _rude1);
                        GameFiber.Wait(4000);
                        NativeFunction.Natives.x72C896464915D1B1(_rude1, Game.LocalPlayer.Character);
                        NativeFunction.Natives.x72C896464915D1B1(_rude2, Game.LocalPlayer.Character);
                        _victim.Tasks.Cower(-1);
                        Functions.AddPedToPursuit(pursuit, _rude1);
                        Functions.AddPedToPursuit(pursuit, _rude2);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    }
                );
                break;
            case 3:
                GameFiber.StartNew(
                    delegate
                    {
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                        _victim.Tasks.PutHandsUp(-1, _rude1);
                        GameFiber.Wait(4000);
                        NativeFunction.Natives.xF166E48407BAC484(_rude1, Game.LocalPlayer.Character, 0, 1);
                        NativeFunction.Natives.xF166E48407BAC484(_rude2, Game.LocalPlayer.Character, 0, 1);
                        PyroFunctions.SetWanted(_victim, true);
                        NativeFunction.Natives.x72C896464915D1B1(_victim, _rude1);
                        GameFiber.Wait(5000);
                        Functions.AddPedToPursuit(pursuit, _rude1);
                        Functions.AddPedToPursuit(pursuit, _rude2);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    }
                );
                break;
            case 4:
                GameFiber.StartNew(
                    delegate
                    {
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                        _victim.Tasks.PutHandsUp(-1, _rude1);
                        GameFiber.Wait(4000);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, Game.LocalPlayer.Character, -1, true);
                        NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, Game.LocalPlayer.Character, -1, true);
                        _victim.Tasks.Cower(-1);
                        GameFiber.Wait(2000);
                        _rude1.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                        _rude2.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                        GameFiber.Wait(4000);
                        Functions.AddPedToPursuit(pursuit, _rude1);
                        Functions.AddPedToPursuit(pursuit, _rude2);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    }
                );
                break;
            default:
                Game.DisplayNotification("An error has been detected! Ending callout early to prevent LSPDFR crash!");
                End();
                break;
        }
    }
}
