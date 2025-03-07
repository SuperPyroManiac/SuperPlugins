using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Truck Crash", CalloutProbability.Medium)]
internal class TruckCrash : SuperCallout
{
    private Blip _sceneBlip;
    private Vehicle _truck;
    private Ped _driver;
    private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for an ambulance.");

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Truck Crash";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a truck crash.";
        CalloutAdvisory = "Caller reports a truck has crashed.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Truck Crash", "Reports of a truck crash, respond ~r~CODE-3");

        SpawnTruck();
        SpawnDriver();
        SetupMenu();
        CreateBlip();
    }

    private void SpawnTruck()
    {
        Model[] truckModels = ["PHANTOM", "HAULER", "PACKER"];
        _truck = new Vehicle(truckModels[new Random(DateTime.Now.Millisecond).Next(truckModels.Length)], SpawnPoint.Position);
        _truck.Heading = SpawnPoint.Heading;
        _truck.EngineHealth = 0;
        PyroFunctions.DamageVehicle(_truck, 200, 200);
        EntitiesToClear.Add(_truck);
    }

    private void SpawnDriver()
    {
        _driver = _truck.CreateRandomDriver();
        _driver.IsPersistent = true;
        _driver.Kill();
        EntitiesToClear.Add(_driver);
    }

    private void SetupMenu()
    {
        MainMenu.RemoveItemAt(1);
        MainMenu.AddItem(_callEms);
        MainMenu.AddItem(EndCall);
        _callEms.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _callEms.Enabled = false;
    }

    private void CreateBlip()
    {
        _sceneBlip = _truck.AttachBlip();
        _sceneBlip.Color = Color.Red;
        _sceneBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_sceneBlip);
    }

    internal override void CalloutOnScene()
    {
        _sceneBlip?.DisableRoute();
        _callEms.Enabled = true;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callEms)
        {
            Game.DisplaySubtitle("~g~You~s~: Dispatch, we have a truck crash with a driver that appears to be deceased. I need EMS out here.");
            PyroFunctions.RequestBackup(Enums.BackupType.Fire);
            PyroFunctions.RequestBackup(Enums.BackupType.Medical);

            _callEms.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }
}
