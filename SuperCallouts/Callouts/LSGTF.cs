using System.Collections.Generic;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using PyroCommon.UIManager;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Gang Taskforce", CalloutProbability.Low)]
internal class Lsgtf : SuperCallout
{
    private readonly List<Ped> _gangMembers = [];
    private readonly List<Blip> _gangBlips = [];
    private readonly Vector3 _raidPoint = new(113.1443f, -1926.435f, 20.8231f);

    private Vehicle _fibVehicle;
    private Ped _fibAgent1;
    private Ped _fibAgent2;
    private Blip _meetingBlip;
    private Vector3 _meetingPosition;

    private MenuPool _conversation;
    private UIMenu _mainMenu;
    private UIMenuItem _startConv;
    private UIMenuItem _beginRaid;
    private UIMenuItem _waitRaid;

    private bool _meetingCompleted;
    private bool _helpDisplayed;

    internal override Location SpawnPoint { get; set; } = new(new Vector3(113.1443f, -1926.435f, 20.8231f));
    internal override float OnSceneDistance { get; set; } = 50f;
    internal override string CalloutName { get; set; } = "Gang Taskforce";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~LSPD Report:~s~ Wanted gang members located.";
        CalloutAdvisory = "FIB agents need assistance with raid on wanted gang members.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Log.Info("LSGTF callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Meet with FIB",
            "FIB has a gang task force ready. Speak with them to conduct the raid."
        );

        // Construct the scene using the custom scene builder
        LsgtfSetup.ConstructLspdraidScene(
            out Ped bad1,
            out Ped bad2,
            out Ped bad3,
            out Ped bad4,
            out Ped bad5,
            out Ped bad6,
            out Ped bad7,
            out Ped bad8,
            out _fibVehicle,
            out _fibAgent1,
            out _fibAgent2
        );

        // Add gang members to collection
        _gangMembers.AddRange([bad1, bad2, bad3, bad4, bad5, bad6, bad7, bad8]);

        // Setup gang members
        SetupGangMembers();

        // Setup FIB agents and vehicle
        SetupFibAssets();

        // Setup conversation menu
        SetupConversationMenu();
    }

    private void SetupGangMembers()
    {
        // Configure weapons for each gang member
        WeaponHash[] weapons =
        [
            WeaponHash.APPistol,
            WeaponHash.CombatPistol,
            WeaponHash.APPistol,
            WeaponHash.Knife,
            WeaponHash.Molotov,
            WeaponHash.Crowbar,
            WeaponHash.MicroSMG,
            WeaponHash.Pistol50,
        ];

        for (int i = 0; i < _gangMembers.Count; i++)
        {
            var gangMember = _gangMembers[i];
            gangMember.IsPersistent = true;

            // Add weapon with unlimited ammo for ranged weapons
            if (weapons[i] != WeaponHash.Knife && weapons[i] != WeaponHash.Crowbar)
                gangMember.Inventory.Weapons.Add(weapons[i]).Ammo = -1;
            else
                gangMember.Inventory.Weapons.Add(weapons[i]);

            gangMember.SetWanted(true);
            EntitiesToClear.Add(gangMember);
        }
    }

    private void SetupFibAssets()
    {
        _fibVehicle.IsPersistent = true;
        _fibAgent1.IsPersistent = true;
        _fibAgent2.IsPersistent = true;
        _fibAgent1.BlockPermanentEvents = true;
        _fibAgent2.BlockPermanentEvents = true;

        EntitiesToClear.Add(_fibVehicle);
        EntitiesToClear.Add(_fibAgent1);
        EntitiesToClear.Add(_fibAgent2);

        _meetingBlip = _fibVehicle.AttachBlip();
        _meetingBlip.EnableRoute(Color.Aquamarine);
        _meetingBlip.Color = Color.Aquamarine;
        BlipsToClear.Add(_meetingBlip);

        _meetingPosition = _fibAgent1.Position;
    }

    private void SetupConversationMenu()
    {
        _conversation = new MenuPool();
        _mainMenu = new UIMenu("Meeting", "Choose an option");
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _conversation.Add(_mainMenu);

        _startConv = new UIMenuItem("What's the plan?");
        _mainMenu.AddItem(_startConv);
        _mainMenu.RefreshIndex();

        Style.ApplyStyle(_conversation, false);
        _mainMenu.OnItemSelect += HandleConversationOptions;
    }

    private void HandleConversationOptions(UIMenu menu, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == _startConv)
        {
            GameFiber.StartNew(
                delegate
                {
                    _startConv.Enabled = false;

                    // Make agents face player
                    NativeFunction.Natives.x5AD23D40115353AC(_fibAgent1, Game.LocalPlayer.Character, -1);
                    NativeFunction.Natives.x5AD23D40115353AC(_fibAgent2, Game.LocalPlayer.Character, -1);

                    // Display conversation dialogue
                    Game.DisplaySubtitle("~g~FIB: ~w~Thanks for coming officer, im sure you are aware how aggressive the gangs around here have become.", 6000);
                    GameFiber.Wait(6000);

                    Game.DisplaySubtitle("~g~FIB: ~w~Earlier in the week we had some gang members torture and murder a police officer. We tracked them to here.", 6000);
                    GameFiber.Wait(6000);

                    Game.DisplaySubtitle(
                        "~g~FIB: ~w~This entire group is known and wanted wanted for multiple murders. We have setup a large scale raid to bring them in.",
                        6000
                    );
                    GameFiber.Wait(6000);

                    Game.DisplaySubtitle("~g~FIB: ~w~You will be the officer in charge in this raid. Let us know when to begin.", 6000);

                    // Add new menu options
                    _beginRaid = new UIMenuItem("Yes, lets start.");
                    _waitRaid = new UIMenuItem("No, I need a minute.");

                    _mainMenu.AddItem(_beginRaid);
                    _mainMenu.AddItem(_waitRaid);
                }
            );
        }
        else if (selectedItem == _beginRaid)
        {
            GameFiber.StartNew(
                delegate
                {
                    _beginRaid.Enabled = false;
                    _waitRaid.Enabled = false;
                    _mainMenu.Visible = false;
                    _meetingCompleted = true;

                    Game.DisplaySubtitle("~g~FIB: ~w~We will call in the raid team. They will be awaiting your arrival.", 5000);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Preparation", "Get to the scene", "Get to the site to start the raid.");

                    // Remove meeting blip and create blips for gang members
                    _meetingBlip?.Delete();
                    CreateGangBlips();

                    // FIB agents enter vehicle and gang members start wandering
                    _fibAgent1.Tasks.EnterVehicle(_fibVehicle, -1);
                    _fibAgent2.Tasks.EnterVehicle(_fibVehicle, 0);

                    foreach (var gangMember in _gangMembers)
                    {
                        gangMember.Tasks.Wander();
                    }
                }
            );
        }
        else if (selectedItem == _waitRaid)
        {
            GameFiber.StartNew(
                delegate
                {
                    _mainMenu.Visible = false;
                    Game.DisplaySubtitle("~g~FIB: ~w~No problem, let us know when you are.", 5000);
                }
            );
        }
    }

    private void CreateGangBlips()
    {
        if (_gangMembers.Count > 0)
        {
            var firstBlip = _gangMembers[0].AttachBlip();
            firstBlip.Color = Color.Red;
            firstBlip.EnableRoute(Color.Red);
            _gangBlips.Add(firstBlip);
            BlipsToClear.Add(firstBlip);
        }

        // Create blips for remaining gang members
        for (int i = 1; i < _gangMembers.Count; i++)
        {
            var blip = _gangMembers[i].AttachBlip();
            blip.Color = Color.Red;
            _gangBlips.Add(blip);
            BlipsToClear.Add(blip);
        }
    }

    internal override void CalloutRunning()
    {
        _conversation?.ProcessMenus();

        // Display help message when close to FIB agents
        if (!_meetingCompleted && !_helpDisplayed && Game.LocalPlayer.Character.DistanceTo(_meetingPosition) < 15f)
        {
            Game.DisplayHelp($"Speak with the FIB agents. Press {Settings.Interact} When close.", 12000);
            _helpDisplayed = true;
        }

        // Toggle conversation menu
        if (Game.IsKeyDown(Settings.Interact) && !_meetingCompleted)
            _mainMenu.Visible = !_mainMenu.Visible;

        // Check if all gang members are dead or player is too far away
        if (OnScene && AreAllGangMembersDead())
            Game.DisplaySubtitle("~r~Radio: ~s~Well that was to be expected. Clear the scene or leave and we will take care of it.", 7000);

        if (OnScene && IsPlayerFarFromAllGangMembers())
            CalloutEnd();
    }

    internal override void CalloutOnScene()
    {
        // Drive FIB vehicle to raid point and activate sirens
        _fibAgent1.Tasks.DriveToPosition(_raidPoint, 10f, VehicleDrivingFlags.Emergency, 10f);
        _fibVehicle.IsSirenOn = true;
        _fibVehicle.IsSirenSilent = true;

        // Disable route on first gang member blip
        if (_gangBlips.Count > 0)
            _gangBlips[0].DisableRoute();

        // Request SWAT backup
        Functions.PlayScannerAudioUsingPosition("DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _raidPoint);
        PyroFunctions.RequestBackup(Enums.BackupType.Noose);
        PyroFunctions.RequestBackup(Enums.BackupType.Swat);

        // Dismiss FIB agents and vehicle
        _fibAgent1.Dismiss();
        _fibAgent2.Dismiss();
        _fibVehicle.Dismiss();

        // Set player relationship with gang
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);

        // Start ambush
        GameFiber.StartNew(
            delegate
            {
                Game.DisplaySubtitle("~r~Radio: ~s~They know our plan somehow! Take cover!", 7000);
                GameFiber.Wait(8000);

                if (_gangMembers.Count > 0 && _gangMembers[0])
                    _gangMembers[0].Tasks.FightAgainst(Game.LocalPlayer.Character);
            }
        );
    }

    private bool AreAllGangMembersDead()
    {
        foreach (var gangMember in _gangMembers)
        {
            if (gangMember && !gangMember.IsDead)
                return false;
        }
        return true;
    }

    private bool IsPlayerFarFromAllGangMembers()
    {
        foreach (var gangMember in _gangMembers)
        {
            if (gangMember && Game.LocalPlayer.Character.DistanceTo(gangMember.Position) <= 100f)
                return false;
        }
        return true;
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Neutral);
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }
}
