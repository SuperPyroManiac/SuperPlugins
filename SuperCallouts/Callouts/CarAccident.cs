#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Car Accident", CalloutProbability.Medium, "Reports of a vehicle crash, limited details", "Code 3")]
internal class CarAccident : SuperCallout
{
    private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for an ambulance.");
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private Ped _cVictim;
    private float _spawnPointH;
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Car Accident (1)";

    internal override void CalloutPrep()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out var tempSpawnPoint, out _spawnPointH);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~b~Dispatch:~s~ Reports of a motor vehicle accident.";
        CalloutAdvisory = "Caller reports possible hit and run.";
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_HIT_AND_RUN_03 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~MVA",
            "Reports of a car accident, respond ~r~CODE-3");

        PyroFunctions.SpawnAnyCar(out _cVehicle, SpawnPoint);
        _cVehicle.Heading = _spawnPointH;
        PyroFunctions.DamageVehicle(_cVehicle, 200, 200);
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
        _cBlip.DisableRoute();
        _callEms.Enabled = true;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callEms)
        {
            Game.DisplaySubtitle(
                "~g~You~s~: Dispatch, we have a vehicle accident, possible hit and run. Looks like someone is inside and injured! I need EMS out here.");
            CalloutInterfaceAPI.Functions.SendMessage(this, "EMS has been notified and is on route. 11-78");
            if (PyroCommon.Main.UsingUB)
            {
                Wrapper.CallEms();
                Wrapper.CallFd();
            }
            else
            {
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Ambulance);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Firetruck);
            }

            _callEms.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }
}