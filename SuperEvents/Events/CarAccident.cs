using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using PyroCommon.Utils;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Car Accident", "Investigate the scene.")]
internal class CarAccident : AmbientEvent
{
    private readonly int _choice = new Random(DateTime.Now.Millisecond).Next(0, 5);
    private Ped _ePed;
    private Ped _ePed2;
    private Vehicle _eVehicle;
    private Vehicle _eVehicle2;
    private string _name1;
    private string _name2;
    private Vector3 _spawnPoint;
    private float _spawnPointH;

    private Tasks _tasks = Tasks.CheckDistance;

    //UI Items
    private UIMenuItem _speakSuspect;
    private UIMenuItem _speakSuspect2;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        CommonUtils.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        //Vehicles
        CommonUtils.SpawnNormalCar(out _eVehicle, _spawnPoint);
        _eVehicle.Heading = _spawnPointH;
        _eVehicle.IsPersistent = true;
        CommonUtils.DamageVehicle(_eVehicle, 200, 200);
        CommonUtils.SpawnNormalCar(out _eVehicle2, _eVehicle.GetOffsetPositionFront(7f));
        _eVehicle2.Rotation = new Rotator(0f, 0f, 90f);
        _eVehicle2.IsPersistent = true;
        CommonUtils.DamageVehicle(_eVehicle2, 200, 200);
        EntitiesToClear.Add(_eVehicle);
        EntitiesToClear.Add(_eVehicle2);
        //Peds
        _ePed = _eVehicle.CreateRandomDriver();
        _ePed.IsPersistent = true;
        _ePed.BlockPermanentEvents = true;
        _ePed2 = _eVehicle2.CreateRandomDriver();
        _ePed2.IsPersistent = true;
        _ePed2.BlockPermanentEvents = true;
        _name1 = Functions.GetPersonaForPed(_ePed).FullName;
        _name2 = Functions.GetPersonaForPed(_ePed2).FullName;
        EntitiesToClear.Add(_ePed);
        EntitiesToClear.Add(_ePed2);
        //Randomize
        LogUtils.Info("Car Accident Scenario #" + _choice);
        switch (_choice)
        {
            case 0: //Peds fight
                if (_ePed.IsInAnyVehicle(false) && _eVehicle)
                    _ePed.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            case 1: //Ped Dies, other flees
                if (_ePed.IsInAnyVehicle(false) && _eVehicle)
                    _ePed.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                _ePed2.Kill();
                break;
            case 2: //Hit and run
                if (_ePed2.IsInAnyVehicle(false) && _eVehicle2)
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            case 3: //Fire + dead ped.
                _ePed.Kill();
                if (_ePed2.IsInAnyVehicle(false) && _eVehicle2)
                    _ePed2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                break;
            default:
                EndEvent(true);
                break;
        }

