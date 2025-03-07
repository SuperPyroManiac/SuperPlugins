using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Shots Fired", CalloutProbability.Medium)]
internal class OfficerShootout : SuperCallout
{
    private Ped _suspect1;
    private Ped _suspect2;
    private Blip _sceneBlip;
    private Ped _officer1;
    private Ped _officer2;
    private Vehicle _policeVehicle;
    private Vector3 _policeVehiclePosition;
    private Vehicle _suspectVehicle;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 50;
    internal override string CalloutName { get; set; } = "Officer Shootout";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Felony stop. Shots fired.";
        CalloutAdvisory = "Panic alert issues, shots fired.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_SHOTS_FIRED_AT_AN_OFFICER_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_02",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Officer Shot",
            "Officer reports shots fired during felony stop, panic button hit. Respond ~r~CODE-3"
        );

        SpawnVehicles();
        SpawnSuspects();
        SpawnOfficers();
        CreateBlip();
    }

    private void SpawnVehicles()
    {
        // Suspect vehicle
        PyroFunctions.SpawnNormalCar(out _suspectVehicle, SpawnPoint.Position);
        _suspectVehicle.Heading = SpawnPoint.Heading;
        _policeVehiclePosition = _suspectVehicle.GetOffsetPositionFront(-9f);
        _suspectVehicle.IsStolen = true;
        EntitiesToClear.Add(_suspectVehicle);

        // Police vehicle
        _policeVehicle = new Vehicle("POLICE", _policeVehiclePosition)
        {
            IsPersistent = true,
            Heading = SpawnPoint.Heading,
            IsSirenOn = true,
            IsSirenSilent = true,
        };
        EntitiesToClear.Add(_policeVehicle);
    }

    private void SpawnSuspects()
    {
        // First suspect
        _suspect1 = new Ped();
        _suspect1.IsPersistent = true;
        _suspect1.Health = 400;
        _suspect1.Inventory.Weapons.Add(WeaponHash.AssaultShotgun).Ammo = -1;
        _suspect1.WarpIntoVehicle(_suspectVehicle, -1);
        _suspect1.RelationshipGroup = new RelationshipGroup("BADGANG");
        PyroFunctions.SetWanted(_suspect1, true);
        _suspect1.Tasks.LeaveVehicle(_suspectVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_suspect1);

        // Second suspect
        _suspect2 = new Ped();
        _suspect2.IsPersistent = true;
        _suspect2.Health = 400;
        _suspect2.Inventory.Weapons.Add(WeaponHash.CarbineRifle).Ammo = -1;
        _suspect2.WarpIntoVehicle(_suspectVehicle, 0);
        _suspect2.RelationshipGroup = new RelationshipGroup("BADGANG");
        PyroFunctions.SetWanted(_suspect2, true);
        _suspect2.Tasks.LeaveVehicle(_suspectVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_suspect2);
    }

    private void SpawnOfficers()
    {
        // First officer
        _officer1 = new Ped("s_m_y_cop_01", SpawnPoint.Position, 0f);
        _officer1.IsPersistent = true;
        _officer1.WarpIntoVehicle(_policeVehicle, -1);
        _officer1.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _officer1.Tasks.LeaveVehicle(_policeVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_officer1);

        // Second officer
        _officer2 = new Ped("s_f_y_cop_01", SpawnPoint.Position, 0f);
        _officer2.IsPersistent = true;
        _officer2.WarpIntoVehicle(_policeVehicle, 0);
        _officer2.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _officer2.Tasks.LeaveVehicle(_policeVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        EntitiesToClear.Add(_officer2);
    }

    private void CreateBlip()
    {
        _sceneBlip = _policeVehicle.AttachBlip();
        _sceneBlip.Color = Color.Red;
        _sceneBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_sceneBlip);
    }

    internal override void CalloutOnScene()
    {
        if (!_officer1 || !_officer2 || !_suspect1 || !_suspect2)
        {
            CalloutEnd(true);
            return;
        }

        InitiateShootout();
        RequestBackup();
        _sceneBlip?.DisableRoute();
    }

    private void InitiateShootout()
    {
        _officer1.Tasks.FightAgainst(_suspect1, 60000);
        _suspect1.Tasks.FightAgainst(_officer1, 60000);
        _officer2.Tasks.FightAgainst(_suspect2, 60000);
        _suspect2.Tasks.FightAgainst(_officer2, 60000);

        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "PLAYER", Relationship.Hate);
    }

    private void RequestBackup()
    {
        Functions.PlayScannerAudioUsingPosition("REQUEST_BACKUP", SpawnPoint.Position);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
    }
}
