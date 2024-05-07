using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Vandalizing", CalloutProbability.Medium, "Reports of a person vandalizing property", "Code 3")]
internal class Vandalizing : SuperCallout
{
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 50;
    internal override string CalloutName { get; set; } = "Vandalizing";
    private Vehicle _cVehicle;
    private Ped _bad;
    private Blip _cBlip;
    private int _rNd = new Random(DateTime.Now.Millisecond).Next(2);
    
    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Person vandalizing a vehicle.";
        CalloutAdvisory = "Caller states a person is damaging a parked vehicle.";
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_SUSPECT_ON_THE_RUN_03 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Vandalizing",
            "A suspect has been reported damaging a vehicle. Respond ~r~CODE-3");
        CalloutInterfaceAPI.Functions.SendMessage(this, "A call came in about a person attacking a vehicle causing serious damage to it. Further details are unknown.");

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint.Position);
        PyroFunctions.DamageVehicle(_cVehicle, 200, 200);
        EntitiesToClear.Add(_cVehicle);

        _bad = new Ped(SpawnPoint.Position.Around(15f));
        _bad.WarpIntoVehicle(_cVehicle, -1);
        _bad.IsPersistent = true;
        _bad.BlockPermanentEvents = true;
        _bad.Metadata.stpDrugsDetected = true;
        _bad.Metadata.stpAlcoholDetected = true;
        PyroFunctions.SetDrunkOld(_bad, true);
        EntitiesToClear.Add(_bad);

        _cBlip = _bad.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        _cBlip.Scale = .5f;
        BlipsToClear.Add(_cBlip);

        _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
    }

    internal override void CalloutOnScene()
    {
        if (_cBlip.Exists()) _cBlip.Delete();
        _bad.BlockPermanentEvents = false;
        
        switch (_rNd)
        {
            case 0:
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 1:
                _bad.Tasks.FightAgainst(Player, -1);
                break;
            case 2:
                _bad.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                _bad.Tasks.FireWeaponAt(_cVehicle, -1, FiringPattern.BurstFirePistol);
                break;
        }
    }
}