        //UI Items
        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
        ConvoMenu.AddItem(_speakSuspect);
        ConvoMenu.AddItem(_speakSuspect2);
        _speakSuspect.Enabled = false;
        _speakSuspect2.Enabled = false;
    }

    protected override void OnProcess()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                    {
                        Questioning.Enabled = true;
                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    if (!_ePed || !_ePed2)
                    {
                        EndEvent(true);
                        break;
                    }
                    _ePed.BlockPermanentEvents = false;
                    _ePed2.BlockPermanentEvents = false;
                    switch (_choice)
                    {
                        case 0: //Peds fight
                            _ePed.Tasks.FightAgainst(_ePed2);
                            _ePed2.Tasks.FightAgainst(_ePed);
                            break;
                        case 1: //Ped Dies, other flees
                            var pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, _ePed2);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            break;
                        case 2: //Hit and run
                            var pursuit2 = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit2, _ePed2);
                            Functions.SetPursuitIsActiveForPlayer(pursuit2, true);
                            break;
                        case 3: //Fire + dead ped.
                            _ePed2.Tasks.Cower(-1);
                            CommonUtils.FireControl(_spawnPoint.Around2D(7f), 24, true);
                            break;
                        default:
                            EndEvent(true);
                            break;
                    }

                    _speakSuspect!.Enabled = true;
                    _speakSuspect2!.Enabled = true;

                    _tasks = Tasks.End;
                    break;
                case Tasks.End:
                    break;
                default:
                    EndEvent(true);
                    break;
            }
        }
        catch (Exception e)
        {
            LogUtils.Error(e.ToString());
            EndEvent(true);
        }
    }

    protected override void OnCleanup() { }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_ePed || !_ePed2)
        {
            EndEvent(true);
            return;
        }

        if (selItem == _speakSuspect)
        {
            if (_ePed.IsDead)
            {
                _speakSuspect.Enabled = false;
                _speakSuspect.RightLabel = "~r~Dead";
                return;
            }

            var dialog1 = new List<string>
            {
                "~b~You~s~: What happened? Are you ok?",
                "~r~" + _name1 + "~s~: No I am not! They destroyed my car!",
                "~b~You~s~: How did the accident happen?",
                "~r~" + _name1 + "~s~: I was just driving then they swerved into my lane.",
                "~b~You~s~: That sounds pretty severe, do you have any pain anywhere?",
                "~r~" + _name1 + "~s~: My neck hurts but I'm more worried about my car.",
                "~b~You~s~: Well you just sit tight, I'm going to call fire to check you out.",
                "~r~" + _name1 + "~s~: Thank you.",
            };
            var dialog2 = new List<string>
            {
                "~b~You~s~: What happened? Are you injured?",
                "~r~" + _name1 + "~s~: No I am not! This idiot crashed into me!",
                "~b~You~s~: Calm down, can you explain what happened?",
                "~r~" + _name1 + "~s~: I was just driving then before I know it my car is totalled! I want him arrested!",
                "~b~You~s~: I understand you're upset but I need you to calm down, I'll go speak with them.",
                "~r~" + _name1 + "~s~: Whatever, I just want him to rot in jail.",
            };
            var dialogIndex1 = 0;
            var dialogIndex2 = 0;
            var dialogOutcome = new Random(DateTime.Now.Millisecond).Next(0, 101);
            var stillTalking = true;

            if (Player.DistanceTo(_ePed) > 5f)
            {
                Game.DisplaySubtitle("Too far to talk!");
                return;
            }

            NativeFunction.Natives.x5AD23D40115353AC(_ePed, Game.LocalPlayer.Character, -1);
            GameFiber.StartNew(
                delegate
                {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5)
                            stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                }
            );
        }

        if (selItem == _speakSuspect2)
        {
            if (_ePed2.IsDead)
            {
                _speakSuspect2.Enabled = false;
                _speakSuspect2.RightLabel = "~r~Dead";
                return;
            }

            var dialog1 = new List<string>
            {
                "~b~You~s~: Hey what's going on? Are you alright?",
                "~r~" + _name2 + "~s~: Screw you, I'm not alright!",
                "~b~You~s~: Do you need medical attention?",
                "~r~" + _name2 + "~s~: No I don't but I want that moron over there arrested!",
                "~b~You~s~: I understand you're upset but I need to find out what happened here.",
                "~r~" + _name2 + "~s~: Screw you, I'm not talking to you.",
            };
            var dialog2 = new List<string>
            {
                "~b~You~s~: Hey what's going on? Are you alright?",
                "~r~" + _name2 + "~s~: No not at all.",
                "~b~You~s~: Do you need medical attention?",
                "~r~" + _name2 + "~s~: No, I just want to be left alone.",
                "~b~You~s~: I understand you're upset but I need to find out what happened here.",
                "~r~" + _name2 + "~s~: Screw you, I'm not talking to you.",
            };
            var dialogIndex1 = 0;
            var dialogIndex2 = 0;
            var dialogOutcome = new Random(DateTime.Now.Millisecond).Next(0, 101);
            var stillTalking = true;

            if (Player.DistanceTo(_ePed2) > 5f)
            {
                Game.DisplaySubtitle("Too far to talk!");
                return;
            }

            NativeFunction.Natives.x5AD23D40115353AC(_ePed2, Game.LocalPlayer.Character, -1);
            GameFiber.StartNew(
                delegate
                {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5)
                            stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        End,
    }
}
