using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Models;
using PyroCommon.Utils;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Models.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Car Accident", CalloutProbability.Medium)]
internal class CarAccident : SuperCallout
{
    private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for an ambulance.");
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private Ped _cVictim;
    internal override Location SpawnPoint { get; set; } = CommonUtils.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (1)";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports possible hit and run.";
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~MVA",
            "Reports of a car accident, respond ~r~CODE-3"
        );

        CommonUtils.SpawnAnyCar(out _cVehicle, SpawnPoint.Position);
        _cVehicle.Heading = SpawnPoint.Heading;
        CommonUtils.DamageVehicle(_cVehicle, 200, 200);
        EntitiesToClear.Add(_cVehicle);

        _cVictim = _cVehicle.CreateRandomDriver();
        _cVictim.IsPersistent = true;
        _cVictim.Kill();
        EntitiesToClear.Add(_cVictim);

        MainMenu.RemoveItemAt(1);
        MainMenu.AddItem(_callEms);
        MainMenu.AddItem(EndCall);
        _callEms.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _callEms.Enabled = false;

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        if (_cBlip.Exists())
            _cBlip.DisableRoute();
        _callEms.Enabled = true;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callEms)
        {
            Game.DisplaySubtitle(
                "~g~You~s~: Dispatch, we have a vehicle accident, possible hit and run. Looks like someone is inside and injured! I need EMS out here."
            );
            CommonUtils.RequestBackup(Enums.BackupType.Fire);
            CommonUtils.RequestBackup(Enums.BackupType.Medical);

            _callEms.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }
}
