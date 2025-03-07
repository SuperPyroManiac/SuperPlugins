using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Casino Raid", CalloutProbability.Low)]
internal class Mafia1 : SuperCallout
{
    private readonly List<Ped> _badGuys = [];
    private readonly List<Ped> _goodGuys = [];
    private readonly List<Vehicle> _vehicles = [];

    // UI Menu Items
    private readonly UIMenuItem _speakFib = new("- Speak With FIB Agent");
    private readonly UIMenuItem _choiceNoose = new("- NOOSE Team");
    private readonly UIMenuItem _choiceSwat = new("- Local SWAT Team");
    private readonly UIMenuItem _choiceYou = new("- Handle It Yourself");

    // Menu Controls
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private UIMenu _convoMenu;
    private UIMenuItem _questioning;
    private UIMenuItem _endCall;

    // FIB Agents and Vehicles
    private Ped _fib1;
    private Ped _fib2;
    private Ped _fib3;
    private Ped _fib4;
    private Ped _fib5;
    private Vehicle _fibCar1;
    private Vehicle _fibCar2;

    // Mafia members and vehicles
    private Ped _bad1;
    private Ped _bad2;
    private Ped _bad3;
    private Ped _bad4;
    private Ped _bad5;
    private Ped _bad6;
    private Ped _bad7;
    private Ped _bad8;
    private Vehicle _badCar1;
    private Vehicle _badCar2;
    private Vehicle _badCar3;
    private Vehicle _badCar4;

