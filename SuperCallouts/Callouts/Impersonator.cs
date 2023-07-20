#region
using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Police Impersonator", CalloutProbability.Medium, "Active traffic stop with an impersonator", "Code 3")]
internal class Impersonator : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Impersonator";
    private Ped _bad;
    private Blip _cBlip;
    private Vehicle _cVehicle1;
    private Vehicle _cVehicle2;
    private string _name1;
    private float _spawnPointH;
    private UIMenuItem _speakSuspect;
    private Ped _victim;

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(400, 100, out var tempSpawnPoint, out _spawnPointH);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~b~Dispatch:~s~ Officer impersonator.";
        CalloutAdvisory = "Caller says they have been stopped by someone that does not look like an officer.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
            Settings.EmergencyNumber +
            " call of someone being pulled over by a non uniformed officer. Description does not match our department for undercover cops. Respond ~r~CODE-3");
        CalloutInterfaceAPI.Functions.SendMessage(this,
            "Caller feels that they are in danger, this is a high priority call.");

        PyroFunctions.SpawnNormalCar(out _cVehicle1, SpawnPoint);
        _cVehicle1.Heading = _spawnPointH;
        EntitiesToClear.Add(_cVehicle1);

        _cVehicle2 = new Vehicle("DILETTANTE2", _cVehicle1.GetOffsetPositionFront(-9f)) { Heading = _spawnPointH, IsPersistent = true };
        _cVehicle2.Metadata.searchDriver =
            "~y~police radio scanner~s~, ~y~handcuffs~s~, ~g~parking ticket~s~, ~g~cigarettes~s~";
        EntitiesToClear.Add(_cVehicle2);

        _bad = _cVehicle2.CreateRandomDriver();
        _bad.IsPersistent = true;
        _bad.Inventory.Weapons.Add(WeaponHash.Pistol);
        _bad.Metadata.searchPed = "~r~kids plastic police badge~s~, ~r~loaded pistol~s~, ~g~wallet~s~";
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        EntitiesToClear.Add(_bad);

        _victim = _cVehicle1.CreateRandomDriver();
        _victim.IsPersistent = true;
        EntitiesToClear.Add(_victim);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);

        _cBlip = _bad.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        _victim.Tasks.CruiseWithVehicle(10f, VehicleDrivingFlags.Normal);
        var pursuit = Functions.CreatePursuit();
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Pullover",
            "Be advised, caller has been instructed to leave scene by the dispatcher.");
        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
        var rNd = new Random();
        var choices = rNd.Next(1, 4);
        switch (choices)
        {
            case 1:
                Game.DisplayHelp("Suspect is fleeing!");
                Functions.AddPedToPursuit(pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                CalloutInterfaceAPI.Functions.SendMessage(this, "Suspect is fleeing, show me in pursuit!");
                break;
            case 2:
                _bad.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(1500);
                _bad.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
                GameFiber.Wait(3000);
                _bad.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                CalloutInterfaceAPI.Functions.SendMessage(this, "Shots fired!");
                CalloutInterfaceAPI.Functions.SendMessage(this,
                    "**Dispatch** Code-33 all units respond. Station is 10-6.");
                break;
            case 3:
                GameFiber.Wait(2000);
                Questioning.Enabled = true;
                break;
            default:
                CalloutEnd(true);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~g~You~s~: What's going on? Why did you have that person stopped?", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: I'm off duty, that person was driving really dangerously.", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: Alright, even if you are off duty you can't be doing that. What department do you work with?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: I'm with a secret department in Los Santos. I can't disclose it to you.",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: If that's the case you may want to call your supervisor. Do you have any identification or a badge?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 +
                    "~s~: I'll have you fired for this officer. I'm not going to talk to you anymore.", 5000);
                CalloutInterfaceAPI.Functions.SendMessage(this, "Report taken from suspect.");
            });
        base.Conversations(sender, selItem, index);
    }
}