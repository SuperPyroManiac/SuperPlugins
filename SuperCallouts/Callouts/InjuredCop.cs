using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Injured Cop", CalloutProbability.Medium, "Officer not responding to radio.")]
internal class InjuredCop : SuperCallout
{
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Injured Cop";
    private Ped _cop;
    private Ped _bad;
    private Vehicle _vehicle;
    private Blip _blip;
    private int _rNd = new Random(DateTime.Now.Millisecond).Next(2);

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Officer not responding.";
        CalloutAdvisory = "Officer not responding to radio, proceed to their vehicles location.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Officer Not Responding",
            "Officer not reporting back over radio. Investigate their location. Respond ~r~CODE-3");

        _vehicle = new Vehicle("POLICE", SpawnPoint.Position);
        _vehicle.IsPersistent = true;
        _vehicle.Heading = SpawnPoint.Heading;
        _vehicle.IsSirenOn = true;
        _vehicle.IsSirenSilent = true;
        EntitiesToClear.Add(_vehicle);

        _cop = new Ped("s_m_y_cop_01", SpawnPoint.Position.Around2D(5), 0);
        _cop.IsPersistent = true;
        _cop.BlockPermanentEvents = true;
        switch (_rNd)
        {
            case 0:
                _cop.WarpIntoVehicle(_vehicle, -1);
                break;
            case 1:
                _cop.Kill();
                PyroFunctions.DamageVehicle(_vehicle, 100, 100);
                break;
            case 2:
                _bad = new Ped(_cop.Position.Around(2));
                _bad.IsPersistent = true;
                _bad.Kill();
                PyroFunctions.SetWanted(_bad, true);
                EntitiesToClear.Add(_bad);
                break;
        }
        EntitiesToClear.Add(_cop);

        _blip = _vehicle.AttachBlip();
        _blip.EnableRoute(Color.Red);
        _blip.Color = Color.Red;
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        Game.DisplayNotification("Investigate the area.");
        if (_blip.Exists()) _blip.Delete();
        switch (_rNd)
        {
            case 0:
                if (_cop.Exists())
                {
                    _cop.Kill();
                    _cop.BlockPermanentEvents = false;
                }
                break;
            case 1:
                if (_cop.Exists()) _cop.BlockPermanentEvents = false;
                break;
            case 2:
                break;
        }
    }
}