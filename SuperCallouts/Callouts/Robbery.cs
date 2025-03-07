using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Armed Robbery", CalloutProbability.Medium)]
internal class Robbery : SuperCallout
{
    private readonly Random _random = new();
    private Blip _suspectBlip;
    private Blip _victimBlip;
    private Blip _vehicleBlip;
    private Vehicle _suspectVehicle;
    private Vehicle _victimVehicle;
    private Ped _suspect1;
    private Ped _suspect2;
    private Ped _victim;
    private string _suspectName1;
    private string _suspectName2;
    private string _victimName;
    private bool _onSecondaryScene;
    private bool _pursuitStarted;
    private LHandle _pursuit = Functions.CreatePursuit();
    private Vector3 _secondarySpawnPoint;
    private UIMenuItem _speakSuspect1;
    private UIMenuItem _speakSuspect2;
    private UIMenuItem _speakVictim;

    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(450f)));
    internal override float OnSceneDistance { get; set; } = 40;
    internal override string CalloutName { get; set; } = "Armed Robbery";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of an armed robbery.";
        CalloutAdvisory = "Caller reports a person is being robbed at gunpoint.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_ROBBERY_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Armed Robbery", "Reports of an armed robbery in progress. Respond ~r~CODE-3");

        SpawnVehicles();
        SpawnPeds();
        CreateBlips();
        SetupConversations();
        SetupInitialScenario();
    }

    private void SpawnVehicles()
    {
        // Victim's vehicle
        PyroFunctions.SpawnNormalCar(out _victimVehicle, SpawnPoint.Position);
        _victimVehicle.Heading = SpawnPoint.Heading;
        EntitiesToClear.Add(_victimVehicle);

        // Suspects' vehicle
        _secondarySpawnPoint = World.GetNextPositionOnStreet(SpawnPoint.Position.Around(450f));
        PyroFunctions.SpawnNormalCar(out _suspectVehicle, _secondarySpawnPoint);
        _suspectVehicle.Heading = SpawnPoint.Heading;
        EntitiesToClear.Add(_suspectVehicle);
    }

    private void SpawnPeds()
    {
        // Victim
        _victim = _victimVehicle.CreateRandomDriver();
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        _victimName = Functions.GetPersonaForPed(_victim).FullName;
        EntitiesToClear.Add(_victim);

        // First suspect
        _suspect1 = new Ped(_victim.GetOffsetPositionFront(2f));
        _suspect1.IsPersistent = true;
        _suspect1.BlockPermanentEvents = true;
        _suspect1.Inventory.Weapons.Add(WeaponHash.Pistol);
        _suspect1.Inventory.EquippedWeapon = WeaponHash.Pistol;
        _suspectName1 = Functions.GetPersonaForPed(_suspect1).FullName;
        EntitiesToClear.Add(_suspect1);

        // Second suspect
        _suspect2 = new Ped(_suspect1.GetOffsetPositionRight(1f));
        _suspect2.IsPersistent = true;
        _suspect2.BlockPermanentEvents = true;
        _suspect2.Inventory.Weapons.Add(WeaponHash.Pistol);
        _suspectName2 = Functions.GetPersonaForPed(_suspect2).FullName;
        EntitiesToClear.Add(_suspect2);
    }

    private void CreateBlips()
    {
        _victimBlip = _victim.AttachBlip();
        _victimBlip.Color = Color.Red;
        _victimBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_victimBlip);
    }

    private void SetupConversations()
    {
        _speakSuspect1 = new UIMenuItem($"Speak with ~y~{_suspectName1}");
        _speakSuspect2 = new UIMenuItem($"Speak with ~y~{_suspectName2}");
        _speakVictim = new UIMenuItem($"Speak with ~b~{_victimName}");

        ConvoMenu.AddItem(_speakSuspect1);
        ConvoMenu.AddItem(_speakSuspect2);
        ConvoMenu.AddItem(_speakVictim);
    }

    private void SetupInitialScenario()
    {
        _victim.Tasks.PutHandsUp(-1, _suspect1);
        _suspect1.Tasks.AimWeaponAt(_victim, -1);
    }

    internal override void CalloutRunning()
    {
        if (!_suspect1 || !_suspect2 || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        UpdateSuspectStatus();
        CheckSecondaryScene();
    }

    private void UpdateSuspectStatus()
    {
        if (_suspect1.IsDead)
        {
            _speakSuspect1.Enabled = false;
            _speakSuspect1.RightLabel = "~r~Dead";
        }

        if (_suspect2.IsDead)
        {
            _speakSuspect2.Enabled = false;
            _speakSuspect2.RightLabel = "~r~Dead";
        }
    }

    private void CheckSecondaryScene()
    {
        if (OnScene && !_onSecondaryScene && !_pursuitStarted && Player.DistanceTo(_suspect1) < 20f)
        {
            _pursuitStarted = true;
            InitiatePursuit();
        }

        if (_pursuitStarted && !_onSecondaryScene && Player.DistanceTo(_secondarySpawnPoint) < 40f)
        {
            _onSecondaryScene = true;
            HandleSecondaryScene();
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect1 || !_suspect2 || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        _victimBlip?.DisableRoute();
        Questioning.Enabled = true;

        // Determine scenario based on random chance
        if (_random.Next(100) < 50)
        {
            // Suspects notice police and flee
            Game.DisplaySubtitle("~r~Suspects have spotted you and are fleeing!", 5000);
            InitiatePursuit();
        }
        else
        {
            // Robbery continues, player must intervene
            Game.DisplayHelp("Intervene in the robbery!", 5000);
        }
    }

    private void InitiatePursuit()
    {
        _suspect1.BlockPermanentEvents = false;
        _suspect2.BlockPermanentEvents = false;

        _suspect1.Tasks.EnterVehicle(_suspectVehicle, -1);
        _suspect2.Tasks.EnterVehicle(_suspectVehicle, 0);

        GameFiber.Wait(3000);

        _pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(_pursuit, _suspect1);
        Functions.AddPedToPursuit(_pursuit, _suspect2);
        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);

        if (_suspectBlip == null)
        {
            _suspectBlip = _suspect1.AttachBlip();
            _suspectBlip.Color = Color.Red;
            BlipsToClear.Add(_suspectBlip);
        }

        if (_vehicleBlip == null)
        {
            _vehicleBlip = _suspectVehicle.AttachBlip();
            _vehicleBlip.Color = Color.Red;
            BlipsToClear.Add(_vehicleBlip);
        }
    }

    private void HandleSecondaryScene()
    {
        Game.DisplayNotification("~r~Suspects are attempting to escape in their vehicle!");

        if (_victimBlip != null)
        {
            _victimBlip.Delete();
            BlipsToClear.Remove(_victimBlip);
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_suspect1 || !_suspect2 || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        if (selItem == _speakSuspect1)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakSuspect1.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: Put the weapon down and your hands up!", 5000);
                    _suspect1.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName1}~s~: I'm not going back to jail!", 5000);

                    // 50% chance suspect will attack
                    if (_random.Next(100) < 50)
                    {
                        GameFiber.Wait(1000);
                        _suspect1.Tasks.FightAgainst(Player);
                    }
                    else
                    {
                        GameFiber.Wait(1000);
                        InitiatePursuit();
                    }
                }
            );
        }

        if (selItem == _speakSuspect2)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakSuspect2.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: Don't move! Put your hands where I can see them!", 5000);
                    _suspect2.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName2}~s~: This wasn't my idea, it was all his plan!", 5000);

                    // 50% chance suspect will surrender
                    if (_random.Next(100) < 50)
                    {
                        GameFiber.Wait(1000);
                        _suspect2.Tasks.PutHandsUp(-1, Player);
                    }
                    else
                    {
                        GameFiber.Wait(1000);
                        InitiatePursuit();
                    }
                }
            );
        }

        if (selItem == _speakVictim)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakVictim.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: Are you okay? What happened?", 5000);
                    _victim.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~b~{_victimName}~s~: They came out of nowhere and demanded my money and car! They had guns!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Did you recognize either of them?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~b~{_victimName}~s~: No, I've never seen them before. Please catch them!", 5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
