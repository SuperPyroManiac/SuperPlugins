using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Injured Cop", CalloutProbability.Medium)]
internal class InjuredCop : SuperCallout
{
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Injured Cop";

    private Ped _officer;
    private Ped _suspect;
    private Vehicle _policeVehicle;
    private Blip _sceneBlip;
    private readonly int _scenarioType = new Random(DateTime.Now.Millisecond).Next(2);

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Officer not responding.";
        CalloutAdvisory = "Officer not responding to radio, proceed to their vehicles location.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Officer Not Responding",
            "Officer not reporting back over radio. Investigate their location. Respond ~r~CODE-3"
        );

        SpawnPoliceVehicle();
        SpawnOfficer();
        SetupScenario();
        CreateBlip();
    }

    private void SpawnPoliceVehicle()
    {
        _policeVehicle = new Vehicle("POLICE", SpawnPoint.Position)
        {
            IsPersistent = true,
            Heading = SpawnPoint.Heading,
            IsSirenOn = true,
            IsSirenSilent = true,
        };
        EntitiesToClear.Add(_policeVehicle);
    }

    private void SpawnOfficer()
    {
        _officer = new Ped("s_m_y_cop_01", SpawnPoint.Position.Around2D(5), 0) { IsPersistent = true, BlockPermanentEvents = true };
        EntitiesToClear.Add(_officer);
    }

    private void SetupScenario()
    {
        switch (_scenarioType)
        {
            case 0: // Officer in vehicle
                _officer.WarpIntoVehicle(_policeVehicle, -1);
                break;

            case 1: // Dead officer, damaged vehicle
                _officer.Kill();
                PyroFunctions.DamageVehicle(_policeVehicle, 100, 100);
                break;

            case 2: // Dead suspect, officer alive
                _suspect = new Ped(_officer.Position.Around(2)) { IsPersistent = true };
                _suspect.Kill();
                PyroFunctions.SetWanted(_suspect, true);
                EntitiesToClear.Add(_suspect);
                break;
        }
    }

    private void CreateBlip()
    {
        _sceneBlip = _policeVehicle.AttachBlip();
        _sceneBlip.EnableRoute(Color.Red);
        _sceneBlip.Color = Color.Red;
        BlipsToClear.Add(_sceneBlip);
    }

    internal override void CalloutOnScene()
    {
        if (!_officer)
        {
            CalloutEnd(true);
            return;
        }

        Game.DisplayNotification("Investigate the area.");
        _sceneBlip?.Delete();

        CompleteScenario();
    }

    private void CompleteScenario()
    {
        switch (_scenarioType)
        {
            case 0: // Officer in vehicle - kill officer
                if (_officer.Exists())
                {
                    _officer.Kill();
                    _officer.BlockPermanentEvents = false;
                }
                break;

            case 1: // Dead officer - release block
                if (_officer.Exists())
                    _officer.BlockPermanentEvents = false;
                break;

            case 2: // Dead suspect - no additional action needed
                break;
        }
    }
}
