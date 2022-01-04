using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts2.CustomScenes;
using SuperCallouts2.SimpleFunctions;

namespace SuperCallouts2.Callouts
{
    [CalloutInfo("Mafia1", CalloutProbability.Medium)]
    internal class Mafia1 : Callout
    {
        private Vector3 _callPos = new Vector3(909.56f, 4.041f, 78.67f);
        private Blip _cBlip;
        private Blip _aBlip;
        private SrState _state = SrState.CheckDistance;
        private SrChoice _choice;
        private static Ped Player => Game.LocalPlayer.Character;
        //Setup Scene
        private readonly List<Ped> _goodguys = new List<Ped>();
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();
        private Ped _fib1;
        private Ped _fib2;
        private Ped _fib3;
        private Ped _fib4;
        private Ped _fib5;
        private Vehicle _fibCar1;
        private Vehicle _fibCar2;
        //Raid Scene
        private readonly List<Ped> _badGuys = new List<Ped>();
        private Ped _bad1;
        private Ped _bad2;
        private Ped _bad3;
        private Ped _bad4;
        private Ped _bad5;
        private Ped _bad6;
        private Ped _bad7;
        private Ped _bad8;
        private Vehicle _badCar1;
        private Vehicle _badCar2;
        private Vehicle _badCar3;
        private Vehicle _badCar4;
        //UI Items
        private MenuPool _interaction;
        private UIMenu _mainMenu;
        private UIMenu _convoMenu;
        private UIMenuItem _questioning;
        private UIMenuItem _endCall;
        private readonly UIMenuItem _speakFib = new UIMenuItem("- Speak With FIB Agent");
        private readonly UIMenuItem _choiceNoose = new UIMenuItem("- NOOSE Team");
        private readonly UIMenuItem _choiceSwat = new UIMenuItem("- Local SWAT Team");
        private readonly UIMenuItem _choiceYou = new UIMenuItem("- Handle It Yourself");

        public override bool OnBeforeCalloutDisplayed()
        {
            ShowCalloutAreaBlipBeforeAccepting(_callPos, 80f);
            CalloutMessage = "~b~FIB Report:~s~ Raid on Mafia drug smuggling.";
            CalloutPosition = _callPos;
            Functions.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION",
                _callPos);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Setup
            Game.LogTrivial("SR Callouts Log: Mafia1 callout accepted...");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~The Mafia",
                "FIB reports the Mafia have been using the casino as a drug trafficking hotspot. Speak with FIB agents and plan a raid.");
            Mafia1Pre.BuildPreScene(out _fib1, out _fib2, out _fib3, out _fib4, out _fib5, out _fibCar1, out _fibCar2);
            
            _cBlip = new Blip(_fib1.Position);
            _cBlip.Color = Color.Yellow;
            _cBlip.EnableRoute(Color.Yellow);
            _cBlip.Alpha /= 2;
            _cBlip.Name = "Callout";
            
            _vehicles.Add(_fibCar1);
            _vehicles.Add(_fibCar2);
            _goodguys.Add(_fib1);
            _goodguys.Add(_fib2);
            _goodguys.Add(_fib3);
            _goodguys.Add(_fib4);
            _goodguys.Add(_fib5);

            foreach (var entity in _vehicles.Where(entity => entity)) entity.IsPersistent = true;
                foreach (var entity in _goodguys.Where(entity => entity))
            {
                entity.IsPersistent = true;
                entity.BlockPermanentEvents = true;
            }
            
