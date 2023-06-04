#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.EventFunctions;

#endregion

namespace SuperEvents
{
    public class AmbientEvent
    {
        internal readonly UIMenu ConvoMenu = new("SuperEvents", "~y~Choose a subject to speak with.");
        private readonly UIMenuItem EndCall = new("~y~End Event", "Ends the event.");
        private readonly MenuPool Interaction = new();
        private readonly UIMenu MainMenu = new("SuperEvents", "Choose an option.");
        internal readonly UIMenuItem Questioning = new("Speak With Subjects");
        private Vector3 _checkDistance;

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
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                // ReSharper disable once VirtualMemberCallInConstructor
                End(true);
            }
        }

        protected static bool EventRunning { get; private set; }
        public Vector3 EventLocation { get; set; }
        internal static bool TimeStart { get; set; }
        internal static List<Entity> EntitiesToClear { get; private set; }
        internal static List<Blip> BlipsToClear { get; private set; }
        private GameFiber ProcessFiber { get; }
        internal static Ped Player => Game.LocalPlayer.Character;

        protected virtual void StartEvent()
        {
            TimeStart = false;
            Interaction.Add(MainMenu);
            Interaction.Add(ConvoMenu);
            MainMenu.MouseControlsEnabled = false;
            MainMenu.AllowCameraMovement = true;
            ConvoMenu.MouseControlsEnabled = false;
            ConvoMenu.AllowCameraMovement = true;
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
                var eventBlip = new Blip(EventLocation, 15f);
                eventBlip.Color = Color.Red;
                eventBlip.Alpha /= 2;
                eventBlip.Name = "Event";
                eventBlip.Flash(500, 8000);
                BlipsToClear.Add(eventBlip);
            }

            _checkDistance = EventLocation;
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
                    if (entity.Exists())
                        entity.Delete();
                Game.LogTrivial("SuperEvents: Event has been forcefully cleaned up.");
            }
            else
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    if (entity.Exists())
                        entity.Dismiss();
                Game.DisplayHelp("~y~Event Ended.");
            }

            foreach (var blip in BlipsToClear.Where(blip => blip))
                if (blip.Exists())
                    blip.Delete();

            Interaction.CloseAllMenus();
            Game.LogTrivial("SuperEvents: Ending Event.");
            //ProcessFiber.Abort();
            EventTimer.TimerStart();
        }

        protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == EndCall) End(false);
        }

        protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
        }
    }
}