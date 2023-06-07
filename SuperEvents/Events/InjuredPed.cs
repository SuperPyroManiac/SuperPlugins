using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.Events;
using Rage;
using RAGENativeUI.Elements;
using SuperEvents.EventFunctions;

namespace SuperEvents.Events
{
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

        protected override void StartEvent()
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            EventLocation = _spawnPoint;
            if (_spawnPoint.DistanceTo(Player) < 35f)
            {
                End(true);
                return;
            }
            //Peds
            _bad = new Ped(_spawnPoint) {Heading = _spawnPointH, IsPersistent = true, BlockPermanentEvents = true};
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
                    EFunctions.SetAnimation(_bad, "move_injured_ground");
                    _speakInjured = new UIMenuItem("Speak with ~y~" + _name1);
                    break;
                default:
                    End(true);
                    break;
            }
            base.StartEvent();
        }

        protected override void Process()
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
            End
        }
    }
}