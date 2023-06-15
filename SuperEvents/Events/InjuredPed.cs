using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;
using RAGENativeUI.Elements;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Injured Person", "Ensure the person is ok!")]
internal class InjuredPed : AmbientEvent
{
    private Ped _bad;
    private Ped _bad2;
    private Vector3 _spawnPoint;
    private float _spawnPointH;
    private string _name1;
    private string _name2;
    private readonly int _choice = new Random().Next(1, 4);

    private Tasks _tasks = Tasks.CheckDistance;

    //UI
    private UIMenuItem _speakInjured;
    private UIMenuItem _speakInjured2;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            End(true);
            return;
        }

        //Peds
        _bad = new Ped(_spawnPoint) { Heading = _spawnPointH, IsPersistent = true, BlockPermanentEvents = true };
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        _name2 = Functions.GetPersonaForPed(_bad2).FullName;
        switch (_choice)
        {
            case 1:
                _bad.IsRagdoll = true;
                _speakInjured = new UIMenuItem("Speak with ~y~" + _name1);
                break;
            case 2:
                _bad.Kill();
                _bad2 = new Ped(_bad.GetOffsetPositionFront(2));
                _bad2.IsPersistent = true;
                _speakInjured = new UIMenuItem("Speak with ~y~" + _name1);
                _speakInjured2 = new UIMenuItem("Speak with ~y~" + _name2);
                break;
            case 3:
                _bad.IsRagdoll = true;
                PyroFunctions.SetAnimation(_bad, "move_injured_ground");
                _speakInjured = new UIMenuItem("Speak with ~y~" + _name1);
                break;
            default:
                End(true);
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
                            if (!_bad.IsAnySpeechPlaying) _bad.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                            break;
                        case 2:
                            if (!_bad2.IsAnySpeechPlaying) _bad2.PlayAmbientSpeech("GENERIC_WAR_CRY");
                            break;
                        case 3:
                            if (!_bad.IsAnySpeechPlaying) _bad.PlayAmbientSpeech("GENERIC_FRIGHTENED_MED");
                            break;
                        default:
                            End(true);
                            break;
                    }

                    break;
                case Tasks.End:
                    break;
                default:
                    End(true);
                    break;
            }

            base.OnProcess();
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
        End
    }
}