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
    private readonly List<Ped> _characters = [];
    private readonly List<Vehicle> _vehicles = [];
    private readonly TimerBarPool _timerPool = new();

    // Mafia members and civilians
    private Ped _suspect1,
        _suspect2,
        _suspect3,
        _suspect4,
        _suspect5,
        _suspect6,
        _suspect7;
    private Ped _civilian1,
        _civilian2;

    // Vehicles
    private Vehicle _vehicle1,
        _vehicle2,
        _vehicle3,
        _vehicle4;

    // Bomb object
    private Object _bombDevice;

    // UI elements
    private MenuPool _menuPool;
    private UIMenu _mainMenu;
    private UIMenuItem _menuEndCall;
    private BarTimerBar _bombTimerBar;

    // State tracking
    private CalloutState _calloutState = CalloutState.CheckDistance;
    private bool _timerActive = false;
    private bool _raidStarted = false;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(288.916f, -1588.429f, 29.53253f));
    internal override float OnSceneDistance { get; set; } = 80f;
    internal override string CalloutName { get; set; } = "Bomb Report";

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
        var sceneBlip = _bombDevice.AttachBlip();
        sceneBlip.EnableRoute(Color.Red);
        sceneBlip.Color = Color.Red;
        BlipsToClear.Add(sceneBlip);
    }

    private void CreateBombScene()
    {
        // Construct the scene
        Mafia4Setup.ConstructMafia4Scene(
            out _suspect1,
            out _suspect2,
            out _suspect3,
            out _suspect4,
            out _suspect5,
            out _suspect6,
            out _suspect7,
            out _civilian1,
            out _civilian2,
            out _vehicle1,
            out _vehicle2,
            out _vehicle3,
            out _vehicle4,
            out _bombDevice
        );

        // Add entities to tracking lists
        _vehicles.AddRange([_vehicle1, _vehicle2, _vehicle3, _vehicle4]);
        _characters.AddRange([_suspect1, _suspect2, _suspect3, _suspect4, _suspect5, _suspect6, _suspect7, _civilian1, _civilian2]);

        // Setup vehicles
        foreach (var vehicle in _vehicles)
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        // Setup mafia members
        foreach (var gangster in _characters)
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
        _bombDevice.IsPersistent = true;
        EntitiesToClear.Add(_bombDevice);
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

            if (_timerActive)
                _timerPool.Draw();

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

                foreach (var gangster in _characters.Where(p => p?.Exists() == true))
                {
                    gangster.BlockPermanentEvents = false;
                    gangster.Tasks.FightAgainstClosestHatedTarget(100, -1);
                }

                if (_suspect1?.Exists() == true)
                    _suspect1.Tasks.FightAgainst(Player);

                foreach (var blip in BlipsToClear)
                    blip?.DisableRoute();

                StartBombTimer();
            }
        );
    }

    private void StartBombTimer()
    {
        _bombTimerBar = new BarTimerBar("Bomb") { Percentage = 1f };
        _timerPool.Add(_bombTimerBar);
        _timerActive = true;

        GameFiber.StartNew(
            delegate
            {
                while (_timerActive)
                {
                    GameFiber.Wait(500);
                    _bombTimerBar.Percentage -= 0.003f;

                    if (_bombTimerBar.Percentage < 0.001f)
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

        foreach (var gangster in _characters.Where(p => p?.Exists() == true))
            gangster.Kill();

        CalloutEnd();
    }

    private void SuccessBombDefusal()
    {
        _timerActive = false;
        _bombTimerBar.Label = "Disarmed";
        Game.DisplayHelp("Bomb Disarmed", 4000);
    }

    private bool AreAllSuspectsDefeated()
    {
        return _characters.All(gangster => !gangster?.Exists() == true || gangster.IsDead);
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
