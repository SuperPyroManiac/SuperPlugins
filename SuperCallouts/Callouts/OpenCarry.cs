#region
using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Open Carry", CalloutProbability.Low, "Person walking around with an assault rifle", "Code 2")]
internal class OpenCarry : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(350f));
    internal override float OnSceneDistance { get; set; } = 20;
    internal override string CalloutName { get; set; } = "Open Carry";
    private Ped _bad1;
    private Blip _cBlip;
    private string _name1;
    private UIMenuItem _speakSuspect;

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
        CalloutAdvisory =
            "Caller reports the person is walking around with a firearm out but has not caused any trouble.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Person With Gun",
            "Reports of a person walking around with an assault rifle. Respond ~y~CODE-2");

        _bad1 = new Ped(SpawnPoint) { IsPersistent = true };
        _bad1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
        _bad1.Tasks.Wander();
        _name1 = Functions.GetPersonaForPed(_bad1).FullName;
        PyroFunctions.SetDrunk(_bad1, true);
        _bad1.Metadata.stpAlcoholDetected = true;
        _bad1.Metadata.hasGunPermit = false;
        _bad1.Metadata.searchPed = "~r~assaultrifle~s~, ~y~pocket knife~s~, ~g~wallet~s~";
        EntitiesToClear.Add(_bad1);

        _cBlip = _bad1.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
        _speakSuspect.Enabled = false;
    }

    internal override void CalloutRunning()
    {
        if (_bad1.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        Game.DisplaySubtitle("~g~You~s~: Hey, stop for a second.");
        _bad1.Tasks.ClearImmediately();
        _speakSuspect.Enabled = true;
        NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
        GameFiber.Wait(1000);
        var pursuit = Functions.CreatePursuit();
        _cBlip.DisableRoute();
        var choices = new Random().Next(1, 6);
        switch (choices)
        {
            case 1:
                Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                Functions.AddPedToPursuit(pursuit, _bad1);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 2:
                Game.DisplayNotification("Investigate the person.");
                _bad1.Tasks.ClearImmediately();
                _bad1.Inventory.Weapons.Clear();
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                break;
            case 3:
                Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                _bad1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                break;
            case 4:
                Game.DisplayNotification("Investigate the person.");
                _bad1.Tasks.ClearImmediately();
                _bad1.Inventory.Weapons.Clear();
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                _bad1.Metadata.hasGunPermit = true;
                break;
            case 5:
                _bad1.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.FullAutomatic);
                break;
            default:
                Game.DisplayNotification(
                    "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                CalloutEnd(true);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                Game.DisplaySubtitle(
                    "~g~You~s~: I'm with the police. What is the reason for carrying your weapon out?", 5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: It's my right officer. Nobody can tell me I can't have my gun.''", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: Alright, I understand your rights and with the proper license you can open carry, but you cannot carry your weapon in your hands like that.",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't see why not!", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: It's the law, as well as it scares people to see someone walking around with a rifle in their hands. There's no reason to. Do you have a  for it?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: Check for yourself.", 5000);
            });
        base.Conversations(sender, selItem, index);
    }
}