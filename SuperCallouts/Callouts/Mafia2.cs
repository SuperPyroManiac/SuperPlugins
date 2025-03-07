using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Drug Raid", CalloutProbability.Low)]
internal class Mafia2 : SuperCallout
{
    private readonly List<Vehicle> _suspectVehicles = [];
    private readonly List<Ped> _suspects = [];
    private Blip _sceneBlip;

    // Mafia vehicles
    private Vehicle _suspectVehicle1;
    private Vehicle _suspectVehicle2;
    private Vehicle _suspectVehicle3;
    private Vehicle _suspectVehicle4;

    // Mafia members
    private Ped _suspect1,
        _suspect2,
        _suspect3,
        _suspect4,
        _suspect5;
    private Ped _suspect6,
        _suspect7,
        _suspect8,
        _suspect9,
        _suspect10;
    private Ped _suspect11,
        _suspect12,
        _suspect13,
        _suspect14,
        _suspect15;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(1543.173f, 3606.55f, 35.19303f));
    internal override float OnSceneDistance { get; set; } = 100f;
    internal override string CalloutName { get; set; } = "Drug Raid";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~FIB Report:~s~ Organized crime members spotted.";
        CalloutAdvisory = "Large group of armed Mafia members conducting drug operation.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Log.Info("Mafia2 callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~The Mafia",
            "FIB and IAA reports the Mafia have been spotted near Sandy Shores. Possible large scale drug trafficking. Investigate the scene."
        );

        // Setup player and initial guidance
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);

        // Construct the scene
        CreateMafiaScene();

        // Create tracking blip
        _sceneBlip = _suspect2.AttachBlip();
        _sceneBlip.EnableRoute(Color.Red);
        _sceneBlip.Color = Color.Red;
        BlipsToClear.Add(_sceneBlip);
    }

    private void CreateMafiaScene()
    {
        // Construct the scene using the custom scene builder
        Mafia2Setup.ConstructMafia2Scene(
            out _suspectVehicle1,
            out _suspectVehicle2,
            out _suspectVehicle3,
            out _suspectVehicle4,
            out _suspect1,
            out _suspect2,
            out _suspect3,
            out _suspect4,
            out _suspect5,
            out _suspect6,
            out _suspect7,
            out _suspect8,
            out _suspect9,
            out _suspect10,
            out _suspect11,
            out _suspect12,
            out _suspect13,
            out _suspect14,
            out _suspect15
        );

        // Add entities to tracking lists
        _suspectVehicles.AddRange([_suspectVehicle1, _suspectVehicle2, _suspectVehicle3, _suspectVehicle4]);
        _suspects.AddRange(
            [
                _suspect1,
                _suspect2,
                _suspect3,
                _suspect4,
                _suspect5,
                _suspect6,
                _suspect7,
                _suspect8,
                _suspect9,
                _suspect10,
                _suspect11,
                _suspect12,
                _suspect13,
                _suspect14,
                _suspect15,
            ]
        );

        // Setup vehicles
        foreach (var vehicle in _suspectVehicles)
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        // Setup mafia members
        foreach (var suspect in _suspects)
        {
            suspect.IsPersistent = true;
            suspect.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            suspect.SetWanted(true);
            Functions.AddPedContraband(suspect, ContrabandType.Narcotics, "Cocaine");
            EntitiesToClear.Add(suspect);
        }
    }

    internal override void CalloutRunning()
    {
        // Check if player left the scene after arriving
        if (OnScene && Game.LocalPlayer.Character.DistanceTo(SpawnPoint.Position) > 120f)
            CalloutEnd();
    }

    internal override void CalloutOnScene()
    {
        Game.DisplaySubtitle("Suspects spotted, appear to be ~r~armed~w~ and ~r~wanted~w~! Proceed with caution or wait for backup.", 5000);
        Game.DisplayNotification("~r~Dispatch:~s~ Officer on scene, mafia activity spotted. Dispatching specialized units.");
        Functions.PlayScannerAudioUsingPosition("DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", SpawnPoint.Position);

        // Request backup
        PyroFunctions.RequestBackup(Enums.BackupType.Noose);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);

        // Setup hostility
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);

        // Have one mafia member directly attack the player
        if (_suspect13?.Exists() == true)
            _suspect13.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);

        // Remove the navigation blip
        _sceneBlip?.Delete();
        BlipsToClear.Remove(_sceneBlip);
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Neutral);
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Neutral);

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }
}
