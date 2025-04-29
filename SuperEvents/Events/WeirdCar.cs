using System;
using PyroCommon.Utils;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Abandoned Vehicle", "Investigate the vehicle.")]
internal class WeirdCar : AmbientEvent
{
    private Vehicle _eVehicle;
    private Vector3 _spawnPoint;

    private Tasks _tasks = Tasks.CheckDistance;

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
            if (!_eVehicle)
            {
                EndEvent(true);
                return;
            }

            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                    {
                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    var choice = new Random(DateTime.Now.Millisecond).Next(1, 7);
                    LogUtils.Info("Abandoned Vehicle event picked scenario #" + choice);
                    switch (choice)
                    {
                        case 1:
                            CommonUtils.DamageVehicle(_eVehicle, 200, 200);
                            break;
                        case 2:
                            CommonUtils.DamageVehicle(_eVehicle, 200, 200);
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
