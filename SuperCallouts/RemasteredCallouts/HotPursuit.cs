using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInterface("[SC] High Speed Pursuit", CalloutProbability.Medium, "High performance vehicle fleeing from police", "Code 3")]
internal class HotPursuit : SuperCallout
{
    private Ped _bad1;
    private Ped _bad2;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private string _name1;
    private string _name2;
    private LHandle _pursuit = Functions.CreatePursuit();
    private UIMenuItem _speakSuspect;
    private UIMenuItem _speakSuspect2;
    private bool _blipHelper;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "High Speed Pursuit";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~o~Traffic ANPR Report:~s~ High value stolen vehicle located.";
        CalloutAdvisory = "This is a powerful vehicle, suspect known to evade police in the past.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Car",
            "ANPR has spotted a stolen vehicle. Suspect is known to flee. Respond ~r~CODE-3");

        Model[] vehicleModels = { "THRAX", "TORERO2", "CHAMPION", "ENTITY3", "THRAX", "FMJ", "ZORRUSSO", "TIGON", "FURIA" };
        _cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], SpawnPoint.Position);
        _cVehicle.IsPersistent = true;
        _cVehicle.IsStolen = true;
        PyroFunctions.AddSearchItem("~y~Wire cutters", null, _cVehicle);
        PyroFunctions.AddSearchItem("~r~Empty beer cans", null, _cVehicle);
        PyroFunctions.AddSearchItem("~r~Opened ammo box", null, _cVehicle);
        EntitiesToClear.Add(_cVehicle);

        _bad1 = _cVehicle.CreateRandomDriver();
        _bad1.IsPersistent = true;
        _bad1.BlockPermanentEvents = true;
        _name1 = Functions.GetPersonaForPed(_bad1).FullName;
        _bad1.Inventory.Weapons.Add(WeaponHash.Pistol);
        _bad1.SetDrunk(Enums.DrunkState.ModeratelyDrunk);
        _bad1.SetWanted(true);
        _bad1.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.Revoked);
        PyroFunctions.AddFirearmItem("Pistol", "weapon_pistol_mk2", true, true, _bad1);
        PyroFunctions.AddSearchItem("~r~Used meth pipe", _bad1);
        _bad1.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);
        EntitiesToClear.Add(_bad1);

        _bad2 = new Ped();
        _bad2.WarpIntoVehicle(_cVehicle, 0);
        _bad2.IsPersistent = true;
        _bad2.BlockPermanentEvents = true;
        _name2 = Functions.GetPersonaForPed(_bad2).FullName;
        _bad2.SetDrunk(Enums.DrunkState.Tipsy);
        EntitiesToClear.Add(_bad2);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
        ConvoMenu.AddItem(_speakSuspect);
        ConvoMenu.AddItem(_speakSuspect2);
        _speakSuspect.Enabled = false;
        _speakSuspect2.Enabled = false;

        _cBlip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 15);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutRunning()
    {
        if ( !OnScene && !_blipHelper )
        {
            GameFiber.StartNew(() =>
            {
                _blipHelper = true;
                SpawnPoint = new Location(_cVehicle.Position);
                _cBlip.DisableRoute();
                _cBlip.Position = SpawnPoint.Position;
                _cBlip.EnableRoute(Color.Red);
                GameFiber.Sleep(2500);
                _blipHelper = false;
            });
        }
        
        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit) 
            && Player.DistanceTo(_bad1) > 75 && Player.DistanceTo(_bad2) > 75) CalloutEnd();

        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit))
        {
            Questioning.Enabled = true;
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

        SpawnPoint = new Location(_cVehicle.Position);
    }

    internal override void CalloutOnScene()
    {
        CalloutInterfaceAPI.Functions.SendMessage(this, "Show me in pursuit!");
        if ( _cBlip.Exists() ) _cBlip.Delete();
        _bad1.BlockPermanentEvents = false;
        _bad2.BlockPermanentEvents = false;
        _pursuit = PyroFunctions.StartPursuit(false, true, _bad1, _bad2);

    Game.DisplayHelp("~r~Suspects are evading!");
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                _bad1.Tasks.FaceEntity(Player);
                Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                GameFiber.Wait(5000);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: I don't know, why do you think?", 5000);
            });

        if (selItem == _speakSuspect2)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                _bad2.Tasks.FaceEntity(Player);
                Game.DisplaySubtitle("~g~You~s~: You know this is a stolen vehicle right? What are you guys doing?", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name2 +
                    "~s~: I didn't do anything wrong, I was just hanging out with my buddy and all this happened.", 5000);
            });
        base.Conversations(sender, selItem, index);
    }
}