using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.API.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Kidnapping", CalloutProbability.Medium)]
internal class Kidnapping : SuperCallout
{
    private readonly Random _rNd = new();
    private Ped _bad1;
    private Blip _cBlip1;
    private Vehicle _cVehicle;
    private string _name1;
    private string _name2;
    private UIMenuItem _speakSuspect;
    private UIMenuItem _speakSuspect2;
    private Ped _victim1;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 25f;
    internal override string CalloutName { get; set; } = "Kidnapping";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Person(s) from amber alert spotted.";
        CalloutAdvisory =
            "Caller says people in the back of a vehicle match the description of a missing person(s) report.";
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch",
            "~r~Possible Missing Person Found",
            "A person reported missing last week has been recognized. Possible kidnapping. Respond ~r~CODE-3");

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint.Position);
        EntitiesToClear.Add(_cVehicle);

        _bad1 = _cVehicle.CreateRandomDriver();
        _bad1.IsPersistent = true;
        _bad1.BlockPermanentEvents = true;
        _name1 = Functions.GetPersonaForPed(_bad1).FullName;
        _bad1.Inventory.Weapons.Add(WeaponHash.Pistol);
        _bad1.Metadata.stpDrugsDetected = true;
        _bad1.Metadata.searchPed =
            "~r~pistol~s~, ~r~handcuffs~s~, ~y~hunting knife~s~, ~g~candy bar~s~, ~g~loose change~s~";
        _bad1.Metadata.hasGunPermit = true;
        _bad1.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);
        EntitiesToClear.Add(_bad1);

        _victim1 = new Ped();
        _victim1.WarpIntoVehicle(_cVehicle, 0);
        _victim1.IsPersistent = true;
        _victim1.BlockPermanentEvents = true;
        _name2 = Functions.GetPersonaForPed(_victim1).FullName;
        _victim1.Metadata.searchPed = "~r~fake ID~s~";
        EntitiesToClear.Add(_victim1);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
        ConvoMenu.AddItem(_speakSuspect);
        ConvoMenu.AddItem(_speakSuspect2);

        _cBlip1 = _bad1.AttachBlip();
        _cBlip1.EnableRoute(Color.Red);
        _cBlip1.Color = Color.Red;
        _cBlip1.Scale = .5f;
        BlipsToClear.Add(_cBlip1);
    }

    internal override void CalloutRunning()
    {
        if (_bad1.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }

        if (_victim1.IsDead)
        {
            _speakSuspect2.Enabled = false;
            _speakSuspect2.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        _cBlip1.Delete();
        _bad1.BlockPermanentEvents = false;
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _bad1);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
        Game.DisplayHelp("~r~Suspect is evading!");
        var choices = _rNd.Next(1, 6);
        switch (choices)
        {
            case 1:
                _victim1.Kill();
                break;
            case 2:
                _victim1.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut).WaitForCompletion();
                _victim1.Tasks.Cower(-1);
                break;
            default:
                Log.Info("Default scenario loaded.");
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        try
        {
            if (selItem == _speakSuspect)
                GameFiber.StartNew(delegate
                {
                    _speakSuspect.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                    NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't know, why do you think?'", 5000);
                });
            if (selItem == _speakSuspect2)
                GameFiber.StartNew(delegate
                {
                    _speakSuspect2.Enabled = false;
                    Game.DisplaySubtitle(
                        "~g~You~s~: Don't worry, i'm a police officer. I'm here to help and you're safe now. Can you tell me what happened?",
                        5000);
                    NativeFunction.Natives.x5AD23D40115353AC(_victim1, Game.LocalPlayer.Character, -1);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~b~" + _name2 +
                        "~s~: My real name is Bailey, they took me forever ago. I don't even know how long! I've been stuck in a cage in a dark room. Please help me where is my family.",
                        5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: Well listen, we are here to help. We will find your family and get you home. Can you tell me what was going on today?",
                        5000);
                    _victim1.Tasks.Cower(-1);
                    Game.DisplaySubtitle(
                        "~b~Bailey Smith~s~: They gave me this fake id.. They were going to give me away I think! Please I want to go home!");
                });
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }

        base.Conversations(sender, selItem, index);
    }
}