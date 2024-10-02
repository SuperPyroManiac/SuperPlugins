using System.Drawing;
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
    private static readonly UIMenu MainMenu = new("Pyro Plugins", "                 By SuperPyroManiac");
    
    private static readonly UIMenu FirstMenu = new("Pyro Plugins", "                 By SuperPyroManiac");
    private static readonly UIMenuCheckboxItem UpdateNotifications = new("~y~Update Notifications", Settings.UpdateNotifications);
    private static readonly UIMenuCheckboxItem ErrorReporting = new("~y~Error Reporting", Settings.ErrorReporting);
    private static readonly UIMenuItem SaveButton = new("~r~Save Settings", "Saves PyroCommon.ini");
    
    private static readonly UIMenu CalloutMenu = new("Callouts", "Choose a callout to spawn.");
    private static readonly UIMenuItem CalloutConfig = new("Config", "Configure SC Settings.");
    private static readonly UIMenuItem CalloutList = new("Force Callout", "Spawn any selected callout.");
    private static readonly UIMenuItem EndCallout = new("~r~End Callout", "Ends the current callout.");
    
    private static readonly UIMenu EventMenu = new("Events", "Choose an event to spawn.");
    private static readonly UIMenuItem EventConfig = new("Config", "Configure SE Settings.");
    private static readonly UIMenuItem EventList = new("Force Event", "Spawn any selected event.");
    private static readonly UIMenuCheckboxItem PauseEvent = new("~y~Pause Events", Main.EventsPaused);
    private static readonly UIMenuItem EndEvent = new("~r~End Event", "Ends the current event.");
    
    internal static readonly UIMenu SeMenu = new("SuperEvents", "Choose an option.");
    internal static readonly UIMenu ScMenu = new("SuperCallouts", "Choose an option.");
    private static readonly UIMenuItem DwConfig = new("Config", "Configure DW Settings.");

    internal static void StartUi()
    {
        _running = true;
        MainMenuPool.Add(MainMenu);
        MainMenuPool.Add(FirstMenu);
        MainMenuPool.Add(CalloutMenu);
        MainMenuPool.Add(EventMenu);
        MainMenuPool.Add(SeMenu);
        MainMenuPool.Add(ScMenu);
        MainMenu.AddItems(
            Extras.UiSeparator(Extras.CenterText(MainMenu, "Installed Plugins")), 
            Extras.SuperCallouts(), Extras.SuperEvents(), Extras.DeadlyWeapons(),
            Extras.UiSeparator(Extras.CenterText(MainMenu, "SuperCallouts")), 
            CalloutConfig, CalloutList, EndCallout, 
            Extras.UiSeparator(Extras.CenterText(MainMenu, "SuperEvents")), 
            EventConfig, EventList, PauseEvent, EndEvent, 
            Extras.UiSeparator(Extras.CenterText(MainMenu, "DeadlyWeapons")),
            DwConfig);
        MainMenu.BindMenuToItem(CalloutMenu, CalloutList);
        MainMenu.BindMenuToItem(EventMenu, EventList);
        
        MainMenu.RefreshIndex();
        CalloutMenu.RefreshIndex();
        EventMenu.RefreshIndex();
        SeMenu.RefreshIndex();
        ScMenu.RefreshIndex();
        
        MainMenu.OnItemSelect += MainMenuSelected;
        CalloutConfig.Enabled = false;
        EventConfig.Enabled = false;
        DwConfig.Enabled = false;
        
        if ( !Main.UsingSc )
        {
            CalloutConfig.RightLabel = "Not Installed!";
            CalloutConfig.Skipped = true;
            CalloutList.Skipped = true;
            EndCallout.Skipped = true;
        }
        
        if ( !Main.UsingSe )
        {
            EventConfig.RightLabel = "Not Installed!";
            EventConfig.Skipped = true;
            EventList.Skipped = true;
            EndEvent.Skipped = true;
            PauseEvent.Skipped = true;
        }
        
        if ( !Main.UsingDw )
        {
            DwConfig.RightLabel = "Not Installed!";
            DwConfig.Skipped = true;
        }
        GameFiber.StartNew(Process);
        if (Settings.FirstTime) FirstRun();
    }

    private static void FirstRun()
    {
        FirstMenu.AddItems(
            Extras.UiSeparator(Extras.CenterText(FirstMenu, "Installed Plugins")),
            Extras.SuperCallouts(), Extras.SuperEvents(), Extras.DeadlyWeapons(),
            Extras.UiSeparator(Extras.CenterText(FirstMenu, "First Time Setup")),
            UpdateNotifications, ErrorReporting,
            Extras.UiSeparator(Extras.CenterText(FirstMenu, "Saves PyroCommon.ini")),
            SaveButton);
        FirstMenu.RefreshIndex();
        FirstMenu.OnItemSelect += FirstMenuSelected;
        UpdateNotifications.Checked = Settings.UpdateNotifications;
        UpdateNotifications.Description = "Shows update notifications on startup.";
        ErrorReporting.Checked = Settings.ErrorReporting;
        ErrorReporting.Description = "Reports errors automatically to help better my plugins. No personal data is shared!";
        RefreshMenus();
        FirstMenu.Visible = true;
        Settings.FirstTime = false;
        Settings.SaveSettings();
    }

    private static void RefreshMenus()
    {
        CalloutMenu.Clear();
        EventMenu.Clear();
        PauseEvent.Checked = Main.EventsPaused;
        if ( Main.UsingSc )
        {
            foreach (var t in PyroFunctions.RegisteredScCallouts)
            {
                var s = new UIMenuItem(t.Name);
                CalloutMenu.AddItem(s);
                s.Activated += (_, _) => Functions.StartCallout(t.Name);
            }
        }
        if ( Main.UsingSe )
        {
            foreach (var t in Wrappers.SuperEvents.GetAllEvents())
            {
                var s = new UIMenuItem($"[{t.Namespace!.Split('.').First()}] {t.Name}");
                EventMenu.AddItem(s);
                s.Activated += (_,_) => Wrappers.SuperEvents.ForceEvent(t.FullName);
            }
        }
        foreach ( var men in MainMenuPool )
        {
            men.SetBannerType(Color.FromArgb(240, 0, 0, 15));
            men.TitleStyle = MainMenu.TitleStyle with
            {
                Color = Color.DarkGoldenrod,
                Font = TextFont.ChaletComprimeCologne,
                DropShadow = true,
                Outline = true
            };
            men.MouseControlsEnabled = false;
            men.AllowCameraMovement = true;
            men.MaxItemsOnScreen = 20;
            var cnt = men.MenuItems.Count;
            if ( cnt > 20 ) cnt = 20;
            men.Offset = new Point((int)((1920 / 2f) - (men.Width / 2f)), (int)((1080 / 2f) - ((cnt * 38f) + 107f + 20f) / 2));
        }
    }

    internal static void ToggleManagerMenu()
    {
        RefreshMenus();
        MainMenu.Visible = !MainMenu.Visible;
    }

    private static void Process()
    {
        while ( _running )
        {
            GameFiber.Yield();
            MainMenuPool.ProcessMenus();
            if (Game.IsKeyDown(Settings.Manager)) ToggleManagerMenu();
        }
    }

    internal static void StopUi()
    {
        MainMenuPool.CloseAllMenus();
        MainMenuPool.Clear();
        MainMenu.Clear();
        CalloutMenu.Clear();
        EventMenu.Clear();
        _running = false;
    }
    
    private static void MainMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        if ( selecteditem == PauseEvent )
        {
            Wrappers.SuperEvents.PauseEvents();
            PauseEvent.Checked = !Main.EventsPaused;
        }
        if (selecteditem == EndCallout) Functions.StopCurrentCallout();
        if (selecteditem == EndEvent) Wrappers.SuperEvents.EndEvent();
    }
    
    private static void FirstMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        if ( selecteditem == UpdateNotifications ) Settings.UpdateNotifications = !UpdateNotifications.Checked;
        if ( selecteditem == ErrorReporting ) Settings.ErrorReporting = !ErrorReporting.Checked;
        if (selecteditem == SaveButton)
        {
            Settings.SaveSettings();
            FirstMenu.Visible = false;
        }
    }
}