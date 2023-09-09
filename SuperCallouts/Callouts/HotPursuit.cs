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

[CalloutInterface("[SC] High Speed Pursuit", CalloutProbability.Medium, "High performance vehicle fleeing from police",
    "Code 3")]
internal class HotPursuit : SuperCallout
{
    private Ped _bad1;
    private Ped _bad2;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private string _name1;
    private string _name2;
    private LHandle _pursuit;
    private UIMenuItem _speakSuspect;
    private UIMenuItem _speakSuspect2;
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(350f));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "High Speed Pursuit";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~o~Traffic ANPR Report:~s~ High value stolen vehicle located.";
        CalloutAdvisory = "This is a powerful vehicle known to evade police in the past.";
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Car",
            "ANPR has spotted a stolen vehicle. This vehicle is high performance and has fled before. Respond ~r~CODE-3");

        Model[] vehicleModels =
            { "ZENTORNO", "TEMPESTA", "AUTARCH", "cheetah", "nero2", "tezeract", "visione", "prototipo", "emerus" };
        _cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], SpawnPoint)
            { IsPersistent = true, IsStolen = true };
        _cVehicle.Metadata.searchDriver = "~r~exposed console wires~s~, ~y~wire cutters~s~";
        _cVehicle.Metadata.searchPassenger = "~r~empty beer cans~s~, ~r~opened box of ammo~s~";
        EntitiesToClear.Add(_cVehicle);

        _bad1 = _cVehicle.CreateRandomDriver();
        _bad1.IsPersistent = true;
        _bad1.BlockPermanentEvents = true;
        _name1 = Functions.GetPersonaForPed(_bad1).FullName;
        _bad1.Inventory.Weapons.Add(WeaponHash.Pistol);
        _bad1.Metadata.stpDrugsDetected = true;
        _bad1.Metadata.stpAlcoholDetected = true;
        _bad1.Metadata.searchPed =
            "~r~pistol~s~, ~r~used meth pipe~s~, ~y~hotwire tools~s~, ~g~suspicious taco~s~, ~g~wallet~s~";
        _bad1.Metadata.hasGunPermit = false;
        PyroFunctions.SetWanted(_bad1, true);
        PyroFunctions.SetDrunk(_bad1, true);
        _bad1.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);
        EntitiesToClear.Add(_bad1);

        _bad2 = new Ped();
        _bad2.WarpIntoVehicle(_cVehicle, 0);
        _bad2.IsPersistent = true;
        _bad2.BlockPermanentEvents = true;
        _name2 = Functions.GetPersonaForPed(_bad2).FullName;
        _bad2.Metadata.stpAlcoholDetected = true;
        PyroFunctions.SetDrunk(_bad2, true);
        EntitiesToClear.Add(_bad2);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
        ConvoMenu.AddItem(_speakSuspect);
        ConvoMenu.AddItem(_speakSuspect2);
        _speakSuspect.Enabled = false;
        _speakSuspect2.Enabled = false;

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        _cBlip.Scale = .5f;
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutRunning()
    {
        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit) && Player.DistanceTo(_bad1) > 75 &&
            Player.DistanceTo(_bad2) > 75) CalloutEnd();

        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit))
        {
            _speakSuspect.Enabled = true;
            _speakSuspect2.Enabled = true;
        }

        if (_bad1.IsDead)
        {
            _speakSuspect.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }

        if (_bad2.IsDead)
        {
            _speakSuspect2.Enabled = false;
            _speakSuspect2.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        CalloutInterfaceAPI.Functions.SendMessage(this, "Show me in pursuit!");
        if (_cBlip.Exists()) _cBlip.Delete();
        _bad1.BlockPermanentEvents = false;
        _bad2.BlockPermanentEvents = false;
        _pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(_pursuit, _bad1);
        Functions.AddPedToPursuit(_pursuit, _bad2);
        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
        Game.DisplayHelp("~r~Suspects are evading!");
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't know, why do you think?", 5000);
            });

        if (selItem == _speakSuspect2)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                Game.DisplaySubtitle("~g~You~s~: You know this is a stolen vehicle right? What are you guys doing?",
                    5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad2, Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name2 +
                    "~s~: I didn't do anything wrong, I was just hanging out with my buddy and all this happened.",
                    5000);
            });
        base.Conversations(sender, selItem, index);
    }
}