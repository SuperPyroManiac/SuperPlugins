using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Car Accident2", CalloutProbability.Medium)]
internal class CarAccident2 : SuperCallout
{
    private readonly UIMenuItem _callFd = new("~r~ Call Fire Department", "Calls for ambulance and firetruck.");
    private Blip _victim1Blip;
    private Blip _victim2Blip;
    private Vehicle _vehicle1;
    private Vehicle _vehicle2;
    private string _suspectName;
    private UIMenuItem _speakSuspect;
    private Ped _victim1;
    private Ped _victim2;

    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(45f, 320f)));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (2)";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports their is multiple vehicles involved.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~MVA", "Reports of a car accident, respond ~r~CODE-3");

        SpawnVehicles();
        SpawnVictims();
        SetupMenu();
        CreateBlips();
    }

    private void SpawnVehicles()
    {
        // First vehicle
        PyroFunctions.SpawnNormalCar(out _vehicle1, SpawnPoint.Position);
        _vehicle1.EngineHealth = 0;
        PyroFunctions.DamageVehicle(_vehicle1, 200, 200);
        EntitiesToClear.Add(_vehicle1);

        // Second vehicle
        PyroFunctions.SpawnNormalCar(out _vehicle2, _vehicle1.GetOffsetPosition(new Vector3(0, 7.0f, 0)));
        _vehicle2.EngineHealth = 0;
        _vehicle2.Rotation = new Rotator(0f, 0f, 180f);
        PyroFunctions.DamageVehicle(_vehicle2, 200, 200);
        _vehicle2.Metadata.searchDriver = "~r~half full hard liqueur bottle~s~, ~y~pack of lighters~s~, ~g~coke cans~s~, ~g~cigarettes~s~";
        EntitiesToClear.Add(_vehicle2);
    }

    private void SpawnVictims()
    {
        // First victim (injured)
        _victim1 = _vehicle1.CreateRandomDriver();
        _victim1.IsPersistent = true;
        _victim1.BlockPermanentEvents = true;
        _victim1.Tasks.LeaveVehicle(_vehicle1, LeaveVehicleFlags.LeaveDoorOpen);
        PyroFunctions.SetAnimation(_victim1, "move_injured_ground");
        EntitiesToClear.Add(_victim1);

        // Second victim (drunk driver)
        _victim2 = _vehicle2.CreateRandomDriver();
        _victim2.IsPersistent = true;
        _victim2.BlockPermanentEvents = true;
        _victim2.Tasks.LeaveVehicle(_vehicle2, LeaveVehicleFlags.LeaveDoorOpen);
        PyroFunctions.SetDrunkOld(_victim2, true);
        _victim2.Metadata.searchPed = "~r~crushed beer can~s~, ~g~wallet~s~";
        _victim2.Metadata.stpAlcoholDetected = true;
        _suspectName = Functions.GetPersonaForPed(_victim2).FullName;
        EntitiesToClear.Add(_victim2);
    }

    private void SetupMenu()
    {
        _speakSuspect = new UIMenuItem($"Speak with ~y~{_suspectName}");
        ConvoMenu.AddItem(_speakSuspect);

        MainMenu.RemoveItemAt(1);
        MainMenu.AddItem(_callFd);
        MainMenu.AddItem(EndCall);
        _callFd.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _callFd.Enabled = false;
    }

    private void CreateBlips()
    {
        _victim1Blip = _victim1.AttachBlip();
        _victim1Blip.Color = Color.Red;
        _victim1Blip.EnableRoute(Color.Red);
        BlipsToClear.Add(_victim1Blip);

        _victim2Blip = _victim2.AttachBlip();
        _victim2Blip.Color = Color.Red;
        BlipsToClear.Add(_victim2Blip);
    }

    internal override void CalloutRunning()
    {
        if (!_victim2)
        {
            CalloutEnd(true);
            return;
        }

        UpdateSuspectStatus();
    }

    private void UpdateSuspectStatus()
    {
        if (_victim2.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_victim1 || !_victim2)
        {
            CalloutEnd(true);
            return;
        }

        _victim1Blip?.DisableRoute();
        Questioning.Enabled = true;
        _callFd.Enabled = true;

        SetupVictimBehavior();
    }

    private void SetupVictimBehavior()
    {
        NativeFunction.Natives.xCDDC2B77CE54AC6E(_victim1, _victim2, -1, 1000); //TASK_WRITHE
        NativeFunction.Natives.x5AD23D40115353AC(_victim2, Player, -1);
        _victim1.BlockPermanentEvents = false;
        _victim2.BlockPermanentEvents = false;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callFd)
        {
            _callFd.Enabled = false;
            Game.DisplaySubtitle("~g~You~s~: Dispatch, we have an MVA. One person is seriously injured.");
            PyroFunctions.RequestBackup(Enums.BackupType.Fire);
            PyroFunctions.RequestBackup(Enums.BackupType.Medical);

            base.Interactions(sender, selItem, index);
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_victim1 || !_victim2)
        {
            CalloutEnd(true);
            return;
        }

        if (selItem == _speakSuspect)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakSuspect.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: What happened? Are you ok?", 5000);
                    NativeFunction.Natives.x5AD23D40115353AC(_victim2, Player, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: Who are you? I don't have to talk to you!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I'm a police officer, I need you to tell me what happened, someone is really hurt!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: No!", 5000);
                    _victim2.Tasks.EnterVehicle(_vehicle2, -1);
                    _victim2.BlockPermanentEvents = true;
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
