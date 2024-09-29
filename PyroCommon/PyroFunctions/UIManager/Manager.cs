using System.Linq;
using LSPD_First_Response.Mod.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace PyroCommon.PyroFunctions.UIManager;

internal static class Manager
{
    private static bool _running;
    private static readonly MenuPool MainMenuPool = new();
    private static readonly UIMenu MainMenu = new("SuperPlugins", "Choose an option.");
    private static readonly UIMenu CalloutMenu = new("Callouts", "Choose a callout to spawn.");
    private static readonly UIMenu EventMenu = new("Events", "Choose an event to spawn.");
    private static readonly UIMenuItem CalloutList = new("Force Callout", "Spawn any selected callout.");
    private static readonly UIMenuItem EndCallout = new("~r~End Callout", "Ends the current callout.");
    private static readonly UIMenuItem EventList = new("Force Event", "Spawn any selected event.");
    private static readonly UIMenuItem PauseEvent = new("~y~Pause Events", "Pause events from spawning. Force Event bypasses this.");
    private static readonly UIMenuItem EndEvent = new("~r~End Event", "Ends the current event.");

    private static void Run()
    {
        _running = true;
        StartUi();
    }

    private static void StartUi()
    {
        MainMenuPool.Add(MainMenu);
        MainMenuPool.Add(CalloutMenu);
        MainMenuPool.Add(EventMenu);
        MainMenu.MouseControlsEnabled = false;
        MainMenu.AllowCameraMovement = true;
        CalloutMenu.MouseControlsEnabled = false;
        CalloutMenu.AllowCameraMovement = true;
        EventMenu.MouseControlsEnabled = false;
        EventMenu.AllowCameraMovement = true;
        MainMenu.AddItem(CalloutList);
        MainMenu.AddItem(EventList);
        MainMenu.AddItem(PauseEvent);
        MainMenu.AddItem(EndCallout);
        MainMenu.AddItem(EndEvent);
        MainMenu.BindMenuToItem(CalloutMenu, CalloutList);
        MainMenu.BindMenuToItem(EventMenu, EventList);
        MainMenu.RefreshIndex();
        CalloutMenu.RefreshIndex();
        EventMenu.RefreshIndex();
        MainMenu.OnItemSelect += MainMenuSelected;
        CalloutList.Enabled = false;
        EndCallout.Enabled = false;
        EventList.Enabled = false;
        EndEvent.Enabled = false;
        PauseEvent.Enabled = false;
        
        if ( Main.UsingSc )
        {
            CalloutList.Enabled = true;
            EndCallout.Enabled = true;
            foreach (var t in PyroFunctions.RegisteredCallouts.Where(x => x.Name.Contains("[SC] ")))
            {
                var s = new UIMenuItem(t.Name);
                EventMenu.AddItem(s);
                s.Activated += (_, _) => Functions.StartCallout(t.Name);
            }
        }
        
        if ( Main.UsingSe )
        {
            EventList.Enabled = true;
            EndEvent.Enabled = true;
            PauseEvent.Enabled = true;
            foreach (var t in Wrappers.SuperEvents.GetAllEvents())
            {
                var s = new UIMenuItem($"[{t.Namespace!.Split('.').First()}] {t.Name}");
                EventMenu.AddItem(s);
                s.Activated += (_,_) => Wrappers.SuperEvents.ForceEvent(t.FullName);
            }
        }
        GameFiber.StartNew(Process);
    }

    internal static void ToggleMenu()
    {
        MainMenu.Visible = !MainMenu.Visible;
    }

    private static void Process()
    {
        while ( _running )
        {
            GameFiber.Yield();
            MainMenuPool.ProcessMenus();
        }
    }
    
    private static void MainMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        if (selecteditem == PauseEvent) Wrappers.SuperEvents.PauseEvents();
        if (selecteditem == EndCallout) Functions.StopCurrentCallout();
        if (selecteditem == EndEvent) Wrappers.SuperEvents.EndEvent();
    }
}