using System;
using PyroCommon.API;
using PyroCommon.Events;
using Rage;
using SuperEvents.EventFunctions;

namespace SuperEvents.Events;

internal class WeirdCar : AmbientEvent
{
    private Vehicle _eVehicle;
    private Vector3 _spawnPoint;

    private Tasks _tasks = Tasks.CheckDistance;

    protected internal override void StartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _);
        EventLocation = _spawnPoint;
        EventTitle = "Abandoned Vehicle";
        EventDescription = "Investigate the vehicle.";
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            End(true);
            return;
        }

        //eVehicle
        PyroFunctions.SpawnNormalCar(out _eVehicle, _spawnPoint);
        EntitiesToClear.Add(_eVehicle);

        base.StartEvent();
    }

    protected internal override void Process()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                    {
                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    var choice = new Random().Next(1, 7);
                    Game.LogTrivial("SuperEvents: Abandoned Vehicle event picked scenario #" + choice);
                    switch (choice)
                    {
                        case 1:
                            PyroFunctions.DamageVehicle(_eVehicle, 200, 200);
                            break;
                        case 2:
                            PyroFunctions.DamageVehicle(_eVehicle, 200, 200);
                            _eVehicle.IsStolen = true;
                            break;
                        case 3:
                            _eVehicle.IsEngineOn = true;
                            _eVehicle.IsInteriorLightOn = true;
                            break;
                        case 4:
                            _eVehicle.Rotation = new Rotator(0f, 180f, 0f);
                            break;
                        case 5:
                            _eVehicle.IsStolen = true;
                            break;
                        case 6:
                            _eVehicle.AlarmTimeLeft = TimeSpan.MaxValue;
                            break;
                        default:
                            End(true);
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
        End
    }
}