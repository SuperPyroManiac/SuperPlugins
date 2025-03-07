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
using Object = Rage.Object;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Bomb Report", CalloutProbability.Low)]
internal class Mafia4 : SuperCallout
{
    private readonly List<Ped> _peds = [];
    private readonly List<Vehicle> _vehicles = [];
    private readonly TimerBarPool _cTimer = new();

    // Mafia members
    private Ped _bad1,
        _bad2,
        _bad3,
        _bad4,
        _bad5,
        _bad6,
        _bad7;
    private Ped _doctor1,
        _doctor2;

    // Vehicles
    private Vehicle _eVehicle,
        _eVehicle2,
        _eVehicle3,
        _eVehicle4;

    // Bomb object
    private Object _bomb;

    // UI elements
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private UIMenuItem _endCall;
    private BarTimerBar _cTimerBar;

    // State tracking
    private RunState _state = RunState.CheckDistance;
    private bool _timerActive = false;
    private bool _raidStarted = false;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(288.916f, -1588.429f, 29.53253f));
    internal override float OnSceneDistance { get; set; } = 80f;
    internal override string CalloutName { get; set; } = "Bomb Report";

    private static Ped Player => Game.LocalPlayer.Character;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Organized crime members spotted.";
        CalloutAdvisory = "Organized crime members have setup a large bomb downtown, multiple armed suspects.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_SHOTS_FIRED_AT_AN_OFFICER_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_02",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Log.Info("Mafia4 callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~The Mafia",
            "A massive bomb has been spotted downtown, multiple armed suspects. Get to the scene."
        );

        // Setup player and initial guidance
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);

        // Construct the scene
        CreateBombScene();

        // Setup UI elements
        SetupUserInterface();

        // Create search blip
        var sceneBlip = _bomb.AttachBlip();
        sceneBlip.EnableRoute(Color.Red);
        sceneBlip.Color = Color.Red;
        BlipsToClear.Add(sceneBlip);
    }

    private void CreateBombScene()
    {
        // Construct the scene
        Mafia4Setup.ConstructMafia4Scene(
            out _bad1,
            out _bad2,
            out _bad3,
            out _bad4,
            out _bad5,
            out _bad6,
            out _bad7,
            out _doctor1,
            out _doctor2,
            out _eVehicle,
            out _eVehicle2,
            out _eVehicle3,
            out _eVehicle4,
            out _bomb
        );

        // Add entities to tracking lists
        _vehicles.AddRange([_eVehicle, _eVehicle2, _eVehicle3, _eVehicle4]);
        _peds.AddRange([_bad1, _bad2, _bad3, _bad4, _bad5, _bad6, _bad7, _doctor1, _doctor2]);

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

        // Setup bomb
        _bomb.IsPersistent = true;
        EntitiesToClear.Add(_bomb);
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

            if (_timerActive)
                _cTimer.Draw();

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
        PyroFunctions.RequestBackup(Enums.BackupType.Swat);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
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

                StartBombTimer();
            }
        );
    }

    private void StartBombTimer()
    {
        _cTimerBar = new BarTimerBar("Bomb") { Percentage = 1f };
        _cTimer.Add(_cTimerBar);
        _timerActive = true;

        GameFiber.StartNew(
            delegate
            {
                while (_timerActive)
                {
                    GameFiber.Wait(500);
                    _cTimerBar.Percentage -= 0.003f;

                    if (_cTimerBar.Percentage < 0.001f)
                        FailBombDefusal();

                    if (AreAllSuspectsDefeated())
                    {
                        SuccessBombDefusal();
                        break;
                    }
                }
            }
        );
    }

    private void FailBombDefusal()
    {
        _timerActive = false;

        foreach (var vehicle in _vehicles.Where(v => v?.Exists() == true))
            vehicle.Explode();

        foreach (var gangster in _peds.Where(p => p?.Exists() == true))
            gangster.Kill();

        CalloutEnd();
    }

    private void SuccessBombDefusal()
    {
        _timerActive = false;
        _cTimerBar.Label = "Disarmed";
        Game.DisplayHelp("Bomb Disarmed", 4000);
    }

    private bool AreAllSuspectsDefeated()
    {
        return _peds.All(gangster => !gangster?.Exists() == true || gangster.IsDead);
    }

    internal override void CalloutOnScene()
    {
        // This is handled by the state machine in CalloutRunning
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        // Stop the timer
        _timerActive = false;

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
