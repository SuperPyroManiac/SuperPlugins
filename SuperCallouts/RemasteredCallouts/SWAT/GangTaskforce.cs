using System.Collections.Generic;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Gang Taskforce", CalloutProbability.Low)]
internal class GangTaskforce : SuperCallout
{
    // Gang members and their tracking
    private readonly List<Ped> _gangMembers = [];
    private readonly List<Blip> _gangBlips = [];
    private readonly Vector3 _raidPosition = new(113.1443f, -1926.435f, 20.8231f);

    // FIB assets
    private Vehicle _agentVehicle;
    private Ped _agentLead;
    private Ped _agentSecondary;
    private Blip _meetingBlip;

    // Menu items
    private UIMenuItem _menuStartConversation;
    private UIMenuItem _menuBeginRaid;
    private UIMenuItem _menuWaitRaid;

    // State tracking
    private bool _meetingCompleted;
    private bool _raidStarted;
    private bool _helpDisplayed;
    private bool _backupRequested;

    // Base properties
    internal override Location SpawnPoint { get; set; } = new(new Vector3(113.1443f, -1926.435f, 20.8231f));
    internal override float OnSceneDistance { get; set; } = 50f;
    internal override string CalloutName { get; set; } = "Gang Taskforce";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ FIB requesting assistance.";
        CalloutAdvisory = "FIB agents requesting local police backup for a gang raid operation.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_GANG_RAID IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~FIB Operation",
            "FIB agents are requesting local police backup for a gang raid operation. Meet with the agents first. Respond ~y~CODE-2"
        );

        // Setup meeting location and FIB agents
        SpawnPoint = new Location(new Vector3(20.23883f, -1773.036f, 28.7713f));
        SetupFibAssets();
        SetupConversationMenu();
    }

    private void SetupFibAssets()
    {
        // Create FIB vehicle at exact position from setup
        _agentVehicle = new Vehicle("POLICE3", SpawnPoint.Position, 0f)
        {
            LicensePlate = "64HTK085",
            DirtLevel = 4f,
            PrimaryColor = Color.FromArgb(255, 0, 0, 0),
            SecondaryColor = Color.FromArgb(255, 0, 0, 0),
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
        };
        _agentVehicle.Heading = 11.33f; // Converted from quaternion

        // Create FIB agents at exact positions from setup
        _agentLead = new Ped("MP_M_FIBSEC_01", new Vector3(18.11886f, -1772.186f, 29.30607f), 175.8938f);
        _agentSecondary = new Ped("MP_M_FIBSEC_01", new Vector3(18.8285f, -1773.461f, 29.31185f), 286.9934f);

        // Set up agent appearances
        _agentLead.SetVariation(0, 0, 0);
        _agentLead.SetVariation(2, 0, 0);
        _agentLead.SetVariation(3, 0, 0);
        _agentLead.SetVariation(4, 0, 0);
        _agentLead.SetVariation(10, 0, 0);

        _agentSecondary.SetVariation(0, 1, 2);
        _agentSecondary.SetVariation(2, 0, 0);
        _agentSecondary.SetVariation(3, 1, 1);
        _agentSecondary.SetVariation(4, 0, 0);
        _agentSecondary.SetVariation(10, 1, 0);

        // Make entities persistent
        _agentVehicle.IsPersistent = true;
        _agentLead.IsPersistent = true;
        _agentSecondary.IsPersistent = true;
        _agentLead.BlockPermanentEvents = true;
        _agentSecondary.BlockPermanentEvents = true;

        // Add to cleanup lists
        EntitiesToClear.Add(_agentVehicle);
        EntitiesToClear.Add(_agentLead);
        EntitiesToClear.Add(_agentSecondary);

        // Create blip and set route
        _meetingBlip = _agentVehicle.AttachBlip();
        _meetingBlip.EnableRoute(Color.Yellow);
        _meetingBlip.Color = Color.Yellow;
        BlipsToClear.Add(_meetingBlip);
    }

    private void SetupConversationMenu()
    {
        _menuStartConversation = new UIMenuItem("Speak with FIB Agents");
        _menuBeginRaid = new UIMenuItem("Begin Raid Operation");
        _menuWaitRaid = new UIMenuItem("Wait for More Backup");

        ConvoMenu.AddItem(_menuStartConversation);
        MainMenu.AddItem(_menuBeginRaid);
        MainMenu.AddItem(_menuWaitRaid);

        _menuBeginRaid.Enabled = false;
        _menuWaitRaid.Enabled = false;
    }

    internal override void CalloutRunning()
    {
        // Display help message when player is close to agents
        if (!_helpDisplayed && Player.DistanceTo(_agentLead) < 15f)
        {
            Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to interact with the FIB agents.");
            _helpDisplayed = true;
        }

        // Check if all gang members are dead after raid begins
        if (_meetingCompleted && _gangMembers.Count > 0)
        {
            if (AreAllGangMembersHandled())
            {
                Game.DisplayNotification("~g~All gang members have been neutralized. Operation successful!");
                CalloutEnd();
            }

            // End callout if player gets too far from the operation
            if (IsPlayerFarFromAllGangMembers() && _raidStarted)
            {
                Game.DisplayNotification("~r~You've left the operation area. The FIB will handle the rest.");
                CalloutEnd();
            }
        }

        //Check if player is at the raid location
        if (!_raidStarted && _meetingCompleted && Player.DistanceTo(_raidPosition) < 80f)
        {
            _raidStarted = true;
            if (_backupRequested)
            {
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
                PyroFunctions.RequestBackup(Enums.BackupType.Noose);
            }
        }
    }

    internal override void CalloutOnScene()
    {
        // When player arrives at meeting point
        if (_meetingBlip)
        {
            _meetingBlip.DisableRoute();
            _meetingBlip.Scale = 0.7f;
        }

        _agentLead.Tasks.FaceEntity(Player);
        _agentSecondary.Tasks.FaceEntity(Player);

        Questioning.Enabled = true;
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _menuStartConversation)
        {
            GameFiber.StartNew(
                delegate
                {
                    _menuStartConversation.Enabled = false;

                    // Initial conversation with FIB agents
                    Game.DisplaySubtitle("~b~FIB Agent:~s~ Officer, thanks for coming. We're planning a raid on a Ballas safehouse nearby.", 5000);
                    GameFiber.Wait(5000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ We have intel that they're storing weapons and drugs. We need local police backup for the operation.", 5000);
                    GameFiber.Wait(5000);

                    Game.DisplaySubtitle("~g~You:~s~ What's the plan?", 3000);
                    GameFiber.Wait(3000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ We'll drive to the location and breach together. These guys are armed and dangerous.", 5000);
                    GameFiber.Wait(5000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ You can choose to start now or wait for additional backup. Your call.", 5000);

                    // Enable raid options
                    _menuBeginRaid.Enabled = true;
                    _menuWaitRaid.Enabled = true;
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _menuBeginRaid)
        {
            GameFiber.StartNew(
                delegate
                {
                    // Begin raid immediately
                    _menuBeginRaid.Enabled = false;
                    _menuWaitRaid.Enabled = false;

                    Game.DisplaySubtitle("~g~You:~s~ Let's do this now. Lead the way.", 3000);
                    GameFiber.Wait(3000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ Roger that. We're headed to the target location.", 3000);

                    BeginRaidOperation();
                }
            );
        }
        else if (selItem == _menuWaitRaid)
        {
            GameFiber.StartNew(
                delegate
                {
                    // Wait for backup
                    _menuBeginRaid.Enabled = false;
                    _menuWaitRaid.Enabled = false;

                    Game.DisplaySubtitle("~g~You:~s~ Let's wait for additional backup before we move in.", 3000);
                    GameFiber.Wait(3000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ Good call. We'll call for NOOSE to come.", 3000);
                    GameFiber.Wait(3000);

                    Game.DisplaySubtitle("~b~FIB Agent:~s~ Backup is prepared. Let's move in.", 3000);

                    _backupRequested = true;
                    BeginRaidOperation();
                }
            );
        }

        base.Interactions(sender, selItem, index);
    }

    private void BeginRaidOperation()
    {
        // Update meeting status
        _meetingCompleted = true;

        // Clean up meeting blip
        if (_meetingBlip)
        {
            _meetingBlip.Delete();
        }

        SetupGangMembers();
        CreateGangBlips();

        // Move agents to raid position
        _agentLead.Tasks.EnterVehicle(_agentVehicle, -1);
        _agentSecondary.Tasks.EnterVehicle(_agentVehicle, 0);
        GameFiber.Wait(2000);

        _agentLead.Tasks.DriveToPosition(_agentVehicle, _raidPosition, 20f, VehicleDrivingFlags.Emergency, 5f);

        Game.DisplayNotification("~r~Follow the FIB agents to the raid location!");
    }

    private void SetupGangMembers()
    {
        for (int i = 0; i < 7; i++)
        {
            var gangMember = new Ped("g_m_y_ballasout_01", _raidPosition.Around(10f), 0f);
            gangMember.IsPersistent = true;
            _gangMembers.Add(gangMember);
            EntitiesToClear.Add(gangMember);
        }

        var weapons = new WeaponHash[]
        {
            WeaponHash.AssaultRifle,
            WeaponHash.Pistol,
            WeaponHash.MicroSMG,
            WeaponHash.Knife,
            WeaponHash.PumpShotgun,
            WeaponHash.Crowbar,
            WeaponHash.Pistol50,
        };

        // Set up each gang member with weapons and wanted status
        for (int i = 0; i < _gangMembers.Count; i++)
        {
            var gangMember = _gangMembers[i];

            if (weapons[i] != WeaponHash.Knife && weapons[i] != WeaponHash.Crowbar)
                gangMember.Inventory.Weapons.Add(weapons[i]).Ammo = -1;
            else
                gangMember.Inventory.Weapons.Add(weapons[i]);

            gangMember.SetWanted(true);
            gangMember.SetResistance(Enums.ResistanceAction.Attack, false, 100);
        }
    }

    private void CreateGangBlips()
    {
        var areaBlip = new Blip(_raidPosition, 50f);
        areaBlip.Color = Color.Red;
        areaBlip.Alpha = 0.5f;
        BlipsToClear.Add(areaBlip);
    }

    private bool AreAllGangMembersHandled()
    {
        foreach (var gangMember in _gangMembers)
        {
            if (gangMember && (gangMember.IsAlive || !gangMember.IsCuffed))
            {
                if (gangMember.DistanceTo(_raidPosition) > 70f)
                    gangMember.Kill();

                return false;
            }
        }
        return true;
    }

    private bool IsPlayerFarFromAllGangMembers()
    {
        if (Player.DistanceTo(_raidPosition) < 100f)
            return false;

        foreach (var gangMember in _gangMembers)
        {
            if (gangMember && Player.DistanceTo(gangMember) < 100f)
                return false;
        }
        return true;
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        foreach (var blip in _gangBlips)
        {
            if (blip)
                blip.Delete();
        }

        base.CalloutEnd(forceCleanup);
    }
}