    // State tracking
    private Blip _actionBlip;
    private SrChoice _choice;
    private SrState _state = SrState.CheckDistance;
    private bool _raidStarted;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(909.56f, 4.041f, 78.67f));
    internal override float OnSceneDistance { get; set; } = 120f;
    internal override string CalloutName { get; set; } = "Casino Raid";

    private static Ped Player => Game.LocalPlayer.Character;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~FIB Report:~s~ Raid on Mafia drug smuggling.";
        CalloutAdvisory = "FIB reports criminal activity at casino. Coordinate with agents for raid.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Log.Info("Mafia1 callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~The Mafia",
            "FIB reports the Mafia have been using the casino as a drug trafficking hotspot. Speak with FIB agents and plan a raid."
        );

        // Build initial scene with FIB agents
        SetupInitialScene();

        // Setup UI menus
        SetupUserInterface();
    }

    private void SetupInitialScene()
    {
        // Create FIB agents and vehicles
        Mafia1Setup.BuildMafia1PreScene(out _fib1, out _fib2, out _fib3, out _fib4, out _fib5, out _fibCar1, out _fibCar2);

        // Create blip for FIB meeting point
        var meetingBlip = new Blip(_fib1.Position)
        {
            Color = Color.Yellow,
            Alpha = 127,
            Name = "Callout",
        };
        meetingBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(meetingBlip);

        // Add FIB agents and vehicles to tracking lists
        _goodGuys.AddRange([_fib1, _fib2, _fib3, _fib4, _fib5]);
        _vehicles.AddRange([_fibCar1, _fibCar2]);

        // Make all entities persistent
        foreach (var vehicle in _vehicles.Where(entity => entity))
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        foreach (var agent in _goodGuys.Where(entity => entity))
        {
            agent.IsPersistent = true;
            agent.BlockPermanentEvents = true;
            EntitiesToClear.Add(agent);
        }
    }

    private void SetupUserInterface()
    {
        // Build UI menus
        PyroFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);

        // Setup event handlers
        _mainMenu.OnItemSelect += InteractionProcess;
        _convoMenu.OnItemSelect += ConversationProcess;

        // Initially disable questioning
        _questioning.Enabled = false;
    }

    internal override void CalloutRunning()
    {
        try
        {
            // Process the current state
            ProcessCurrentState();

            // Handle key bindings
            HandleKeyBindings();

            // Process menus
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
            case SrState.CheckDistance:
                // When player reaches FIB agents
                if (Player.DistanceTo(_fib1.Position) < 10f)
                {
                    BlipsToClear[0]?.DisableRoute();

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~y~SuperCallouts",
                        "~r~Speak With FIB",
                        "Press: " + Settings.Interact + " to speak with the FIB."
                    );

                    // Make FIB agents face player
                    NativeFunction.Natives.x5AD23D40115353AC(_fib1, Player, -1);
                    NativeFunction.Natives.x5AD23D40115353AC(_fib2, Player, -1);

                    // Enable conversation options
                    _questioning.Enabled = true;
                    _convoMenu.AddItem(_speakFib);

                    _state = SrState.End;
                }
                break;

            case SrState.CheckDistance2:
                // When player reaches the raid location
                if (OnScene && !_raidStarted)
                {
                    StartRaid();
                }
                break;

            case SrState.RaidScene:
                // Processing for active raid scene
                if (!_raidStarted)
                {
                    // Dismiss FIB agents
                    foreach (var agent in _goodGuys.Where(entity => entity?.Exists() == true))
                    {
                        agent.Dismiss();
                    }

                    // Activate bad guys
                    foreach (var badGuy in _badGuys.Where(entity => entity?.Exists() == true))
                    {
                        badGuy.BlockPermanentEvents = false;
                    }

                    // Start the fight after a delay
                    GameFiber.StartNew(
                        delegate
                        {
                            GameFiber.Wait(5000);

                            foreach (var badGuy in _badGuys.Where(entity => entity?.Exists() == true))
                            {
                                badGuy.Tasks.FightAgainstClosestHatedTarget(150, -1);
                            }

                            _actionBlip?.DisableRoute();
                            _state = SrState.End;
                        }
                    );

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

        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");

        // Request appropriate backup based on player's choice
        switch (_choice)
        {
            case SrChoice.Noose:
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
                break;

            case SrChoice.Swat:
                PyroFunctions.RequestBackup(Enums.BackupType.Swat);
                PyroFunctions.RequestBackup(Enums.BackupType.Swat);
                break;

            case SrChoice.You:
                PyroFunctions.RequestBackup(Enums.BackupType.Code3);
                PyroFunctions.RequestBackup(Enums.BackupType.Code3);
                break;

            default:
                Log.Error("Oops there was an error here. There was an issue detecting your choice!");
                CalloutEnd(true);
                return;
        }

        _state = SrState.RaidScene;
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

        // Close all open menus
        _interaction?.CloseAllMenus();

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }

    #region UI Processing

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            CalloutEnd();
        }
    }

    private void ConversationProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakFib)
        {
            GameFiber.StartNew(
                delegate
                {
                    if (_fib1 && Player.DistanceTo(_fib1) > 4f)
                    {
                        Game.DisplaySubtitle("~r~Get closer to talk!");
                        return;
                    }

                    // Display FIB conversation dialogue
                    Game.DisplaySubtitle("~b~Agent~s~: Hello sergeant, you may be aware of the current crime family in the city.", 7000);
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle("~b~Agent~s~: In the past we didn't have enough evidence to convict them but they slipped up this week.", 7000);
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle("~b~Agent~s~: We have intel on them transporting weapons and drugs using the casino as a base of operations.", 7000);
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle("~b~Agent~s~: So today you will be leading a raid on the casino. There is a few different ways we can handle this.", 7000);
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle("~b~Agent~s~: We have NOOSE available to help 'subdue' the suspects. You may also lead a SWAT team from your department.", 7000);
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle(
                        "~b~Agent~s~: Last option is we leave the situation under your control. You can bring your own backup, but this seems dangerous.",
                        7000
                    );
                    GameFiber.Wait(7000);

                    Game.DisplaySubtitle("~b~Agent~s~: Let me know what option sounds good to you.", 6000);

                    // Add decision options to menu
                    _convoMenu.AddItem(_choiceNoose);
                    _convoMenu.AddItem(_choiceSwat);
                    _convoMenu.AddItem(_choiceYou);
                    _convoMenu.RefreshIndex();
                }
            );
        }
        else if (selItem == _choiceNoose)
        {
            HandleBackupChoice(SrChoice.Noose, "~b~Agent~s~: We will have a NOOSE team on standby until you arrive on scene.");
        }
        else if (selItem == _choiceSwat)
        {
            HandleBackupChoice(SrChoice.Swat, "~b~Agent~s~: Your departments SWAT team will standby for your arrival.");
        }
        else if (selItem == _choiceYou)
        {
            HandleBackupChoice(SrChoice.You, "~b~Agent~s~: We will leave it to you then. Seems like a dangerous choice though.");
        }
    }

    private void HandleBackupChoice(SrChoice choice, string message)
    {
        GameFiber.StartNew(
            delegate
            {
                // Close menus and disable questioning
                _interaction.CloseAllMenus();
                _questioning.Enabled = false;

                // Display response message
                Game.DisplaySubtitle(message, 6000);

                // Replace meeting point blip with action blip
                foreach (var blip in BlipsToClear)
                    blip?.Delete();
                BlipsToClear.Clear();

                _actionBlip = new Blip(SpawnPoint.Position.Around2D(1, 2), 30) { Color = Color.Red, Alpha = 127 };
                _actionBlip.EnableRoute(Color.Red);
                BlipsToClear.Add(_actionBlip);

                // Load the raid scene
                LoadRaidScene();

                // Set choice and update state
                _choice = choice;
                _state = SrState.CheckDistance2;
            }
        );
    }

    #endregion

    #region Scene Loading

    private void LoadRaidScene()
    {
        // Create mafia members and vehicles
        Mafia1Setup.BuildScene(out _bad1, out _bad2, out _bad3, out _bad4, out _bad5, out _bad6, out _bad7, out _bad8, out _badCar1, out _badCar2, out _badCar3, out _badCar4);

        // Add to tracking lists
        _vehicles.AddRange([_badCar1, _badCar2, _badCar3, _badCar4]);
        _badGuys.AddRange([_bad1, _bad2, _bad3, _bad4, _bad5, _bad6, _bad7, _bad8]);

        // Setup mafia members
        foreach (var badGuy in _badGuys.Where(entity => entity != null))
        {
            badGuy.IsPersistent = true;
            badGuy.Inventory.Weapons.Add(WeaponHash.AdvancedRifle);
            badGuy.BlockPermanentEvents = true;
            badGuy.SetWanted(true);
            EntitiesToClear.Add(badGuy);
        }

        // Setup mafia vehicles
        foreach (var vehicle in _vehicles.Where(v => v != null && v != _fibCar1 && v != _fibCar2))
        {
            vehicle.IsPersistent = true;
            vehicle.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
            EntitiesToClear.Add(vehicle);
        }
    }

    #endregion

    #region Enums

    private enum SrState
    {
        CheckDistance,
        CheckDistance2,
        RaidScene,
        End,
    }

    private enum SrChoice
    {
        Noose,
        Swat,
        You,
    }

    #endregion
}
