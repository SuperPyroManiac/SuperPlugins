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
    internal class AmbientEvent
    {
        internal static bool EventRunning { get; set; }
        internal static bool TimeStart { get; set; }
        internal static List<Entity> EntitiesToClear { get; private set; }
        internal static List<Blip> BlipsToClear { get; private set; }
        internal GameFiber ProcessFiber { get; }
        internal Ped Player => Game.LocalPlayer.Character;
        private Vector3 _checkDistance;
        
        //Main Menu
        internal readonly MenuPool Interaction = new MenuPool();
        internal readonly UIMenu MainMenu = new UIMenu("SuperEvents", "Choose an option.");
        internal readonly UIMenu ConvoMenu = new UIMenu("SuperEvents", "~y~Choose a subject to speak with.");
        internal readonly UIMenuItem Questioning = new UIMenuItem("Speak With Subjects");
        internal readonly UIMenuItem EndCall = new UIMenuItem("~y~End Event", "Ends the event.");

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
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                // ReSharper disable once VirtualMemberCallInConstructor
                End(true);
            }
        }

        internal virtual void StartEvent(Vector3 spawnPoint, float spawnPointH)
        {
            AmbientEvent.TimeStart = false;
            Interaction.Add(MainMenu);
            Interaction.Add(ConvoMenu);
            MainMenu.MouseControlsEnabled = false;
            MainMenu.AllowCameraMovement = true;
            MainMenu.AddItem(Questioning);
            MainMenu.AddItem(EndCall);
            MainMenu.BindMenuToItem(ConvoMenu, Questioning);
            ConvoMenu.ParentMenu = MainMenu;
            Questioning.Enabled = false;
            MainMenu.RefreshIndex();
            ConvoMenu.RefreshIndex();
            MainMenu.OnItemSelect += Interactions;
            ConvoMenu.OnItemSelect += Conversations;
            if (Settings.ShowBlips)
            {
                var eventBlip = new Blip(spawnPoint, 15f);
                eventBlip.Color = Color.Red;
                eventBlip.Alpha /= 2;
                eventBlip.Name = "Event";
                eventBlip.Flash(500, 8000);
                BlipsToClear.Add(eventBlip);
            }
            _checkDistance = spawnPoint;
            EventRunning = true;
            ProcessFiber.Start();
        }

        protected virtual void Process()
        {
            if (Game.IsKeyDown(Settings.EndEvent)) End(false);
            if (Game.IsKeyDown(Settings.Interact)) MainMenu.Visible = !MainMenu.Visible;
            if (_checkDistance.DistanceTo(Player) > 200f)
            {
                End(false);
                Game.LogTrivial("SuperEvents: Ending event due to player being too far.");
            }
            Interaction.ProcessMenus();
        }

        protected virtual void End(bool forceCleanup)
        {
            EventRunning = false;
            
            if (forceCleanup)
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    if (entity.Exists()) entity.Delete();
                Game.LogTrivial("SuperEvents: Event has been forcefully cleaned up.");
            }
            else
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    if (entity.Exists()) entity.Dismiss(); 
                Game.DisplayHelp("~y~Event Ended.");
            }
            
            foreach (var blip in BlipsToClear.Where(blip => blip))
                if (blip.Exists()) blip.Delete();
            
            Interaction.CloseAllMenus();
            Game.LogTrivial("SuperEvents: Ending Event.");
            //ProcessFiber.Abort();
            EventTimer.TimerStart();
        }

        protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == EndCall)
            {
                End(false);
            }
        }

        protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
        }
    }
}