using System;
using PyroCommon.PyroFunctions;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Injured Person", "Ensure the person is ok!")]
internal class InjuredPed : AmbientEvent
{
    private Ped? _bad;
    private Ped? _bad2;
    private Vector3 _spawnPoint;
    private float _spawnPointH;
    private readonly int _choice = new Random(DateTime.Now.Millisecond).Next(1, 4);

    private Tasks _tasks = Tasks.CheckDistance;

    //UI

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        //Peds
        _bad = new Ped(_spawnPoint) { Heading = _spawnPointH, IsPersistent = true, BlockPermanentEvents = true };
        switch (_choice)
        {
            case 1:
                _bad.IsRagdoll = true;
                break;
            case 2:
                _bad.Kill();
                _bad2 = new Ped(_bad.GetOffsetPositionFront(2));
                _bad2.IsPersistent = true;
                break;
            case 3:
                _bad.IsRagdoll = true;
                PyroFunctions.SetAnimation(_bad, "move_injured_ground");
                break;
            default:
                EndEvent(true);
                break;
        }
    }

    protected override void OnProcess()
    {
        try
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    switch (_choice)
                    {
                        case 1:
                            if (_bad != null && !_bad.IsAnySpeechPlaying) _bad.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                            break;
                        case 2:
                            if (_bad2 != null && !_bad2.IsAnySpeechPlaying) _bad2.PlayAmbientSpeech("GENERIC_WAR_CRY");
                            break;
                        case 3:
                            if (_bad != null && !_bad.IsAnySpeechPlaying) _bad.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                            break;
                        default:
                            EndEvent(true);
                            break;
                    }

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
        End
    }
}