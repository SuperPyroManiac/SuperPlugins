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
    private readonly List<Ped> _peds = [];
    private readonly List<Vehicle> _vehicles = [];

    // Mafia members
    private Ped _bad1,
        _bad2,
        _bad3,
        _bad4,
        _bad5,
        _bad6;
    private Ped _bad7,
        _bad8,
        _bad9,
        _bad10,
        _bad11,
        _bad12;

    // Vehicles
    private Vehicle _defender;
    private Vehicle _limo;
    private Vehicle _truck1;
    private Vehicle _truck2;
    private Vehicle _truck3;

    // UI elements
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private UIMenuItem _endCall;

    // State tracking
    private RunState _state = RunState.CheckDistance;
    private bool _raidStarted;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(949.3857f, -3129.14f, 5.900989f));
    internal override float OnSceneDistance { get; set; } = 90f;
    internal override string CalloutName { get; set; } = "Mafia Raid";

    private static Ped Player => Game.LocalPlayer.Character;

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
        var sceneBlip = _truck2.AttachBlip();
        sceneBlip.EnableRoute(Color.Red);
        sceneBlip.Color = Color.Red;
        BlipsToClear.Add(sceneBlip);
    }

    private void CreateMafiaScene()
    {
        // Construct the scene
        Mafia3Setup.ConstructMafia3Scene(
            out _limo,
            out _defender,
            out _truck1,
            out _truck2,
            out _truck3,
            out _bad1,
            out _bad2,
            out _bad3,
            out _bad4,
            out _bad5,
            out _bad6,
            out _bad7,
            out _bad8,
            out _bad9,
            out _bad10,
            out _bad11,
            out _bad12
        );

        // Add entities to tracking lists
        _vehicles.AddRange([_limo, _defender, _truck1, _truck2, _truck3]);
        _peds.AddRange([_bad1, _bad2, _bad3, _bad4, _bad5, _bad6, _bad7, _bad8, _bad9, _bad10, _bad11, _bad12]);

        // Setup vehicles
        foreach (var vehicle in _vehicles)
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        // Setup mafia members
        foreach (var gangster in _peds)
        {
            gangster.IsPersistent = true;
            gangster.BlockPermanentEvents = true;
            gangster.Inventory.Weapons.Add(WeaponHash.AssaultRifle).Ammo = -1;
            gangster.RelationshipGroup = new RelationshipGroup("MAFIA");
            gangster.SetWanted(true);
            Functions.AddPedContraband(gangster, ContrabandType.Narcotics, "Cocaine");
            EntitiesToClear.Add(gangster);
        }

        // Add special items to trucks
        _truck1.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~empty body bags~s~";
        _truck2.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
        _truck3.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~r~explosives~s~";
    }

    private void SetupUserInterface()
    {
        PyroFunctions.BuildUi(out _interaction, out _mainMenu, out _, out _, out _endCall);
        _mainMenu.OnItemSelect += InteractionProcess;
    }

    internal override void CalloutRunning()
    {
        try
        {
            ProcessCurrentState();
            HandleKeyBindings();
            _interaction?.ProcessMenus();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }
    }

    private void ProcessCurrentState()
    {
        switch (_state)
        {
            case RunState.CheckDistance:
                if (OnScene && !_raidStarted)
                {
                    StartRaid();
                    _state = RunState.RaidScene;
                }
                break;

            case RunState.RaidScene:
                if (!_raidStarted)
                {
                    InitiateFight();
                    _state = RunState.End;
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

                foreach (var gangster in _peds.Where(p => p?.Exists() == true))
                {
                    gangster.BlockPermanentEvents = false;
                    gangster.Tasks.FightAgainstClosestHatedTarget(100, -1);
                }

                if (_bad1?.Exists() == true)
                    _bad1.Tasks.FightAgainst(Player);

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
        _interaction?.CloseAllMenus();

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            CalloutEnd();
        }
    }

    private enum RunState
    {
        CheckDistance,
        RaidScene,
        End,
    }
}
