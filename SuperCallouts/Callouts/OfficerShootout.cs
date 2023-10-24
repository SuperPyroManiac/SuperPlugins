#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Shots Fired", CalloutProbability.Medium, "Officers under fire - high priority", "Code 99")]
internal class OfficerShootout : SuperCallout
{
    private Ped _bad1;
    private Ped _bad2;
    private Blip _cBlip;
    private Ped _cop1;
    private Ped _cop2;
    private Vehicle _copVehicle;
    private Vector3 _cSpawnPoint;
    private Vehicle _cVehicle;
    private float _spawnPointH;
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 50;
    internal override string CalloutName { get; set; } = "Officer Shootout";

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(400, 100, out var tempSpawnPoint, out _spawnPointH);
        SpawnPoint = tempSpawnPoint;
        ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 10f);
        CalloutMessage = "~b~Dispatch:~s~ Felony stop. Shots fired.";
        CalloutAdvisory = "Panic alert issues, shots fired.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_SHOTS_FIRED_AT_AN_OFFICER_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_02",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Officer Shot",
            "Officer reports shots fired during felony stop, panic button hit. Respond ~r~CODE-3");

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint);
        _cVehicle.Heading = _spawnPointH;
        _cSpawnPoint = _cVehicle.GetOffsetPositionFront(-9f);
        _cVehicle.IsStolen = true;
        EntitiesToClear.Add(_cVehicle);

        _copVehicle = new Vehicle("POLICE", _cSpawnPoint)
        {
            IsPersistent = true, Heading = _spawnPointH, IsSirenOn = true, IsSirenSilent = true
        };
        EntitiesToClear.Add(_copVehicle);

        _bad1 = new Ped { IsPersistent = true, Health = 400 };
        _bad1.Inventory.Weapons.Add(WeaponHash.AssaultShotgun).Ammo = -1;
        _bad1.WarpIntoVehicle(_cVehicle, -1);
        _bad1.RelationshipGroup = new RelationshipGroup("BADGANG");
        PyroFunctions.SetWanted(_bad1, true);
        _bad1.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_bad1);

        _bad2 = new Ped { IsPersistent = true, Health = 400 };
        _bad2.Inventory.Weapons.Add(WeaponHash.CarbineRifle).Ammo = -1;
        _bad2.WarpIntoVehicle(_cVehicle, 0);
        _bad2.RelationshipGroup = new RelationshipGroup("BADGANG");
        PyroFunctions.SetWanted(_bad2, true);
        _bad2.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_bad2);

        _cop1 = new Ped("s_m_y_cop_01", SpawnPoint, 0f) { IsPersistent = true };
        _cop1.WarpIntoVehicle(_copVehicle, -1);
        _cop1.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _cop1.Tasks.LeaveVehicle(_copVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_cop1);

        _cop2 = new Ped("s_f_y_cop_01", SpawnPoint, 0f) { IsPersistent = true };
        _cop2.WarpIntoVehicle(_copVehicle, 0);
        _cop2.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _cop2.Tasks.LeaveVehicle(_copVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_cop2);

        _cBlip = _copVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cop1.Tasks.FightAgainst(_bad1, 60000);
        _bad1.Tasks.FightAgainst(_cop1, 60000);
        _cop2.Tasks.FightAgainst(_bad2, 60000);
        _bad2.Tasks.FightAgainst(_cop2, 60000);
        Functions.PlayScannerAudioUsingPosition("REQUEST_BACKUP", SpawnPoint);
        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "PLAYER", Relationship.Hate);
        if (PyroCommon.Main.UsingUB)
        {
            Wrapper.CallCode3();
            Wrapper.CallCode3();
        }
        else
        {
            Functions.RequestBackup(_copVehicle.Position, EBackupResponseType.Code3,
                EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_copVehicle.Position, EBackupResponseType.Code3,
                EBackupUnitType.LocalUnit);
        }

        if (_cBlip.Exists()) _cBlip.DisableRoute();
    }
}