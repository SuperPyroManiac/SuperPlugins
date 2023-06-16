using System;
using PyroCommon.API;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Wild Animal", "Stop the animal from hurting anyone.")]
internal class WildAnimal : AmbientEvent
{
    private Tasks _tasks = Tasks.CheckDistance;
    private Ped _animal;
    private Vector3 _spawnPoint;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Ped
        _spawnPoint = World.GetNextPositionOnStreet(Player.Position.Around(150f));
        EventLocation = _spawnPoint;
        Model[] meanAnimal = { "A_C_MTLION", "A_C_COYOTE" };
        _animal = new Ped(meanAnimal[new Random().Next(meanAnimal.Length)], _spawnPoint, 50) { IsPersistent = true };
    }

    protected override void OnProcess()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Player.DistanceTo(_animal) < 20f)
                    {
                        _animal.Tasks.FightAgainst(Player);
                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    break;
                default:
                    End(true);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error( e.ToString());
            End(true);
        }
    }

    protected override void OnCleanup()
    {
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene
    }
}