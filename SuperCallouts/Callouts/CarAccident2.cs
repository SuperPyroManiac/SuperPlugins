#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Car Accident", CalloutProbability.Medium, "Reports of a vehicle crash, limited details", "Code 3")]
internal class CarAccident2 : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(45f, 320f));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (2)";
    private readonly UIMenuItem _callFd = new("~r~ Call Fire Department", "Calls for ambulance and firetruck.");
    private Blip _cBlip1;
    private Blip _cBlip2;
    private Vehicle _cVehicle1;
    private Vehicle _cVehicle2;
    private string _name1;
    private UIMenuItem _speakSuspect;
    private Ped _victim1;
    private Ped _victim2;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports their is multiple vehicles involved.";
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~MVA",
            "Reports of a car accident, respond ~r~CODE-3");

        PyroFunctions.SpawnNormalCar(out _cVehicle1, SpawnPoint);
        _cVehicle1.EngineHealth = 0;
        PyroFunctions.DamageVehicle(_cVehicle1, 200, 200);
        EntitiesToClear.Add(_cVehicle1);

        PyroFunctions.SpawnNormalCar(out _cVehicle2, _cVehicle1.GetOffsetPosition(new Vector3(0, 7.0f, 0)));
        _cVehicle2.EngineHealth = 0;
        _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
        PyroFunctions.DamageVehicle(_cVehicle2, 200, 200);
        _cVehicle2.Metadata.searchDriver =
            "~r~half full hard liqure bottle~s~, ~y~pack of lighters~s~, ~g~coke cans~s~, ~g~cigarettes~s~";
        EntitiesToClear.Add(_cVehicle2);

        _victim1 = _cVehicle1.CreateRandomDriver();
        _victim1.IsPersistent = true;
        _victim1.BlockPermanentEvents = true;
        _victim1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
        PyroFunctions.SetAnimation(_victim1, "move_injured_ground");
        EntitiesToClear.Add(_victim1);

        _victim2 = _cVehicle2.CreateRandomDriver();
        _victim2.IsPersistent = true;
        _victim2.BlockPermanentEvents = true;
        _victim2.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
        PyroFunctions.SetDrunk(_victim2, true);
        _victim2.Metadata.searchPed = "~r~crushed beer can~s~, ~g~wallet~s~";
        _victim2.Metadata.stpAlcoholDetected = true;
        _name1 = Functions.GetPersonaForPed(_victim2).FullName;
        EntitiesToClear.Add(_victim2);


        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
        MainMenu.RemoveItemAt(1);
        MainMenu.AddItem(_callFd);
        MainMenu.AddItem(EndCall);
        _callFd.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _callFd.Enabled = false;

        _cBlip1 = _victim1.AttachBlip();
        _cBlip1.Color = Color.Red;
        _cBlip1.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip1);
        _cBlip2 = _victim2.AttachBlip();
        _cBlip2.Color = Color.Red;
        BlipsToClear.Add(_cBlip2);
    }

    internal override void CalloutRunning()
    {
        if (_victim2.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        _cBlip1.DisableRoute();
        Questioning.Enabled = true;
        _callFd.Enabled = true;
        NativeFunction.Natives.xCDDC2B77CE54AC6E(_victim1, _victim2, -1, 1000); //TASK_WRITHE
        NativeFunction.Natives.x5AD23D40115353AC(_victim2, Game.LocalPlayer.Character, -1);
        _victim1.BlockPermanentEvents = false;
        _victim2.BlockPermanentEvents = false;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callFd)
        {
            Game.DisplaySubtitle("~g~You~s~: Dispatch, we have an MVA. One person is seriously injured.");
            CalloutInterfaceAPI.Functions.SendMessage(this,
                "**Dispatch** EMS has been notified and is on route. 11-78");
            if (PyroCommon.Main.UsingUB)
            {
                Wrapper.CallEms();
                Wrapper.CallFd();
            }
            else
            {
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Ambulance);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Firetruck);
            }
            _callFd.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                CalloutInterfaceAPI.Functions.SendMessage(this, "Speaking with subject.");
                Game.DisplaySubtitle("~g~You~s~: What happened? Are you ok?", 5000);
                NativeFunction.Natives.x5AD23D40115353AC(_victim2,
                    Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: Who are you? I don't have to talk to you!", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: I'm a police officer, I need you to tell me what happened, someone is really hurt!",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: No!", 5000);
                CalloutInterfaceAPI.Functions.SendMessage(this, "Subject refuses to speak.");
                _victim2.Tasks.EnterVehicle(_cVehicle2, -1);
                _victim2.BlockPermanentEvents = true;
            });
        base.Conversations(sender, selItem, index);
    }
}