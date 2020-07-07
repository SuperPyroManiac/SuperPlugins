using System;
using System.Collections.Generic;
using System.Linq;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace Burning_Bait
{
    internal class Bait
    {
        internal static bool BaitRunning { get; set; }
        internal static List<Entity> EntitiesToClear { get; private set; }
        internal static List<Blip> BlipsToClear { get; private set; }
        internal GameFiber ProcessFiber { get; private set; }
        internal Ped Player => Game.LocalPlayer.Character;
        //UI Items
        internal readonly MenuPool Interaction = new MenuPool();
        internal readonly UIMenu MainMenu = new UIMenu("Burning Baits", "Choose an option.");
        internal readonly UIMenu BaitMenu = new UIMenu("Burning Bait", "~y~Choose an option.");
        internal readonly UIMenuItem StartBait = new UIMenuItem("Start Bait");
        internal readonly UIMenuItem EndCall = new UIMenuItem("~y~End Bait", "Ends the scenario.");

        internal void BaitEvent()
        {
            try
            {
                EntitiesToClear = new List<Entity>();
                BlipsToClear = new List<Blip>();
                ProcessFiber = new GameFiber(delegate
                {
                    while (BaitRunning)
                    {
                        Process();
                        GameFiber.Yield();
                    }
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("Burning Bait Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Burning Bait Error Report End");
                // ReSharper disable once VirtualMemberCallInConstructor
                End(true);
            }
        }
                internal virtual void StartEvent(Vector3 spawnPoint, float spawnPointH)
        {
            Interaction.Add(MainMenu);
            Interaction.Add(BaitMenu);
            MainMenu.AddItem(StartBait);
            MainMenu.AddItem(EndCall);
            MainMenu.BindMenuToItem(BaitMenu, StartBait);
            BaitMenu.ParentMenu = MainMenu;
            //StartBait.Enabled = false;
            MainMenu.RefreshIndex();
            BaitMenu.RefreshIndex();
            MainMenu.OnItemSelect += Interactions;
            BaitMenu.OnItemSelect += Conversations;
            BaitRunning = true;
            ProcessFiber.Start();
        }

        protected virtual void Process()
        {
            //if (Game.IsKeyDown(Settings.EndBait)) End(false);
            //if (Game.IsKeyDown(Settings.Interact)) MainMenu.Visible = !MainMenu.Visible;
            Interaction.ProcessMenus();
        }

        protected virtual void End(bool forceCleanup)
        {
            BaitRunning = false;
            
            if (forceCleanup)
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    if (entity.Exists()) entity.Delete();
                Game.LogTrivial("Burning Bait: Event has been forcefully cleaned up.");
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
            Game.LogTrivial("Burning Bait: Ending Event.");
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