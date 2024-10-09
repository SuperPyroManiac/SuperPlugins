using System;
using System.Linq;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Reckless Driving", "Stop The Vehicle")]
internal class RecklessDriver : AmbientEvent
{
    private Ped _ePed;
    private Vehicle _eVehicle;
    private Vector3 _spawnPoint;

    private Tasks _tasks = Tasks.CheckDistance;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        var randomVehicles = Player.GetNearbyVehicles(15);
        if (randomVehicles == null || randomVehicles.Length == 0)
        {
            EndEvent(true);
            return;
        }

        foreach (var randomVehicle in randomVehicles)
        {
            if (!randomVehicle.Exists() || !randomVehicle.HasDriver) return;
            _eVehicle = randomVehicle;
        }

        if (!_eVehicle || !_eVehicle.Exists() || !_eVehicle.HasDriver)
        {
            EndEvent(true);
            return;
        }

        _ePed = _eVehicle.Driver;
        _spawnPoint = _eVehicle.Position;
        EventLocation = _spawnPoint;
        //eVehicle
        _eVehicle.IsPersistent = true;
        //ePed
        _ePed.IsPersistent = true;
        if (_ePed == Player || _eVehicle.HasSiren || !_ePed.IsHuman ||
            _ePed.RelationshipGroup == RelationshipGroup.Fireman ||
            _ePed.RelationshipGroup == RelationshipGroup.Medic || _ePed.RelationshipGroup == RelationshipGroup.Cop)
        {
            EndEvent();
        }
    }

    protected override void OnProcess()
    {
        try
        {
            if ( !_ePed )
            {
                EndEvent(true);
                return;
            }
            
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 15f)
                    {
                        var rrNd = new Random(DateTime.Now.Millisecond).Next(1, 3);
                        switch (rrNd)
                        {
                            case 1:
                                _ePed.Tasks.CruiseWithVehicle(_eVehicle, 20f, VehicleDrivingFlags.Reverse);
                                break;
                            case 2:
                                _ePed.Tasks.CruiseWithVehicle(_eVehicle, 20f, VehicleDrivingFlags.AllowWrongWay);
                                break;
                            case 3:
                                _ePed.Tasks.CruiseWithVehicle(_eVehicle, 20f, VehicleDrivingFlags.Emergency);
                                break;
                            default:
                                EndEvent();
                                break;
                        }

                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    if (Functions.IsPlayerPerformingPullover())
                    {
                        foreach (var blip in BlipsToClear.Where(blip => blip))
                        {
                            if (blip.Exists()) blip.Delete();
                        }

                        _tasks = Tasks.CheckPullover;
                    }

                    break;
                case Tasks.CheckPullover:
                    var rNd = new Random(DateTime.Now.Millisecond).Next(1, 5);
                    switch (rNd)
                    {
                        case 1:
                            Functions.ForceEndCurrentPullover();
                            var pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, _ePed);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            break;
                        case 2:
                            PyroFunctions.SetWanted(_ePed, true);
                            break;
                        case 3:
                            PyroFunctions.SetDrunkOld(_ePed, true);
                            break;
                        case 4:
                            PyroFunctions.SetWanted(_ePed, false);
                            break;
                        default:
                            EndEvent();
                            break;
                    }

                    _tasks = Tasks.End;
                    break;
                case Tasks.End:
                    break;
                default:
                    EndEvent(true);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error( e.ToString());
            EndEvent(true);
        }
    }

    protected override void OnCleanup()
    {
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        CheckPullover,
        End
    }
}