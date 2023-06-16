using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Open Carry", "Investigate the person.")]
internal class OpenCarry : AmbientEvent
{
    private readonly int _choice = new Random().Next(1, 4);
    private Ped _bad;
    private string _name;
    private Vector3 _spawnPoint;

    private float _spawnPointH;

    //UI Items
    private UIMenuItem _speakSuspect;

    private Tasks _tasks = Tasks.CheckDistance;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            End(true);
            return;
        }

        //Ped
        _bad = new Ped(_spawnPoint, _spawnPointH) { IsPersistent = true };
        EntitiesToClear.Add(_bad);
        _bad.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
        _bad.Metadata.stpAlcoholDetected = true;
        _bad.Metadata.hasGunPermit = false;
        _bad.Metadata.searchPed = "~r~assault rifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
        _bad.Tasks.Wander();
        _name = Functions.GetPersonaForPed(_bad).FullName;
        //UI
        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    protected override void OnProcess()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Player.DistanceTo(_bad) < 10f)
                    {
                        Questioning.Enabled = true;
                        _tasks = Tasks.End;
                    }

                    break;
                case Tasks.OnScene:
                    Log.Info("OpenCarry event picked scenerio #" + _choice);
                    NativeFunction.Natives.x5AD23D40115353AC(_bad, Player, 2500);
                    GameFiber.Wait(3000);
                    LHandle pursuit;
                    switch (_choice)
                    {
                        case 1:
                            Game.DisplaySubtitle(
                                "~r~" + _name +
                                ": ~s~It's.. It's my right.. I'll leave im sorry! Please leave me alone!", 3000);
                            pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            _tasks = Tasks.End;
                            break;
                        case 2:
                            Game.DisplaySubtitle("~r~" + _name + ": ~s~Whats it to you? You think you are tough?",
                                3000);
                            GameFiber.Wait(3000);
                            Game.DisplaySubtitle(
                                "~g~You: ~s~When you carry a large firearm like that in public, it tends to scare people.",
                                3000);
                            GameFiber.Wait(3000);
                            Game.DisplaySubtitle(
                                "~g~You: ~s~So I need to ask for your license, and for you to please sling your weapon for my safety.",
                                3000);
                            GameFiber.Wait(3000);
                            Game.DisplaySubtitle(
                                "~r~" + _name + ": ~s~Why would I do that? How about I give you a better look!",
                                5000);
                            GameFiber.Wait(1000);
                            _bad.Tasks.AimWeaponAt(Player, 5000);
                            GameFiber.Wait(5000);
                            pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, _bad);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            _tasks = Tasks.End;
                            break;
                        case 3:
                            Game.DisplaySubtitle("~r~" + _name + ": ~s~Oh no, I am so sorry, here have it!", 3000);
                            _bad.Inventory.EquippedWeapon.Drop();
                            NativeFunction.Natives.x5AD23D40115353AC(_bad, Player, -1);
                            GameFiber.Wait(3000);
                            Game.DisplaySubtitle(
                                "~g~You: ~s~Why are you walking around with large firearm in your hands?", 3000);
                            GameFiber.Wait(3000);
                            Game.DisplaySubtitle(
                                "~r~" + _name + ": ~s~It's my friends, I want to look cool, that's it!", 3000);
                            GameFiber.Wait(3000);
                            _tasks = Tasks.End;
                            break;
                        default:
                            End(true);
                            break;
                    }

                    _tasks = Tasks.End;
                    break;
                case Tasks.End:
                    break;
                default:
                    End(true);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error( e.ToString());
            End(true);
        }
    }

    protected override void OnCleanup()
    {
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
        {
            if (_bad.IsDead)
            {
                _speakSuspect.Enabled = false;
                _speakSuspect.RightLabel = "~r~Dead";
                End(false);
                return;
            }

            Game.DisplaySubtitle("~g~You: ~s~Hey there! I want to speak to you about that assault rifle.", 3000);
            _bad.Tasks.Clear();
            _speakSuspect.Enabled = false;
            _tasks = Tasks.OnScene;
        }

        base.Conversations(sender, selItem, index);
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        End
    }
}