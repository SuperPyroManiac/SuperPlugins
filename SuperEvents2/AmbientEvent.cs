#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents2.SimpleFunctions;
#endregion

namespace SuperEvents2
{
    public class AmbientEvent
    {
        public static bool EventRunning { get; set; }
        public static bool TimeStart { get; set; }
        public static List<Entity> EntitiesToClear { get; private set; }
        public static List<Blip> BlipsToClear { get; private set; }
        public GameFiber ProcessFiber { get; }
        public Ped Player => Game.LocalPlayer.Character;
        private Vector3 CheckDistance;
        
        //Main Menu
        internal MenuPool _interaction = new MenuPool();
        internal UIMenu _mainMenu = new UIMenu("SuperEvents", "Choose an option.");
        internal UIMenu _convoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        internal UIMenuItem _questioning = new UIMenuItem("Speak With Subjects");
        internal UIMenuItem _endCall = new UIMenuItem("~y~End Event", "Ends the event.");

        protected AmbientEvent()
        {
            try
            {
                EntitiesToClear = new List<Entity>();
                BlipsToClear = new List<Blip>();
                ProcessFiber = new GameFiber(delegate
                {
                    while (EventRunning)
                    {
                        Process();
                        GameFiber.Yield();
                    }
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                // ReSharper disable once VirtualMemberCallInConstructor
                End(true);
            }
        }
        
        public virtual void StartEvent(Vector3 spawnPoint, float spawnPointH)
        {
            AmbientEvent.TimeStart = false;
            _interaction.Add(_mainMenu);
            _interaction.Add(_convoMenu);
            _mainMenu.AddItem(_questioning);
            _mainMenu.AddItem(_endCall);
            _mainMenu.BindMenuToItem(_convoMenu, _questioning);
            _convoMenu.ParentMenu = _mainMenu;
            _questioning.Enabled = false;
            _mainMenu.RefreshIndex();
            _convoMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            _convoMenu.OnItemSelect += Conversations;
            if (Settings.ShowBlips)
            {
                var eventBlip = new Blip(spawnPoint, 15f);
                eventBlip.Color = Color.Red;
                eventBlip.Alpha /= 2;
                eventBlip.Name = "Event";
                eventBlip.Flash(500, 5000);
                BlipsToClear.Add(eventBlip);
            }
            CheckDistance = spawnPoint;
            EventRunning = true;
            ProcessFiber.Start();
        }

        protected virtual void Process()
        {
            if (Game.IsKeyDown(Settings.EndEvent)) End(false);
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            if (CheckDistance.DistanceTo(Player) > 200f) End(false);
            _interaction.ProcessMenus();
        }

        protected virtual void End(bool forceCleanup)
        {
            EventRunning = false;
            
            if (forceCleanup)
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    entity.Delete();
                Game.LogTrivial("Due to an issue this event has been forcefully removed!");
            }
            else
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    entity.Dismiss(); 
                Game.DisplayHelp("~y~Event Ended.");
            }
            
            foreach (var blip in BlipsToClear.Where(blip => blip))
                blip.Delete();
            
            _interaction.CloseAllMenus();
            Game.LogTrivial("SuperEvents: Ending Event.");
            EventTimer.TimerStart();
        }

        protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                End(false);
            }
        }

        protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
        }
    }
}