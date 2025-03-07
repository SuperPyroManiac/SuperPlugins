using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Ambulance Escort", CalloutProbability.Medium)]
internal class AmbulanceEscort : SuperCallout
{
    private readonly List<Vector3> _hospitals = [new(1825, 3692, 34), new(-454, -339, 34), new(293, -1438, 29), new(-232, 6316, 30), new(294, -1439, 29)];

    private Blip _ambulanceBlip;
    private Blip _hospitalBlip;
    private Vehicle _ambulance;
    private Ped _paramedic1;
    private Ped _paramedic2;
    private Vector3 _hospital;
    private Ped _victim;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(400, 70);
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Ambulance Escort";

    internal override void CalloutPrep()
    {
        _hospital = _hospitals.OrderBy(x => x.DistanceTo(Player.Position)).FirstOrDefault();
        CalloutMessage = "~b~Dispatch:~s~ Ambulance requests police escort.";
        CalloutAdvisory = "Ambulance needs assistance clearing traffic.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Ambulance Escort",
            "Ambulance has a wounded police officer in critical condition, ensure the ambulance has a clear path to the nearest hospital, get to the scene! High priority, respond ~y~CODE-3"
        );

        SpawnAmbulance();
        SpawnParamedics();
        SpawnVictim();
        CreateAmbulanceBlip();
    }

    private void SpawnAmbulance()
    {
        _ambulance = new Vehicle("AMBULANCE", SpawnPoint.Position)
        {
            Heading = SpawnPoint.Heading,
            IsPersistent = true,
            IsSirenOn = true,
        };
        EntitiesToClear.Add(_ambulance);
    }

    private void SpawnParamedics()
    {
        _paramedic1 = new Ped("s_m_m_paramedic_01", SpawnPoint.Position, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _paramedic1.WarpIntoVehicle(_ambulance, -1);
        EntitiesToClear.Add(_paramedic1);

        _paramedic2 = new Ped("s_m_m_paramedic_01", SpawnPoint.Position, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _paramedic2.WarpIntoVehicle(_ambulance, 0);
        EntitiesToClear.Add(_paramedic2);
    }

    private void SpawnVictim()
    {
        _victim = new Ped("s_m_y_hwaycop_01", SpawnPoint.Position, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _victim.WarpIntoVehicle(_ambulance, 1);
        EntitiesToClear.Add(_victim);
    }

    private void CreateAmbulanceBlip()
    {
        _ambulanceBlip = _ambulance.AttachBlip();
        _ambulanceBlip.EnableRoute(Color.Green);
        _ambulanceBlip.Color = Color.Green;
        BlipsToClear.Add(_ambulanceBlip);
    }

    internal override void CalloutRunning()
    {
        if (!_ambulance || !_paramedic1 || !_paramedic2 || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        if (_ambulance.DistanceTo(_hospital) < 15f && OnScene)
        {
            HandleArrivalAtHospital();
        }
    }

    private void HandleArrivalAtHospital()
    {
        _ambulance.IsSirenSilent = true;

        if (_paramedic1.IsInAnyVehicle(false))
            _paramedic1.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

        if (_paramedic2.IsInAnyVehicle(false))
            _paramedic2.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

        if (_victim.IsInAnyVehicle(false))
            _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

        CalloutEnd();
    }

    internal override void CalloutOnScene()
    {
        if (!_ambulance || !_paramedic1 || !_paramedic2 || !_victim || !_ambulanceBlip)
        {
            CalloutEnd(true);
            return;
        }

        Game.DisplayHelp("Ensure the ambulance has a clear path!");
        _ambulanceBlip.DisableRoute();

        if (_paramedic1.IsInAnyVehicle(false))
            _paramedic1.Tasks.DriveToPosition(_ambulance, _hospital, 20f, VehicleDrivingFlags.Emergency, 10f);

        CreateHospitalBlip();
    }

    private void CreateHospitalBlip()
    {
        _hospitalBlip = new Blip(_hospital);
        _hospitalBlip.EnableRoute(Color.Blue);
        _hospitalBlip.Color = Color.Blue;
        BlipsToClear.Add(_hospitalBlip);
    }
}
