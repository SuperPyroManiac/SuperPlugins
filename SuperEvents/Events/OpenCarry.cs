using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Open Carry", "Investigate the person.")]
internal class OpenCarry : AmbientEvent
{
    private readonly int _choice = new Random(DateTime.Now.Millisecond).Next(1, 4);
    private Ped? _bad;
    private string? _name;
    private Vector3 _spawnPoint;

    private float _spawnPointH;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        OnSceneDistance = 10;

        //Ped
        _bad = new Ped(_spawnPoint, _spawnPointH) { IsPersistent = true };
        EntitiesToClear.Add(_bad);
        _bad.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
        _bad.Metadata.stpAlcoholDetected = true;
        _bad.Metadata.hasGunPermit = false;
        _bad.Metadata.searchPed = "~r~assault rifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
        _bad.Tasks.Wander();
        _name = Functions.GetPersonaForPed(_bad).FullName;
    }

    protected override void OnProcess()
    {
    }

    protected override void OnScene()
    {
        if ( _bad == null )
        {
            EndEvent(true);
            return;
        }
        
        Game.DisplaySubtitle("~g~You: ~s~Hey there! I want to speak to you about that assault rifle.", 3000);
        _bad.Tasks.Clear();
        Log.Info("OpenCarry event picked scenerio #" + _choice);
        NativeFunction.Natives.x5AD23D40115353AC(_bad, Player, 2500);
        GameFiber.Wait(3000);
        LHandle pursuit;
        
        switch (_choice)
        {
            case 1:
                Game.DisplaySubtitle("~r~" + _name + ": ~s~It's.. It's my right.. I'll leave im sorry! Please leave me alone!", 3000);
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 2:
                Game.DisplaySubtitle("~r~" + _name + ": ~s~Whats it to you? You think you are tough?", 3000);
                GameFiber.Wait(3000);
                Game.DisplaySubtitle(
                    "~g~You: ~s~When you carry a large firearm like that in public, it tends to scare people.", 3000);
                GameFiber.Wait(3000);
                Game.DisplaySubtitle(
                    "~g~You: ~s~So I need to ask for your license, and for you to please sling your weapon for my safety.", 3000);
                GameFiber.Wait(3000);
                Game.DisplaySubtitle(
                    "~r~" + _name + ": ~s~Why would I do that? How about I give you a better look!", 5000);
                GameFiber.Wait(1000);
                _bad.Tasks.AimWeaponAt(Player, 5000);
                GameFiber.Wait(5000);
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
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
                break;
            default:
                EndEvent(true);
                break;
        }
    }

    protected override void OnCleanup()
    {
    }
}