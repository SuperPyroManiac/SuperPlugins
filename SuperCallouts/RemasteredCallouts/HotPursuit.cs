using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] High Speed Pursuit", CalloutProbability.Medium)]
internal class HotPursuit : SuperCallout
{
    private Ped _driver;
    private Ped _passenger;
    private Blip _blip;
    private Vehicle _vehicle;
    private string _driverName;
    private string _passengerName;
    private LHandle _pursuit = Functions.CreatePursuit();
    private UIMenuItem _speakDriver;
    private UIMenuItem _speakPassenger;
    private bool _blipHelper;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(350f)));
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "High Speed Pursuit";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~o~Traffic ANPR Report:~s~ High value stolen vehicle located.";
        CalloutAdvisory = "This is a powerful vehicle, suspect known to evade police in the past.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_HOT_PURSUIT IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Stolen Car",
            "ANPR has spotted a stolen vehicle. Suspect is known to flee. Respond ~r~CODE-3"
        );

        SpawnVehicle();
        SpawnSuspects();
        CreateConversationOptions();
        CreateBlip();
    }

    private void SpawnVehicle()
    {
        Model[] vehicleModels = ["THRAX", "TORERO2", "CHAMPION", "ENTITY3", "THRAX", "FMJ", "ZORRUSSO", "TIGON", "FURIA"];
        _vehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], SpawnPoint.Position);
        _vehicle.IsPersistent = true;
        _vehicle.IsStolen = true;

        PyroFunctions.AddSearchItem("~y~Wire cutters", null, _vehicle);
        PyroFunctions.AddSearchItem("~r~Empty beer cans", null, _vehicle);
        PyroFunctions.AddSearchItem("~r~Opened ammo box", null, _vehicle);

        EntitiesToClear.Add(_vehicle);
    }

    private void SpawnSuspects()
    {
        // Driver
        _driver = _vehicle.CreateRandomDriver();
        _driver.IsPersistent = true;
        _driver.BlockPermanentEvents = true;
        _driverName = Functions.GetPersonaForPed(_driver).FullName;
        _driver.Inventory.Weapons.Add(WeaponHash.Pistol);
        _driver.SetDrunk(Enums.DrunkState.ModeratelyDrunk);
        _driver.SetWanted(true);
        _driver.SetLicenseStatus(Enums.Permits.Guns, Enums.PermitStatus.Revoked);
        PyroFunctions.AddFirearmItem("Pistol", "weapon_pistol_mk2", true, true, _driver);
        PyroFunctions.AddSearchItem("~r~Used meth pipe", _driver);
        _driver.Tasks.CruiseWithVehicle(_vehicle, 10f, VehicleDrivingFlags.Normal);
        EntitiesToClear.Add(_driver);

        // Passenger
        _passenger = new Ped();
        _passenger.WarpIntoVehicle(_vehicle, 0);
        _passenger.IsPersistent = true;
        _passenger.BlockPermanentEvents = true;
        _passengerName = Functions.GetPersonaForPed(_passenger).FullName;
        _passenger.SetDrunk(Enums.DrunkState.Tipsy);
        EntitiesToClear.Add(_passenger);
    }

    private void CreateConversationOptions()
    {
        _speakDriver = new UIMenuItem($"Speak with ~y~{_driverName}");
        _speakPassenger = new UIMenuItem($"Speak with ~y~{_passengerName}");
        ConvoMenu.AddItem(_speakDriver);
        ConvoMenu.AddItem(_speakPassenger);
        _speakDriver.Enabled = false;
        _speakPassenger.Enabled = false;
    }

    private void CreateBlip()
    {
        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true, false, 15);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutRunning()
    {
        if (!_driver || !_passenger || !_vehicle)
        {
            CalloutEnd(true);
            return;
        }

        UpdateBlipIfNeeded();
        CheckPursuitStatus();
        UpdateSuspectStatus();
        SpawnPoint = new Location(_vehicle.Position);
    }

    private void UpdateBlipIfNeeded()
    {
        if (_blip && !OnScene && !_blipHelper)
        {
            GameFiber.StartNew(() =>
            {
                _blipHelper = true;
                SpawnPoint = new Location(_vehicle.Position);
                _blip.DisableRoute();
                _blip.Position = SpawnPoint.Position;
                _blip.EnableRoute(Color.Red);
                GameFiber.Sleep(2500);
                _blipHelper = false;
            });
        }
    }

    private void CheckPursuitStatus()
    {
        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit) && Player.DistanceTo(_driver) > 75 && Player.DistanceTo(_passenger) > 75)
            CalloutEnd();

        if (OnScene && !Functions.IsPursuitStillRunning(_pursuit))
        {
            Questioning.Enabled = true;
            _speakDriver.Enabled = true;
            _speakPassenger.Enabled = true;
        }
    }

    private void UpdateSuspectStatus()
    {
        if (_driver.IsDead)
        {
            _speakDriver.Enabled = false;
            _speakDriver.RightLabel = "~r~Dead";
        }

        if (_passenger.IsDead)
        {
            _speakPassenger.Enabled = false;
            _speakPassenger.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_blip || !_driver || !_passenger)
        {
            CalloutEnd(true);
            return;
        }

        _blip.Delete();
        _driver.BlockPermanentEvents = false;
        _passenger.BlockPermanentEvents = false;
        _pursuit = PyroFunctions.StartPursuit(false, true, _driver, _passenger);

        Game.DisplayHelp("~r~Suspects are evading!");
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_driver || !_passenger)
        {
            CalloutEnd(true);
            return;
        }

        if (selItem == _speakDriver)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakDriver.Enabled = false;
                    _driver.Tasks.FaceEntity(Player);
                    Game.DisplaySubtitle("~g~You~s~: Why are you running?", 5000);
                    GameFiber.Wait(5000);
                    _driver.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle($"~r~{_driverName}~s~: I don't know, why do you think?", 5000);
                }
            );
        }

        if (selItem == _speakPassenger)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakDriver.Enabled = false;
                    _passenger.Tasks.FaceEntity(Player);
                    Game.DisplaySubtitle("~g~You~s~: You know this is a stolen vehicle right? What are you guys doing?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_passengerName}~s~: I didn't do anything wrong, I was just hanging out with my buddy and all this happened.", 5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
