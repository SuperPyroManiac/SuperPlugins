#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Open Carry", CalloutProbability.Low, "Person walking around with an assault rifle", "Code 2")]
internal class OpenCarry : SuperCallout
{
    private Ped _suspect;
    private Blip _cBlip;
    private string _name1;
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)), 0);
    internal override float OnSceneDistance { get; set; } = 20;
    internal override string CalloutName { get; set; } = "Open Carry";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a person with a firearm.";
        CalloutAdvisory =
            "Caller reports the person is walking around with a firearm out but has not caused any trouble.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Person With Gun",
            "Reports of a person walking around with an assault rifle. Respond ~y~CODE-2");

        _suspect = PyroFunctions.SpawnPed(SpawnPoint);
        _suspect.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
        _name1 = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.SetDrunk(Enums.DrunkState.ModeratelyDrunk);
        _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.None);
        PyroFunctions.AddFirearmItem("Assault Rifle", "weapon_assaultrifle", true, false, _suspect);
        PyroFunctions.AddWeaponItem("Knife", "weapon_knife", _suspect);
        PyroFunctions.AddDrugItem("Smelly White Powder", Enums.DrugType.Hydrocodone, _suspect);//TODO: Remove, this is for testing!
        PyroFunctions.AddSearchItem("Giant Horse Dildo -testing-", _suspect);//TODO: Remove, this is for testing!
        EntitiesToClear.Add(_suspect);

        _cBlip = _suspect.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
        _speakSuspect.Enabled = false;
    }

    internal override void CalloutRunning()
    {
        if (_suspect.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        Game.DisplaySubtitle("~g~You~s~: Hey, stop for a second.");
        _suspect.Tasks.ClearImmediately();
        _speakSuspect.Enabled = true;
        _suspect.Tasks.FaceEntity(Player, -1);
        _cBlip.DisableRoute();
        GameFiber.Wait(1000);
        var choices = new Random(DateTime.Now.Millisecond).Next(1, 6);
        switch (choices)
        {
            case 1:
                Game.DisplaySubtitle("~r~Suspect: ~s~I know my rights, leave me alone!", 5000);
                var pursuit = PyroFunctions.StartPursuit(_suspect);
                break;
            case 2:
                Game.DisplayNotification("Investigate the person.");
                _suspect.Inventory.Weapons.Clear();
                break;
            case 3:
                Game.DisplaySubtitle("~r~Suspect: ~s~REEEEEE", 5000);
                _suspect.Tasks.AimWeaponAt(Player, -1);
                break;
            case 4:
                Game.DisplayNotification("Investigate the person.");
                _suspect.Tasks.ClearImmediately();
                _suspect.Inventory.Weapons.Clear();
                _suspect.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.Valid);
                break;
            case 5:
                _suspect.Tasks.FireWeaponAt(Player, -1, FiringPattern.FullAutomatic);
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
                _suspect.Tasks.FaceEntity(Player, -1);
                GameFiber.Wait(5000);
                _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
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