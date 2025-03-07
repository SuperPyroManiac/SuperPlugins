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
    private readonly List<Vehicle> _mafiaCars = [];
    private readonly List<Ped> _mafiaDudes = [];
    private Blip _sceneBlip;

    // Mafia vehicles
    private Vehicle _cVehicle1;
    private Vehicle _cVehicle2;
    private Vehicle _cVehicle3;
    private Vehicle _cVehicle4;

    // Mafia members
    private Ped _mafiaDude1,
        _mafiaDude2,
        _mafiaDude3,
        _mafiaDude4,
        _mafiaDude5;
    private Ped _mafiaDude6,
        _mafiaDude7,
        _mafiaDude8,
        _mafiaDude9,
        _mafiaDude10;
    private Ped _mafiaDude11,
        _mafiaDude12,
        _mafiaDude13,
        _mafiaDude14,
        _mafiaDude15;

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
        _sceneBlip = _mafiaDude2.AttachBlip();
        _sceneBlip.EnableRoute(Color.Red);
        _sceneBlip.Color = Color.Red;
        BlipsToClear.Add(_sceneBlip);
    }

    private void CreateMafiaScene()
    {
        // Construct the scene using the custom scene builder
        Mafia2Setup.ConstructMafia2Scene(
            out _cVehicle1,
            out _cVehicle2,
            out _cVehicle3,
            out _cVehicle4,
            out _mafiaDude1,
            out _mafiaDude2,
            out _mafiaDude3,
            out _mafiaDude4,
            out _mafiaDude5,
            out _mafiaDude6,
            out _mafiaDude7,
            out _mafiaDude8,
            out _mafiaDude9,
            out _mafiaDude10,
            out _mafiaDude11,
            out _mafiaDude12,
            out _mafiaDude13,
            out _mafiaDude14,
            out _mafiaDude15
        );

        // Add entities to tracking lists
        _mafiaCars.AddRange([_cVehicle1, _cVehicle2, _cVehicle3, _cVehicle4]);
        _mafiaDudes.AddRange(
            [
                _mafiaDude1,
                _mafiaDude2,
                _mafiaDude3,
                _mafiaDude4,
                _mafiaDude5,
                _mafiaDude6,
                _mafiaDude7,
                _mafiaDude8,
                _mafiaDude9,
                _mafiaDude10,
                _mafiaDude11,
                _mafiaDude12,
                _mafiaDude13,
                _mafiaDude14,
                _mafiaDude15,
            ]
        );

        // Setup vehicles
        foreach (var car in _mafiaCars)
        {
            car.IsPersistent = true;
            EntitiesToClear.Add(car);
        }

        // Setup mafia members
        foreach (var gangster in _mafiaDudes)
        {
            gangster.IsPersistent = true;
            gangster.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            gangster.SetWanted(true);
            Functions.AddPedContraband(gangster, ContrabandType.Narcotics, "Cocaine");
            EntitiesToClear.Add(gangster);
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
        if (_mafiaDude13?.Exists() == true)
            _mafiaDude13.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);

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
