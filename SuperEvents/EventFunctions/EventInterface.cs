using System;
using System.Linq;
using System.Windows.Forms;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
// ReSharper disable IdentifierTypo

namespace SuperEvents.EventFunctions;

internal static class EventInterface
{
    private static readonly MenuPool eventMenuPool = new();
    private static readonly UIMenu MainMenu = new("SuperEvents", "Choose an option.");
    private static readonly UIMenu EventMenu = new("SuperEvents", "Choose an event to spawn.");
    private static readonly UIMenuItem EventList = new("Force Event", "Spawn any selected event.");
    private static readonly UIMenuItem PauseEvent = new("~y~Pause Events", "Pause events from spawning. Force Event bypasses this.");
    private static readonly UIMenuItem EndEvent = new("~r~End Event", "Ends the current event.");

    internal static void StartInterface()
    {
        try
        {
            GameFiber.StartNew(delegate
            {
                //Wait for 10 seconds before creating the menus in case events are not yet registered.
                GameFiber.Wait(10000);
                SetupUI();
                while (Main.PluginRunning)
                {
                    Process();
                    GameFiber.Yield();
                }
            });
        }
        catch (Exception e) { Log.Error(e.ToString()); }
    }

    private static void SetupUI()
    {
        //Add menus to pool
        eventMenuPool.Add(MainMenu);
        eventMenuPool.Add(EventMenu);
        //Fix controls being weird in game
        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;
        EventMenu.MouseControlsEnabled = false;
        EventMenu.AllowCameraMovement = true;
        //Add buttons to menus - in order!
        MainMenu.AddItem(EventList);
        MainMenu.AddItem(PauseEvent);
        MainMenu.AddItem(EndEvent);
        //Bind the menu to a button
        MainMenu.BindMenuToItem(EventMenu, EventList);
        //Refresh the index of the menus
        MainMenu.RefreshIndex();
        EventMenu.RefreshIndex();
        //Add events to the menus!
        MainMenu.OnItemSelect += MainMenuSelected;
        //Create event buttons in EventMenu
        foreach (var t in EventManager.AllEvents)
        {
            var s = new UIMenuItem($"[{t.Namespace.Split('.').First()}] {t.Name}");
            EventMenu.AddItem(s);
            s.Activated += (_,_) => EventManager.ForceEvent(t.FullName);
        }
    }
    
    private static void Process()
    {
        if (Game.IsKeyDown(Settings.EventManager)) MainMenu.Visible = !MainMenu.Visible;
        eventMenuPool.ProcessMenus();
    }
    
    private static void MainMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        if (selecteditem == PauseEvent) Main.PausePlugin();
        if (selecteditem == EndEvent) EventManager.CurrentEvent?.EndEvent();
    }
}