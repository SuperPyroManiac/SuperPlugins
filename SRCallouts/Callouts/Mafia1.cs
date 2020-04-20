using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SRCallouts.SceneSetup;
using SRCallouts.SimpleFunctions;

namespace SRCallouts.Callouts
{
    [CalloutInfo("Mafia1", CalloutProbability.Medium)]
    public class Mafia1 : Callout
    {
        private readonly Vector3 _callPos = new Vector3(918.88f, 38.91f, 81.09f);
        private Blip _cBlip;
        private SRState _state = SRState.checkDistance;
        private Ped Player => Game.LocalPlayer.Character;
        //Setup Scene
        public List<Ped> Goodguys;
        public List<Vehicle> Vehicles;
        private Ped _fib1;
        private Ped _fib2;
        private Ped _fib3;
        private Ped _fib4;
        private Ped _fib5;
        private Vehicle _fibCar1;
        private Vehicle _fibCar2;
        //Raid Scene
        public List<Ped> BadGuys;
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
        private MenuPool Interaction;
        private UIMenu MainMenu;
        private UIMenu ConvoMenu;
        private UIMenuItem Questioning;
        private UIMenuItem EndCall;
        private UIMenuItem SpeakFIB = new UIMenuItem("Speak With FIB Agent");

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
            
            Vehicles.Add(_fibCar1);
            Vehicles.Add(_fibCar2);
            Vehicles.Add(_badCar1);
            Vehicles.Add(_badCar2);
            Vehicles.Add(_badCar3);
            Vehicles.Add(_badCar4);
            Goodguys.Add(_fib1);
            Goodguys.Add(_fib2);
            Goodguys.Add(_fib3);
            Goodguys.Add(_fib4);
            Goodguys.Add(_fib5);
            BadGuys.Add(_bad1);
            BadGuys.Add(_bad2);
            BadGuys.Add(_bad3);
            BadGuys.Add(_bad4);
            BadGuys.Add(_bad5);
            BadGuys.Add(_bad6);
            BadGuys.Add(_bad7);
            BadGuys.Add(_bad8);
            
            Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
            
            foreach (var entity in Vehicles.Where(entity => entity))
            {
                entity.IsPersistent = true;
                entity.Metadata.searchTrunk = "~r~multiple pallets of cocain~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
            }
            foreach (var entity in Goodguys.Where(entity => entity))
            {
                entity.IsPersistent = true;
                entity.BlockPermanentEvents = true;
            }

            foreach (var entity in BadGuys.Where(entity => entity))
            {
                entity.IsPersistent = true;
                entity.Inventory.Weapons.Add(WeaponHash.AdvancedRifle);
                SFunctions.SetWanted(entity, true);
            }
            //UI Items
            SFunctions.BuildUi(out Interaction, out MainMenu, out ConvoMenu, out Questioning, out EndCall);
            MainMenu.OnItemSelect += InteractionProcess;
            ConvoMenu.OnItemSelect += ConversationProcess;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            try
            {
                switch (_state)
                {
                    case SRState.checkDistance:
                        if (Player.DistanceTo(_fib1.Position) < 10f)
                        {
                            _cBlip.DisableRoute();
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~SR Callouts",
                                "~r~Speak With FIB",
                                "Press: " + Settings.Interact + " to speak with the FIB.");
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _fib1, Player, -1);
                            NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _fib2, Player, -1);
                            _state = SRState.fibScene;
                        }
                        break;
                    case SRState.fibScene:
                        break;
                    case SRState.checkDistance2:
                        break;
                    case SRState.raidScene:
                        break;
                    case SRState.End:
                        break;
                    default:
                        End();
                        break;
                }
                //Keybinds
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.IsKeyDown(Settings.Interact))
                {
                    MainMenu.Visible = !MainMenu.Visible;
                }
                Interaction.ProcessMenus();
                base.Process();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("SR Callouts Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SR Callouts Error Report End");
                End();
            }
        }
        
        public override void End()
        {
            base.End();
        }
        
        //UI Functions
        private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == EndCall)
            {
                Game.DisplaySubtitle("~y~Callout Ended.");
                End();
            }
        }

        private void ConversationProcess(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == SpeakFIB)
            {
                GameFiber.StartNew(delegate
                {
                    ConvoMenu.RemoveItemAt(0);
                    Game.DisplaySubtitle("Sup sergeant");
                });
            }
        }

        private enum SRState
        {
            checkDistance,
            fibScene,
            checkDistance2,
            raidScene,
            End
        }
    }
}