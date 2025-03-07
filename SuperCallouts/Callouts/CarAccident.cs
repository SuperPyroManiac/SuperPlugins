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

[CalloutInfo("[SC] Car Accident", CalloutProbability.Medium)]
internal class CarAccident : SuperCallout
{
    private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for an ambulance.");
    private Blip _vehicleBlip;
    private Vehicle _vehicle;
    private Ped _victim;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (1)";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports possible hit and run.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~MVA", "Reports of a car accident, respond ~r~CODE-3");

        SpawnVehicleAndVictim();
        SetupMenu();
        CreateBlip();
    }

    private void SpawnVehicleAndVictim()
    {
        PyroFunctions.SpawnAnyCar(out _vehicle, SpawnPoint.Position);
        _vehicle.Heading = SpawnPoint.Heading;
        PyroFunctions.DamageVehicle(_vehicle, 200, 200);
        EntitiesToClear.Add(_vehicle);

        _victim = _vehicle.CreateRandomDriver();
        _victim.IsPersistent = true;
        _victim.Kill();
        EntitiesToClear.Add(_victim);
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
        _vehicleBlip = _vehicle.AttachBlip();
        _vehicleBlip.Color = Color.Red;
        _vehicleBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_vehicleBlip);
    }

    internal override void CalloutOnScene()
    {
        _vehicleBlip?.DisableRoute();
        _callEms.Enabled = true;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callEms)
        {
            Game.DisplaySubtitle("~g~You~s~: Dispatch, we have a vehicle accident, possible hit and run. Looks like someone is inside and injured! I need EMS out here.");
            PyroFunctions.RequestBackup(Enums.BackupType.Fire);
            PyroFunctions.RequestBackup(Enums.BackupType.Medical);

            _callEms.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }
}
