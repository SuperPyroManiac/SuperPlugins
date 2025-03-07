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
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Mafia Raid", CalloutProbability.Low)]
internal class Mafia3 : SuperCallout
{
    private readonly List<Ped> _suspects = [];
    private readonly List<Vehicle> _vehicles = [];

    // Mafia members
    private Ped _suspect1,
        _suspect2,
        _suspect3,
        _suspect4,
        _suspect5,
        _suspect6;
    private Ped _suspect7,
        _suspect8,
        _suspect9,
        _suspect10,
        _suspect11,
        _suspect12;

    // Vehicles
    private Vehicle _limousine;
    private Vehicle _suv;
    private Vehicle _transportTruck1;
    private Vehicle _transportTruck2;
    private Vehicle _transportTruck3;

    // UI elements
    private MenuPool _menuPool;
    private UIMenu _mainMenu;
    private UIMenuItem _menuEndCall;

    // State tracking
    private CalloutState _calloutState = CalloutState.CheckDistance;
    private bool _raidStarted;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(949.3857f, -3129.14f, 5.900989f));
    internal override float OnSceneDistance { get; set; } = 90f;
    internal override string CalloutName { get; set; } = "Mafia Raid";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~FIB Report:~s~ Organized crime members spotted.";
        CalloutAdvisory = "FIB raid in progress at harbor. Multiple armed suspects.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Log.Info("Mafia3 callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~The Mafia",
            "FIB and IAA began a raid on a drug scene at the harbor. Suspects are heavily armed and backup is required. Get to the scene."
        );

        // Setup player and initial guidance
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);

        // Construct the scene
        CreateMafiaScene();

        // Setup UI elements
        SetupUserInterface();

        // Create search blip
        var sceneBlip = _transportTruck2.AttachBlip();
        sceneBlip.EnableRoute(Color.Red);
        sceneBlip.Color = Color.Red;
        BlipsToClear.Add(sceneBlip);
    }

    private void CreateMafiaScene()
    {
        // Construct the scene
        Mafia3Setup.ConstructMafia3Scene(
            out _limousine,
            out _suv,
            out _transportTruck1,
            out _transportTruck2,
            out _transportTruck3,
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
            out _suspect12
        );

        // Add entities to tracking lists
        _vehicles.AddRange([_limousine, _suv, _transportTruck1, _transportTruck2, _transportTruck3]);
        _suspects.AddRange([_suspect1, _suspect2, _suspect3, _suspect4, _suspect5, _suspect6, _suspect7, _suspect8, _suspect9, _suspect10, _suspect11, _suspect12]);

        // Setup vehicles
        foreach (var vehicle in _vehicles)
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        // Setup mafia members
        foreach (var suspect in _suspects)
        {
            suspect.IsPersistent = true;
            suspect.BlockPermanentEvents = true;
            suspect.Inventory.Weapons.Add(WeaponHash.AssaultRifle).Ammo = -1;
            suspect.RelationshipGroup = new RelationshipGroup("MAFIA");
            suspect.SetWanted(true);
            Functions.AddPedContraband(suspect, ContrabandType.Narcotics, "Cocaine");
            EntitiesToClear.Add(suspect);
        }

        // Add special items to trucks
        _transportTruck1.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~empty body bags~s~";
        _transportTruck2.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
        _transportTruck3.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~r~explosives~s~";
    }

    private void SetupUserInterface()
    {
        PyroFunctions.BuildUi(out _menuPool, out _mainMenu, out _, out _, out _menuEndCall);
        _mainMenu.OnItemSelect += InteractionProcess;
    }

    internal override void CalloutRunning()
    {
        try
        {
            ProcessCurrentState();
            HandleKeyBindings();
            _menuPool?.ProcessMenus();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
    }

    private void ProcessCurrentState()
    {
        switch (_calloutState)
        {
            case CalloutState.CheckDistance:
                if (OnScene && !_raidStarted)
                {
                    StartRaid();
                    _calloutState = CalloutState.RaidScene;
                }
                break;

            case CalloutState.RaidScene:
                if (!_raidStarted)
                {
                    InitiateFight();
                    _calloutState = CalloutState.End;
                    _raidStarted = true;
                }
                break;
        }
    }

    private void HandleKeyBindings()
    {
        if (Game.IsKeyDown(Settings.EndCall))
            CalloutEnd();

        if (Game.IsKeyDown(Settings.Interact))
            _mainMenu.Visible = !_mainMenu.Visible;
    }

    private void StartRaid()
    {
        // Set relationship between Mafia and law enforcement
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "PLAYER", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);

        // Request backup
        PyroFunctions.RequestBackup(Enums.BackupType.Noose);
        PyroFunctions.RequestBackup(Enums.BackupType.Noose);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
    }

    private void InitiateFight()
    {
        GameFiber.StartNew(
            delegate
            {
                GameFiber.Wait(5000);

                foreach (var suspect in _suspects.Where(p => p?.Exists() == true))
                {
                    suspect.BlockPermanentEvents = false;
                    suspect.Tasks.FightAgainstClosestHatedTarget(100, -1);
                }

                if (_suspect1?.Exists() == true)
                    _suspect1.Tasks.FightAgainst(Player);

                foreach (var blip in BlipsToClear)
                    blip?.DisableRoute();
            }
        );
    }

    internal override void CalloutOnScene()
    {
        // This is handled by the state machine in CalloutRunning
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        // Reset relationships
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Dislike);
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Dislike);
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "PLAYER", Relationship.Dislike);

        // Close all menus
        _menuPool?.CloseAllMenus();

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _menuEndCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            CalloutEnd();
        }
    }

    private enum CalloutState
    {
        CheckDistance,
        RaidScene,
        End,
    }
}
