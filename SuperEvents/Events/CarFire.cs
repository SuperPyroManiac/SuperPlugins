using System;
using PyroCommon.Utils;
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
        CommonUtils.FindSideOfRoad(120, 45, out _spawnPoint, out _);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        //eVehicle
        CommonUtils.SpawnNormalCar(out _eVehicle, _spawnPoint);
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
                    var choice = new Random(DateTime.Now.Millisecond).Next(1, 4);
                    LogUtils.Info("Fire event picked scenerio #" + choice);
                    if (!_eVehicle)
                    {
                        EndEvent(true);
                        break;
                    }
                    switch (choice)
                    {
                        case 1:
                            CommonUtils.FireControl(_spawnPoint.Around2D(4f), 24, true);
                            CommonUtils.FireControl(_spawnPoint.Around2D(4f), 24, false);
                            break;
                        case 2:
                            _eVehicle.Explode();
                            CommonUtils.FireControl(_spawnPoint.Around2D(4f), 10, true);
                            break;
                        case 3:
                            _victim = _eVehicle.CreateRandomDriver();
                            _victim.IsPersistent = true;
                            EntitiesToClear.Add(_victim);
                            CommonUtils.FireControl(_spawnPoint.Around2D(4f), 24, true);
                            CommonUtils.FireControl(_spawnPoint.Around2D(4f), 24, false);
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
            LogUtils.Error(e.ToString());
            EndEvent(true);
        }
    }

    protected override void OnCleanup() { }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        End,
    }
}
