using System;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents2.SimpleFunctions;

namespace SuperEvents2.Events
{
    public class Fight : AmbientEvent
    {
        private Vector3 _spawnPoint;
        private float _spawnPointH;
        private Ped _suspect;
        private Ped _suspect2;
        //UI Items
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;

        public override void StartEvent(Vector3 s, float f)
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Player) < 35f) {End(true); return;}
            //Suspect1
            _suspect = new Ped(_spawnPoint) {IsPersistent = true, BlockPermanentEvents = true};
            EFunctions.SetDrunk(_suspect, true);
            var _name1 = Functions.GetPersonaForPed(_suspect).FullName;
            _suspect.Metadata.stpAlcoholDetected = true;
            EntitiesToClear.Add(_suspect);
            //Suspect2
            _suspect2 = new Ped(_suspect.FrontPosition) {IsPersistent = true, BlockPermanentEvents = true};
            EFunctions.SetDrunk(_suspect2, true);
            var _name2 = Functions.GetPersonaForPed(_suspect2).FullName;
            _suspect2.Metadata.stpAlcoholDetected = true;
            EntitiesToClear.Add(_suspect2);
            //UI Items
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            base.StartEvent(_spawnPoint, _spawnPointH);
        }

        protected override void Process()
        {
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (!_suspect.IsAnySpeechPlaying) _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    if (!_suspect2.IsAnySpeechPlaying) _suspect2.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 20f)
                    {
                        if (Settings.ShowHints)
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                "~r~A Fight", "Stop the fight, and make sure everyone is ok.");
                        Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        _questioning.Enabled = true;
                        _tasks = Tasks.OnScene;
                    }
                    break;
                case Tasks.OnScene:
                    var choice = new Random().Next(1,4);
                    switch (choice)
                    {
                        case 1:
                            _suspect.Tasks.FightAgainst(_suspect2);
                            _suspect2.Tasks.FightAgainst(_suspect);
                            break;
                        case 2:
                            break;//TODO: SCENES
                        case 3:
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

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
            {
                End(false);
            }
            if (selItem == _speakSuspect2)//TODO: SPEACH
            {
                End(false);
            }
            base.Conversations(sender, selItem, index);
        }

        private Tasks _tasks = Tasks.CheckDistance;
        private enum Tasks
        {
            CheckDistance,
            OnScene,
            End
        }
    }
}