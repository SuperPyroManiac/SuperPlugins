using System;
using System.Linq;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using PyroCommon.Events;
using Rage;
using SuperEvents.EventFunctions;

namespace SuperEvents.Events;

internal class RecklessDriver : AmbientEvent
{
    private Ped _ePed;
    private Vehicle _eVehicle;
    private Vector3 _spawnPoint;
    private float _spawnPointH;

    private Tasks _tasks = Tasks.CheckDistance;

    protected internal override void StartEvent()
    {
        //Setup
        var randomVehicles = Player.GetNearbyVehicles(15);
        if (randomVehicles == null || randomVehicles.Length == 0)
        {
            End(true);
            return;
        }

        foreach (var randomVehicle in randomVehicles)
        {
            if (!randomVehicle.Exists() || !randomVehicle.HasDriver) return;
            _eVehicle = randomVehicle;
        }

        if (_eVehicle == null || !_eVehicle.Exists() || !_eVehicle.HasDriver)
        {
            End(true);
            return;
        }

        _ePed = _eVehicle.Driver;
        _spawnPoint = _eVehicle.Position;
        EventLocation = _spawnPoint;
        _spawnPointH = _eVehicle.Heading;
        //eVehicle
        _eVehicle.IsPersistent = true;
        //ePed
        _ePed.IsPersistent = true;
        if (_ePed == Player || _eVehicle.HasSiren || !_ePed.IsHuman ||
            _ePed.RelationshipGroup == RelationshipGroup.Fireman ||
            _ePed.RelationshipGroup == RelationshipGroup.Medic || _ePed.RelationshipGroup == RelationshipGroup.Cop)
        {
            End(false);
            return;
        }

        base.StartEvent();
    }

    protected internal override void Process()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 15f)
                    {
                        if (Settings.ShowHints)
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~Reckless Driving", "Stop the vehicle.");
                        Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        var rrNd = new Random().Next(1, 3);
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
                                End(false);
                                break;
                        }

                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    if (Functions.IsPlayerPerformingPullover())
                    {
                        foreach (var blip in BlipsToClear.Where(blip => blip))
                        {if (blip.Exists()) blip.Delete();}
                        _tasks = Tasks.CheckPullover;
                    }

                    break;
                case Tasks.CheckPullover:
                    var rNd = new Random().Next(1, 5);
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
                            PyroFunctions.SetDrunk(_ePed, true);
                            break;
                        case 4:
                            PyroFunctions.SetWanted(_ePed, false);
                            break;
                        default:
                            End(false);
                            break;
                    }

                    _tasks = Tasks.End;
                    break;
                case Tasks.End:
                    break;
                default:
                    End(true);
                    break;
            }

            base.Process();
        }
        catch (Exception e)
        {
            Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.LogTrivial("SuperEvents Error Report Start");
            Game.LogTrivial("======================================================");
            Game.LogTrivial(e.ToString());
            Game.LogTrivial("======================================================");
            Game.LogTrivial("SuperEvents Error Report End");
            End(true);
        }
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        CheckPullover,
        End
    }
}