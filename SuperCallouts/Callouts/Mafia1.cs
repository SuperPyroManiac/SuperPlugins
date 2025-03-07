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
    private readonly List<Ped> _suspects = [];
    private readonly List<Ped> _agents = [];
    private readonly List<Vehicle> _vehicles = [];

    // UI Menu Items
    private readonly UIMenuItem _menuSpeakFib = new("- Speak With FIB Agent");
    private readonly UIMenuItem _menuChoiceNoose = new("- NOOSE Team");
    private readonly UIMenuItem _menuChoiceSwat = new("- Local SWAT Team");
    private readonly UIMenuItem _menuChoiceSolo = new("- Handle It Yourself");

    // Menu Controls
    private MenuPool _menuPool;
    private UIMenu _mainMenu;
    private UIMenu _conversationMenu;
    private UIMenuItem _menuQuestioning;
    private UIMenuItem _menuEndCall;

    // FIB Agents and Vehicles
    private Ped _agentLead;
    private Ped _agentSecondary;
    private Ped _agentBackup1;
    private Ped _agentBackup2;
    private Ped _agentBackup3;
    private Vehicle _agentVehicle1;
    private Vehicle _agentVehicle2;

    // Mafia members and vehicles
    private Ped _suspect1;
    private Ped _suspect2;
    private Ped _suspect3;
    private Ped _suspect4;
    private Ped _suspect5;
    private Ped _suspect6;
    private Ped _suspect7;
    private Ped _suspect8;
    private Vehicle _suspectVehicle1;
    private Vehicle _suspectVehicle2;
    private Vehicle _suspectVehicle3;
    private Vehicle _suspectVehicle4;

    // State tracking
    private Blip _targetBlip;
    private RaidChoice _raidChoice;
    private CalloutState _calloutState = CalloutState.CheckDistance;
    private bool _raidStarted;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(909.56f, 4.041f, 78.67f));
    internal override float OnSceneDistance { get; set; } = 120f;
    internal override string CalloutName { get; set; } = "Casino Raid";

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
        Mafia1Setup.BuildMafia1PreScene(out _agentLead, out _agentSecondary, out _agentBackup1, out _agentBackup2, out _agentBackup3, out _agentVehicle1, out _agentVehicle2);

        // Create blip for FIB meeting point
        var meetingBlip = new Blip(_agentLead.Position)
        {
            Color = Color.Yellow,
            Alpha = 127,
            Name = "Callout",
        };
        meetingBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(meetingBlip);

        // Add FIB agents and vehicles to tracking lists
        _agents.AddRange([_agentLead, _agentSecondary, _agentBackup1, _agentBackup2, _agentBackup3]);
        _vehicles.AddRange([_agentVehicle1, _agentVehicle2]);

        // Make all entities persistent
        foreach (var vehicle in _vehicles.Where(entity => entity))
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        foreach (var agent in _agents.Where(entity => entity))
        {
            agent.IsPersistent = true;
            agent.BlockPermanentEvents = true;
            EntitiesToClear.Add(agent);
        }
    }

    private void SetupUserInterface()
    {
        // Build UI menus
        PyroFunctions.BuildUi(out _menuPool, out _mainMenu, out _conversationMenu, out _menuQuestioning, out _menuEndCall);

        // Setup event handlers
        _mainMenu.OnItemSelect += InteractionProcess;
        _conversationMenu.OnItemSelect += ConversationProcess;

        // Initially disable questioning
        _menuQuestioning.Enabled = false;
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
                // When player reaches FIB agents
                if (Player.DistanceTo(_agentLead.Position) < 10f)
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
                    NativeFunction.Natives.x5AD23D40115353AC(_agentLead, Player, -1);
                    NativeFunction.Natives.x5AD23D40115353AC(_agentSecondary, Player, -1);

                    // Enable conversation options
                    _menuQuestioning.Enabled = true;
                    _conversationMenu.AddItem(_menuSpeakFib);

                    _calloutState = CalloutState.End;
                }
                break;

            case CalloutState.CheckDistance2:
                // When player reaches the raid location
                if (OnScene && !_raidStarted)
                {
                    StartRaid();
                }
                break;

            case CalloutState.RaidScene:
                // Processing for active raid scene
                if (!_raidStarted)
                {
                    // Dismiss FIB agents
                    foreach (var agent in _agents.Where(entity => entity?.Exists() == true))
                    {
                        agent.Dismiss();
                    }

                    // Activate bad guys
                    foreach (var suspect in _suspects.Where(entity => entity?.Exists() == true))
                    {
                        suspect.BlockPermanentEvents = false;
                    }

                    // Start the fight after a delay
                    GameFiber.StartNew(
                        delegate
                        {
                            GameFiber.Wait(5000);

                            foreach (var suspect in _suspects.Where(entity => entity?.Exists() == true))
                            {
                                suspect.Tasks.FightAgainstClosestHatedTarget(150, -1);
                            }

                            _targetBlip?.DisableRoute();
                            _calloutState = CalloutState.End;
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
        switch (_raidChoice)
        {
            case RaidChoice.Noose:
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
                break;

            case RaidChoice.Swat:
                PyroFunctions.RequestBackup(Enums.BackupType.Swat);
                PyroFunctions.RequestBackup(Enums.BackupType.Swat);
                break;

            case RaidChoice.Solo:
                PyroFunctions.RequestBackup(Enums.BackupType.Code3);
                PyroFunctions.RequestBackup(Enums.BackupType.Code3);
                break;

            default:
                Log.Error("Oops there was an error here. There was an issue detecting your choice!");
                CalloutEnd(true);
                return;
        }

        _calloutState = CalloutState.RaidScene;
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
        _menuPool?.CloseAllMenus();

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }

    #region UI Processing

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _menuEndCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            CalloutEnd();
        }
    }

    private void ConversationProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _menuSpeakFib)
        {
            GameFiber.StartNew(
                delegate
                {
                    if (_agentLead && Player.DistanceTo(_agentLead) > 4f)
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
                    _conversationMenu.AddItem(_menuChoiceNoose);
                    _conversationMenu.AddItem(_menuChoiceSwat);
                    _conversationMenu.AddItem(_menuChoiceSolo);
                    _conversationMenu.RefreshIndex();
                }
            );
        }
        else if (selItem == _menuChoiceNoose)
        {
            HandleBackupChoice(RaidChoice.Noose, "~b~Agent~s~: We will have a NOOSE team on standby until you arrive on scene.");
        }
        else if (selItem == _menuChoiceSwat)
        {
            HandleBackupChoice(RaidChoice.Swat, "~b~Agent~s~: Your departments SWAT team will standby for your arrival.");
        }
        else if (selItem == _menuChoiceSolo)
        {
            HandleBackupChoice(RaidChoice.Solo, "~b~Agent~s~: We will leave it to you then. Seems like a dangerous choice though.");
        }
    }

    private void HandleBackupChoice(RaidChoice choice, string message)
    {
        GameFiber.StartNew(
            delegate
            {
                // Close menus and disable questioning
                _menuPool.CloseAllMenus();
                _menuQuestioning.Enabled = false;

                // Display response message
                Game.DisplaySubtitle(message, 6000);

                // Replace meeting point blip with action blip
                foreach (var blip in BlipsToClear)
                    blip?.Delete();
                BlipsToClear.Clear();

                _targetBlip = new Blip(SpawnPoint.Position.Around2D(1, 2), 30) { Color = Color.Red, Alpha = 127 };
                _targetBlip.EnableRoute(Color.Red);
                BlipsToClear.Add(_targetBlip);

                // Load the raid scene
                LoadRaidScene();

                // Set choice and update state
                _raidChoice = choice;
                _calloutState = CalloutState.CheckDistance2;
            }
        );
    }

    #endregion

    #region Scene Loading

    private void LoadRaidScene()
    {
        // Create mafia members and vehicles
        Mafia1Setup.BuildScene(
            out _suspect1,
            out _suspect2,
            out _suspect3,
            out _suspect4,
            out _suspect5,
            out _suspect6,
            out _suspect7,
            out _suspect8,
            out _suspectVehicle1,
            out _suspectVehicle2,
            out _suspectVehicle3,
            out _suspectVehicle4
        );

        // Add to tracking lists
        _vehicles.AddRange([_suspectVehicle1, _suspectVehicle2, _suspectVehicle3, _suspectVehicle4]);
        _suspects.AddRange([_suspect1, _suspect2, _suspect3, _suspect4, _suspect5, _suspect6, _suspect7, _suspect8]);

        // Setup mafia members
        foreach (var suspect in _suspects.Where(entity => entity != null))
        {
            suspect.IsPersistent = true;
            suspect.Inventory.Weapons.Add(WeaponHash.AdvancedRifle);
            suspect.BlockPermanentEvents = true;
            suspect.SetWanted(true);
            EntitiesToClear.Add(suspect);
        }

        // Setup mafia vehicles
        foreach (var vehicle in _vehicles.Where(v => v != null && v != _agentVehicle1 && v != _agentVehicle2))
        {
            vehicle.IsPersistent = true;
            vehicle.Metadata.searchTrunk = "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
            EntitiesToClear.Add(vehicle);
        }
    }

    #endregion

    #region Enums

    private enum CalloutState
    {
        CheckDistance,
        CheckDistance2,
        RaidScene,
        End,
    }

    private enum RaidChoice
    {
        Noose,
        Swat,
        Solo,
    }

    #endregion
}
