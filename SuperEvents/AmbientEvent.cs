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
        private readonly UIMenuItem _endCall = new("~y~End Event", "Ends the event.");
        private readonly MenuPool _interaction = new();
        private readonly UIMenu _mainMenu = new("SuperEvents", "Choose an option.");
        internal readonly UIMenuItem Questioning = new("Speak With Subjects");
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
        protected Vector3 EventLocation { get; set; }
        internal static bool TimeStart { get; set; }
        internal static List<Entity> EntitiesToClear { get; private set; }
        internal static List<Blip> BlipsToClear { get; private set; }
        private GameFiber ProcessFiber { get; }
        internal static Ped Player => Game.LocalPlayer.Character;

        protected virtual void StartEvent()
        {
            TimeStart = false;
            _interaction.Add(_mainMenu);
            _interaction.Add(ConvoMenu);
            _mainMenu.MouseControlsEnabled = false;
            _mainMenu.AllowCameraMovement = true;
            ConvoMenu.MouseControlsEnabled = false;
            ConvoMenu.AllowCameraMovement = true;
            _mainMenu.AddItem(Questioning);
            _mainMenu.AddItem(_endCall);
            _mainMenu.BindMenuToItem(ConvoMenu, Questioning);
            ConvoMenu.ParentMenu = _mainMenu;
            Questioning.Enabled = false;
            _mainMenu.RefreshIndex();
            ConvoMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
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
            EventRunning = true;
            ProcessFiber.Start();
        }

        protected virtual void Process()
        {
            if (Game.IsKeyDown(Settings.EndEvent)) End(false);
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            if (EventLocation.DistanceTo(Player) > 200f)
            {
                End(false);
                Game.LogTrivial("SuperEvents: Ending event due to player being too far.");
            }

            _interaction.ProcessMenus();
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

            _interaction.CloseAllMenus();
            Game.LogTrivial("SuperEvents: Ending Event.");
            EventTimer.TimerStart();
        }

        protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall) End(false);
        }

        protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
        }
    }
}