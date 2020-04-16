using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
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
        private string _name1;
        private string _name2;
        //UI Items
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;

        public override void StartEvent(Vector3 s, float f)
        {
            //Setup
            EFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
            if (_spawnPoint.DistanceTo(Player) < 35f) {End(true); return;}
            base.StartEvent(_spawnPoint, _spawnPointH);
            //Suspect1
            _suspect = new Ped(_spawnPoint) {IsPersistent = true, BlockPermanentEvents = true};
            EFunctions.SetDrunk(_suspect, true);
            _name1 = Functions.GetPersonaForPed(_suspect).FullName;
            _suspect.Metadata.stpAlcoholDetected = true;
            EntitiesToClear.Add(_suspect);
            //Suspect2
            _suspect2 = new Ped(_suspect.FrontPosition) {IsPersistent = true, BlockPermanentEvents = true};
            EFunctions.SetDrunk(_suspect2, true);
            _name2 = Functions.GetPersonaForPed(_suspect2).FullName;
            _suspect2.Metadata.stpAlcoholDetected = true;
            EntitiesToClear.Add(_suspect2);
            //UI Items
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            _convoMenu.AddItem(_speakSuspect);
            _convoMenu.AddItem(_speakSuspect2);
        }

        protected override void Process()
        {
            try
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
                        Game.LogTrivial("SuperEvents: Fight event picked scenerio #" + choice);
                        switch (choice)
                        {
                            case 1:
                                _suspect.Tasks.FightAgainst(_suspect2);
                                _suspect2.Tasks.FightAgainst(_suspect);
                                break;
                            case 2:
                                _suspect.Tasks.FightAgainst(_suspect2);
                                _suspect2.Tasks.FightAgainst(_suspect);
                                GameFiber.Wait(4000);
                                _suspect.Tasks.FightAgainst(Player);
                                _suspect2.Tasks.FightAgainst(Player);
                                break;
                            case 3:
                                _suspect.Tasks.Cower(-1);
                                _suspect2.Inventory.Weapons.Add(WeaponHash.Pistol);
                                _suspect2.Tasks.FireWeaponAt(_suspect, -1, FiringPattern.BurstFirePistol);
                                GameFiber.Wait(2000);
                                _suspect2.BlockPermanentEvents = false;
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
                //UI Items
                if (_suspect.IsDead)
                {
                    _speakSuspect.Enabled = false;
                    _speakSuspect.SetRightLabel("~r~Dead");
                }
                if (_suspect2.IsDead)
                {
                    _speakSuspect2.Enabled = false;
                    _speakSuspect2.SetRightLabel("~r~Dead");
                }
                base.Process();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                End(true);
            }
        }

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakSuspect)
            {
                var dialog1 = new List<string>
                {
                    "~b~You~s~: What's going on? Why were you guys fighting?",
                    "~r~" + _name1 + "~s~: What does it matter to you?!",
                    "~b~You~s~: Finding out what's going on is my job.",
                    "~r~" + _name1 + "~s~: Whatever, it's not my job to tell you."
                };
                var dialog2 = new List<string>
                {
                    "~b~You~s~: What's going on? Why were you guys fighting?",
                    "~r~" + _name1 + "~s~: He started it! I was just defending myself!",
                    "~b~You~s~: What did he do to start it?",
                    "~r~" + _name1 + "~s~: He is drunk, started saying rude stuff about my cat Ruffles!",
                    "~b~You~s~: Alright, well I'll take note of that."
                };
                var dialogIndex1 = 0;
                var dialogIndex2 = 0;
                var dialogOutcome = new Random().Next(0, 101);
                var stillTalking = true;
                
                if (Player.DistanceTo(_suspect) > 5f)
                {
                    Game.DisplaySubtitle("Too far to talk!");
                    return;
                }
                
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _suspect, Game.LocalPlayer.Character, -1);
                GameFiber.StartNew(delegate {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5) stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                });
            }
            if (selItem == _speakSuspect2)
            {
                var dialog1 = new List<string>
                {
                    "~b~You~s~: Why were you two fighting? What's going on!",
                    "~r~" + _name2 + "~s~: Screw you, I hope you die!",
                    "~b~You~s~: You need to tell me what's going on.",
                    "~r~" + _name2 + "~s~: I don't need to tell you anything."
                };
                var dialog2 = new List<string>
                {
                    "~b~You~s~: Explain to me what's going on.",
                    "~r~" + _name2 + "~s~: I don't even know where I am sir.",
                    "~b~You~s~: Have you used any drugs or had anything to drink?",
                    "~r~" + _name2 + "~s~: All of it.",
                    "~b~You~s~: Alright, well I'll take note of that."
                };
                var dialogIndex1 = 0;
                var dialogIndex2 = 0;
                var dialogOutcome = new Random().Next(0, 101);
                var stillTalking = true;
                
                if (Player.DistanceTo(_suspect2) > 5f)
                {
                    Game.DisplaySubtitle("Too far to talk!");
                    return;
                }
                
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _suspect2, Game.LocalPlayer.Character, -1);
                GameFiber.StartNew(delegate {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5) stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                });
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