            //UI Items
            CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
            _mainMenu.OnItemSelect += InteractionProcess;
            _convoMenu.OnItemSelect += ConversationProcess;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                switch (_state)
                {
                    case SrState.CheckDistance:
                        if (Player.DistanceTo(_fib1.Position) < 10f)
                        {
                            _cBlip.DisableRoute();
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~SR Callouts",
                                "~r~Speak With FIB",
                                "Press: " + Settings.Interact + " to speak with the FIB.");
                            NativeFunction.Natives.x5AD23D40115353AC(_fib1, Player, -1); //Turn_Ped_To_Face_Entity
                            NativeFunction.Natives.x5AD23D40115353AC(_fib2, Player, -1);
                            _questioning.Enabled = true;
                            _convoMenu.AddItem(_speakFib);
                            _state = SrState.End;
                        }
                        break;
                    case SrState.CheckDistance2:
                        if (Player.DistanceTo(_callPos) < 120f)
                        {
                            Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
                            Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
                            switch (_choice)
                            {
                                case SrChoice.Noose:
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.NooseTeam);
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.NooseTeam);
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.NooseAirUnit);
                                    _state = SrState.RaidScene;
                                    break;
                                case SrChoice.Swat:
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.SwatTeam);
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.SwatTeam);
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.AirUnit);
                                    _state = SrState.RaidScene;
                                    break;
                                case SrChoice.You:
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.LocalUnit);
                                    Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                        EBackupUnitType.LocalUnit);
                                    _state = SrState.RaidScene;
                                    break;
                                default:
                                    Game.LogTrivial("Oops there was an error here. There was an issue detecting your choice!");
                                    End();
                                    break;
                            }
                        }
                        break;
                    case SrState.RaidScene:
                        foreach (var entity in _goodguys.Where(entity => entity)) entity?.Dismiss();
                        foreach (var entity in _badGuys.Where(entity => entity)) entity.BlockPermanentEvents = false;
                        GameFiber.StartNew(delegate
                        {
                            GameFiber.Wait(5000);
                            foreach (var entity in _badGuys.Where(entity => entity))
                                entity.Tasks.FightAgainstClosestHatedTarget(100, -1);
                            _aBlip.DisableRoute();
                            _state = SrState.End;
                        });
                        break;
                    case SrState.End:
                        break;
                    default:
                        End();
                        break;
                }
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    _mainMenu.Visible = !_mainMenu.Visible;
                }
                _interaction.ProcessMenus();
                base.Process();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("SuperCallouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperCallouts Error Report End");
                End();
            }
        }

        public override void End()
        {
            _cBlip?.Delete();
            _aBlip?.Delete();
            if (_aBlip.Exists()) _aBlip.Delete();
            foreach (var entity in _badGuys.Where(entity => entity)) entity?.Dismiss();
            foreach (var entity in _goodguys.Where(entity => entity)) entity?.Dismiss();
            foreach (var entity in _vehicles.Where(entity => entity)) entity?.Dismiss();
            Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Dislike);
            _interaction.CloseAllMenus();
            Game.DisplayHelp("Scene ~g~CODE 4", 5000);
            BigMessageThread bigMessage = new BigMessageThread();
            bigMessage.MessageInstance.ShowColoredShard("Code 4", "Callout Ended", HudColor.Green, HudColor.Black,
                2);
                base.End();
        }
        
        //UI Functions
        private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }

        private void ConversationProcess(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _speakFib)
            {
                GameFiber.StartNew(delegate
                {
                    if (Player.DistanceTo(_fib1) > 4f)
                    {
                        Game.DisplaySubtitle("~r~Get closer to talk!");
                        return;
                    }

                    //_convoMenu.RemoveItemAt(0);
                    Game.DisplaySubtitle("~b~Agent~s~: Hello sergeant, you may be aware of the current crime family in the city.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: In the past we didn't have enough evidence to convict them but they slipped up this week.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: We have intel on them transporting weapons and drugs using the casino as a base of operations.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: So today you will be leading a raid on the casino. There is a few different ways we can handle this.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: We have NOOSE available to help 'subdue' the suspects. You may also lead a SWAT team from your department.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: Last option is we leave the situation under your control. You can bring your own backup, but this seems dangerous.", 7000);
                    GameFiber.Wait(7000);
                    Game.DisplaySubtitle("~b~Agent~s~: Let me know what option sounds good to you.", 6000);
                    _convoMenu.AddItem(_choiceNoose);
                    _convoMenu.AddItem(_choiceSwat);
                    _convoMenu.AddItem(_choiceYou);
                    _convoMenu.RefreshIndex();
                });
            }
            if (selItem == _choiceNoose)
            {
                GameFiber.StartNew(delegate
                {
                    _interaction.CloseAllMenus();
                    _questioning.Enabled = false;
                    Game.DisplaySubtitle("~b~Agent~s~: We will have a NOOSE team on standby until you arrive on scene.", 6000);
                    _cBlip?.Delete();
                    _aBlip = new Blip(_callPos.Around2D(1,2), 30);
                    _aBlip.Color = Color.Red;
                    _aBlip.Alpha = .5f;
                    _aBlip.EnableRoute(Color.Red);
                    LoadRaid();
                    _choice = SrChoice.Noose;
                    _state = SrState.CheckDistance2;
                });
            }
            if (selItem == _choiceSwat)
            {
                GameFiber.StartNew(delegate
                {
                    _interaction.CloseAllMenus();
                    _questioning.Enabled = false;
                    Game.DisplaySubtitle("~b~Agent~s~: Your departments SWAT team will standby for your arrival.", 6000);
                    _cBlip?.Delete();
                    _aBlip = new Blip(_callPos.Around2D(1,2), 30);
                    _aBlip.Color = Color.Red;
                    _aBlip.Alpha = .5f;
                    _aBlip.EnableRoute(Color.Red);
                    LoadRaid();
                    _choice = SrChoice.Swat;
                    _state = SrState.CheckDistance2;
                });
            }
            if (selItem == _choiceYou)
            {
                GameFiber.StartNew(delegate
                {
                    _interaction.CloseAllMenus();
                    _questioning.Enabled = false;
                    Game.DisplaySubtitle("~b~Agent~s~: We will leave it to you then. Seems like a dangerous choice though.", 6000);
                    _cBlip?.Delete();
                    _aBlip = new Blip(_callPos.Around2D(1,2), 30);
                    _aBlip.Color = Color.Red;
                    _aBlip.Alpha = .5f;
                    _aBlip.EnableRoute(Color.Red);
                    LoadRaid();
                    _choice = SrChoice.You;
                    _state = SrState.CheckDistance2;
                });
            }
        }

        private void LoadRaid()
        {
            CustomScenes.Mafia1.BuildScene(out _bad1, out _bad2, out _bad3, out _bad4, out _bad5, out _bad6, out _bad7, out _bad8, out _badCar1, out _badCar2, out _badCar3, out _badCar4);
            _vehicles.Add(_badCar1);
            _vehicles.Add(_badCar2);
            _vehicles.Add(_badCar3);
            _vehicles.Add(_badCar4);
            _badGuys.Add(_bad1);
            _badGuys.Add(_bad2);
            _badGuys.Add(_bad3);
            _badGuys.Add(_bad4);
            _badGuys.Add(_bad5);
            _badGuys.Add(_bad6);
            _badGuys.Add(_bad7);
            _badGuys.Add(_bad8);
            foreach (var entity in _badGuys.Where(entity => entity))
            {
                entity.IsPersistent = true;
                entity.Inventory.Weapons.Add(WeaponHash.AdvancedRifle);
                entity.BlockPermanentEvents = true;
                CFunctions.SetWanted(entity, true);
            }
            foreach (var entity in _vehicles.Where(entity => entity))
            {
                if (entity)
                {
                    entity.IsPersistent = true;
                    entity.Metadata.searchTrunk =
                        "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
                }
            }
        }

        private enum SrState
        {
            CheckDistance,
            CheckDistance2,
            RaidScene,
            End
        }
        private enum SrChoice
        {
            Noose,
            Swat,
            You
        }
    }
}