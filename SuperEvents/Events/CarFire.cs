using System;
using PyroCommon.API;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("A Fire", "Call the Fire Department and clear the scene!")]
internal class CarFire : AmbientEvent
{
    private Vehicle _eVehicle;
    private Vector3 _spawnPoint;
    private Tasks _tasks = Tasks.CheckDistance;
    private Ped _victim;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        //eVehicle
        PyroFunctions.SpawnNormalCar(out _eVehicle, _spawnPoint);
        EntitiesToClear.Add(_eVehicle);
    }

    protected override void OnProcess()
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
                    var choice = new Random().Next(1, 4);
                    Log.Info("Fire event picked scenerio #" + choice);
                    switch (choice)
                    {
                        case 1:
                            PyroFunctions.FireControl(_spawnPoint.Around2D(4f), 24, true);
                            PyroFunctions.FireControl(_spawnPoint.Around2D(4f), 24, false);
                            break;
                        case 2:
                            _eVehicle.Explode();
                            PyroFunctions.FireControl(_spawnPoint.Around2D(4f), 10, true);
                            break;
                        case 3:
                            _victim = _eVehicle.CreateRandomDriver();
                            _victim.IsPersistent = true;
                            EntitiesToClear.Add(_victim);
                            PyroFunctions.FireControl(_spawnPoint.Around2D(4f), 24, true);
                            PyroFunctions.FireControl(_spawnPoint.Around2D(4f), 24, false);
                            break;
                        default:
                            EndEvent(true);
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
        End
    }
}