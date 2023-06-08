#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

#endregion

namespace PyroCommon.Events
{
    public abstract class AmbientEvent
    {
        internal bool HasEnded { get; set; }
        internal bool ShowBlips { get; set; }
        internal bool ShowHints { get; set; }
        internal Keys EndEvent { get; set; }
        internal Keys Interact { get; set; }
        protected readonly UIMenu ConvoMenu = new("SuperEvents", "~y~Choose a subject to speak with.");
        protected readonly UIMenuItem EndCall = new("~y~End Event", "Ends the event.");
        protected readonly MenuPool Interaction = new();
        protected readonly UIMenu MainMenu = new("SuperEvents", "Choose an option.");
        protected readonly UIMenuItem Questioning = new("Speak With Subjects");
        public static bool EventRunning { get; internal set; }
        protected Vector3 EventLocation { get; set; }
        protected float OnSceneDistance { get; set; } = 20;
        protected string EventTitle { get; set; }
        protected string EventDescription { get; set; }
        public static List<Entity> EntitiesToClear { get; private set; }
        public static List<Blip> BlipsToClear { get; private set; }
        private GameFiber ProcessFiber { get; }
        protected static Ped Player => Game.LocalPlayer.Character;
        private bool onScene;
        
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

        protected internal virtual void StartEvent()
        {
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
            if (ShowBlips)
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

        protected internal virtual void OnScene()
        {
        }

        protected internal virtual void Process()
        {
            if (Game.IsKeyDown(EndEvent)) End(false);
            if (Game.IsKeyDown(Interact)) MainMenu.Visible = !MainMenu.Visible;
            if (EventLocation.DistanceTo(Player) > 200f)
            {
                End(false);
                Game.LogTrivial("SuperEvents: Ending event due to player being too far.");
            }
            if (!onScene && Game.LocalPlayer.Character.DistanceTo(EventLocation) < OnSceneDistance)
            {
                onScene = true;
                if (ShowHints)
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                        "~r~" + EventTitle, EventDescription);
                Game.DisplayHelp("~y~Press ~r~" + Interact + "~y~ to open interaction menu.");
                OnScene();
            }

            Interaction.ProcessMenus();
        }

        protected internal virtual void End(bool forceCleanup = false)
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
            HasEnded = true; //TODO: DONT FORGET THIS THINGY IN SE
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