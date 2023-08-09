#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Object = Rage.Object;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Bomb Report", CalloutProbability.Low, "A bomb has been found downtown - high priority all units",
    "Code 99", "SWAT")]
internal class Mafia4 : Callout
{
    private readonly Vector3 _callPos = new(288.916f, -1588.429f, 29.53253f);
    private readonly TimerBarPool _cTimer = new();
    private readonly List<Ped> _peds = new();
    private readonly List<Vehicle> _vehicles = new();
    private Ped _bad1;
    private Ped _bad2;
    private Ped _bad3;
    private Ped _bad4;
    private Ped _bad5;
    private Ped _bad6;
    private Ped _bad7;
    private Object _bomb;
    private Blip _cBlip;
    private BarTimerBar _cTimerBar;
    private Ped _doctor1;
    private Ped _doctor2;
    private UIMenuItem _endCall;
    private Vehicle _eVehicle;
    private Vehicle _eVehicle2;
    private Vehicle _eVehicle3;
    private Vehicle _eVehicle4;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private bool _running = true;
    private RunState _state = RunState.CheckDistance;
    private static Ped Player => Game.LocalPlayer.Character;


    public override bool OnBeforeCalloutDisplayed()
    {
        ShowCalloutAreaBlipBeforeAccepting(_callPos, 80f);
        CalloutMessage = "~b~Dispatch:~s~ Organized crime members spotted.";
        CalloutAdvisory = "Organized crime members have setup a large bomb downtown, multiple armed suspects.";
        CalloutPosition = _callPos;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_SHOTS_FIRED_AT_AN_OFFICER_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_02",
            _callPos);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Mafia4Setup.ConstructMafia4Scene(out _bad1, out _bad2, out _bad3, out _bad4, out _bad5, out _bad6, out _bad7,
            out _doctor1, out _doctor2, out _eVehicle, out _eVehicle2, out _eVehicle3, out _eVehicle4, out _bomb);
        Log.Info("Mafia4 callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~The Mafia",
            "A massive bomb has been spotted downtown, multiple armed suspects. Get to the scene.");
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
        //World
        _cBlip = _bomb.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        //Entities
        _vehicles.Add(_eVehicle);
        _vehicles.Add(_eVehicle2);
        _vehicles.Add(_eVehicle3);
        _vehicles.Add(_eVehicle4);
        _peds.Add(_bad1);
        _peds.Add(_bad2);
        _peds.Add(_bad3);
        _peds.Add(_bad4);
        _peds.Add(_bad5);
        _peds.Add(_bad6);
        _peds.Add(_bad7);
        _peds.Add(_doctor1);
        _peds.Add(_doctor2);
        foreach (var mafiaCars in _vehicles) mafiaCars.IsPersistent = true;
        foreach (var mafiaDudes in _peds)
        {
            mafiaDudes.IsPersistent = true;
            mafiaDudes.BlockPermanentEvents = true;
            mafiaDudes.Inventory.Weapons.Add(WeaponHash.AssaultRifle).Ammo = -1;
            mafiaDudes.RelationshipGroup = new RelationshipGroup("MAFIA");
            PyroFunctions.SetWanted(mafiaDudes, true);
            Functions.AddPedContraband(mafiaDudes, ContrabandType.Narcotics, "Cocaine");
        }

        _bomb.IsPersistent = true;
        CalloutInterfaceAPI.Functions.SendMessage(this, "Multiple unites en-route to the scene.");

        //UI Items
        PyroFunctions.BuildUi(out _interaction, out _mainMenu, out _, out _, out _endCall);
        _mainMenu.OnItemSelect += InteractionProcess;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            switch (_state)
            {
                case RunState.CheckDistance:
                    if (Player.DistanceTo(_callPos) < 80f)
                    {
                        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
                        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "PLAYER", Relationship.Hate);
                        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
                        CalloutInterfaceAPI.Functions.SendMessage(this,
                            "Gang members in the area have been paid off by the mob and may also be a thread. Be cautious.");
                        if (PyroCommon.Main.UsingUB)
                        {
                            Wrapper.CallSwat(false);
                            Wrapper.CallCode3();
                            Wrapper.CallCode3();
                            Wrapper.CallCode3();
                        }
                        else
                        {
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                EBackupUnitType.SwatTeam);
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                        }

                        _state = RunState.RaidScene;
                    }

                    break;
                case RunState.RaidScene:
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Wait(5000);
                        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists()))
                        {
                            mafiaDudes.BlockPermanentEvents = false;
                            mafiaDudes.Tasks.FightAgainstClosestHatedTarget(100, -1);
                        }

                        _bad1.Tasks.FightAgainst(Player);

                        _cBlip.DisableRoute();
                        TimerBarCounter();
                    });
                    _state = RunState.End;
                    break;
                case RunState.End:
                    _cTimer.Draw();
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            End();
        }

        //Keybinds
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
        _interaction.ProcessMenus();
        base.Process();
    }

    public override void End()
    {
        _running = false;
        if (_cBlip.Exists()) _cBlip.Delete();
        foreach (var mafiaCars in _vehicles.Where(mafiaCars => mafiaCars.Exists())) mafiaCars.Dismiss();
        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists())) mafiaDudes.Dismiss();
        if (_bomb.Exists()) _bomb.Delete();
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Dislike);
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Dislike);
        _interaction.CloseAllMenus();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);

        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");

        base.End();
    }

    //UI Functions
    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }

    //Timer
    private void TimerBarCounter()
    {
        _cTimerBar = new BarTimerBar("Bomb") { Percentage = 1f };
        _cTimer.Add(_cTimerBar);
        GameFiber.StartNew(delegate
        {
            while (_running)
            {
                GameFiber.Wait(500);
                _cTimerBar.Percentage -= 0.003f;
                if (_cTimerBar.Percentage < 0.001f) Failed();
                if (Safe())
                {
                    _running = false;
                    _cTimerBar.Label = "Disarmed";
                    CalloutInterfaceAPI.Functions.SendMessage(this, "All suspects are down. Bomb has been disarmed.");
                    Game.DisplayHelp("Bomb Disarmed", 4000);
                }
            }
        });
    }

    private void Failed()
    {
        _running = false;
        foreach (var mafiaCars in _vehicles.Where(mafiaCars => mafiaCars.Exists())) mafiaCars.Explode();
        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists())) mafiaDudes.Kill();
        End();
    }

    private bool Safe()
    {
        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists()))
            if (!mafiaDudes.IsDead)
                return false;
        return true;
    }

    private enum RunState
    {
        CheckDistance,
        RaidScene,
        End
    }
}