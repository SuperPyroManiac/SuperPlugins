#region
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Ambulance Escort", CalloutProbability.Medium, "Ambulance requires escort", "Code 3")]
internal class AmbulanceEscort : SuperCallout
{
    internal override Vector3 SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Ambulance Escort";
    private readonly List<Vector3> _hospitals = new()
    {
        new Vector3(1825, 3692, 34),
        new Vector3(-454, -339, 34),
        new Vector3(293, -1438, 29),
        new Vector3(-232, 6316, 30),
        new Vector3(294, -1439, 29)
    };
    private Blip _cBlip;
    private Blip _cBlip2;
    private Vehicle _cVehicle;
    private Ped _doc1;
    private Ped _doc2;
    private Vector3 _hospital;
    private float _spawnPointH;
    private Ped _victim;

    internal override void CalloutPrep()
    {
        _hospital = _hospitals.OrderBy(x => x.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
        PyroFunctions.FindSideOfRoad(400, 70, out var tempSpawnPoint, out _spawnPointH);
        SpawnPoint = tempSpawnPoint;
        CalloutMessage = "~b~Dispatch:~s~ Ambulance requests police escort.";
        CalloutAdvisory = "Ambulance needs assistance clearing traffic.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION", SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Ambulance Escort",
            "Ambulance has a wounded police officer in critical condition, ensure the ambulance has a clear path to the nearest hospital, get to the scene! High priority, respond ~y~CODE-3");

        _cVehicle = new Vehicle("AMBULANCE", SpawnPoint)
        { Heading = _spawnPointH, IsPersistent = true, IsSirenOn = true };
        EntitiesToClear.Add(_cVehicle);

        _doc1 = new Ped("s_m_m_paramedic_01", SpawnPoint, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _doc1.WarpIntoVehicle(_cVehicle, -1);
        EntitiesToClear.Add(_doc1);

        _doc2 = new Ped("s_m_m_paramedic_01", SpawnPoint, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _doc2.WarpIntoVehicle(_cVehicle, 0);
        EntitiesToClear.Add(_doc2);

        _victim = new Ped("s_m_y_hwaycop_01", SpawnPoint, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _victim.WarpIntoVehicle(_cVehicle, 1);
        EntitiesToClear.Add(_victim);

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.EnableRoute(Color.Green);
        _cBlip.Color = Color.Green;
        BlipsToClear.Add(_cBlip);
    }
    
    internal override void CalloutRunning()
    {
        if (_cVehicle.DistanceTo(_hospital) < 15f && OnScene)
        {
            _cVehicle.IsSirenSilent = true;
            _doc1.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
            _doc2.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
            _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
            CalloutEnd();
        }
    }

    internal override void CalloutOnScene()
    {
        Game.DisplayHelp("Ensure the ambulance has a clear path!");
        _cBlip.DisableRoute();
        _doc1.Tasks.DriveToPosition(_cVehicle, _hospital, 20f, VehicleDrivingFlags.Emergency, 10f);
        _cBlip2 = new Blip(_hospital);
        _cBlip2.EnableRoute(Color.Blue);
        _cBlip2.Color = Color.Blue;
        BlipsToClear.Add(_cBlip2);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Proceed to nearest medical center.");
    }
}