using System.Linq;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using PyroCommon.Wrappers;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace PyroCommon.UIManager;

internal static class Manager
{
    private static bool _running;
    internal static readonly MenuPool MainMenuPool = new();
    private static readonly UIMenu MainMenu = new("Pyro Plugins", "                 By SuperPyroManiac");
    
    private static readonly UIMenu FirstMenu = new("Pyro Plugins", "                 By SuperPyroManiac");
    private static readonly UIMenuCheckboxItem UpdateNotifications = new("~y~Update Notifications", Settings.UpdateNotifications);
    private static readonly UIMenuCheckboxItem ErrorReporting = new("~y~Error Reporting", Settings.ErrorReporting);
    private static readonly UIMenuItem ManagerKey = new("~y~Menu Key", "Key used to open the main menu!");
    private static readonly UIMenuItem SaveButton = new("~r~Save Settings", "Saves PyroCommon.ini");
    
    private static readonly UIMenu CalloutMenu = new("Callouts", "Choose a callout to spawn.");
    private static readonly UIMenuItem ScConfig = new("Config", "Configure SC Settings.");
    private static readonly UIMenuItem CalloutList = new("Force Callout", "Spawn any selected callout.");
    private static readonly UIMenuItem EndCallout = new("~r~End Callout", "Ends the current callout.");
    
    private static readonly UIMenu EventMenu = new("Events", "Choose an event to spawn.");
    private static readonly UIMenuItem SeConfig = new("Config", "Configure SE Settings.");
    private static readonly UIMenuItem EventList = new("Force Event", "Spawn any selected event.");
    private static readonly UIMenuCheckboxItem PauseEvent = new("~y~Pause Events", Main.EventsPaused);
    private static readonly UIMenuItem EndEvent = new("~r~End Event", "Ends the current event.");
    
    private static readonly UIMenuItem DwConfig = new("Config", "Configure DW Settings.");
    
    private static readonly UIMenu ScConfigMenu = new("SuperCallouts.ini", "Config Settings");
    private static readonly UIMenuItem ScCfgInteract = new("Interaction Key", "This must be a valid key name!");
    private static readonly UIMenuItem ScCfgEndCall = new("End Call Key", "This must be a valid key name!");
    private static readonly UIMenuItem ScCfgNumber = new("Emergency Number", "Number to show in calls, ex 911");
    private static readonly UIMenuItem ScCfgSave = new("~r~Save Config", "Saves to the ini file!");
    
    private static readonly UIMenu SeConfigMenu = new("SuperEvents.ini", "Config Settings");
    private static readonly UIMenuCheckboxItem SeCfgBlips = new("Show Blips", SeSettings.ShowBlips);
    private static readonly UIMenuCheckboxItem SeCfgHints = new("Show Hints", SeSettings.ShowHints);
    private static readonly UIMenuItem SeCfgTimer = new("Event Timer", "How long between events.");
    private static readonly UIMenuItem SeCfgInteract = new("Interaction Key", "This must be a valid key name!");
    private static readonly UIMenuItem SeCfgEndEvent = new("End Event Key", "This must be a valid key name!");
    private static readonly UIMenuItem SeCfgSave = new("~r~Save Config", "Saves to the ini file!");
    
    private static readonly UIMenu DwConfigMenu = new("DeadlyWeapons.ini", "Config Settings");
    private static readonly UIMenuCheckboxItem DwCfgPdamage = new("Player Damage", DwSettings.PlayerDamage);
    private static readonly UIMenuCheckboxItem DwCfgNdamage = new("NPC Damage", DwSettings.NpcDamage);
    private static readonly UIMenuItem DwCfgRandomizer = new("Damage Randomizer", "Amount to randomize damage.");
    private static readonly UIMenuCheckboxItem DwCfgPanic = new("Panic", DwSettings.Panic);
    private static readonly UIMenuItem DwCfgCooldown = new("Cooldown", "How long between panic activations.");
    private static readonly UIMenuCheckboxItem DwCfgCode3 = new("Code3", DwSettings.Code3Backup);
    private static readonly UIMenuCheckboxItem DwCfgSwat = new("Swat", DwSettings.SwatBackup);
    private static readonly UIMenuCheckboxItem DwCfgNoose = new("Noose", DwSettings.NooseBackup);
    private static readonly UIMenuCheckboxItem DwCfgDebug = new("Debug", DwSettings.Debug);
    private static readonly UIMenuItem DwCfgSave = new("~r~Save Config", "Saves to the ini file!");

    internal static void StartUi()
    {
        _running = true;
        MainMenuPool.Add(MainMenu);
        MainMenuPool.Add(FirstMenu);
        MainMenuPool.Add(CalloutMenu);
        MainMenuPool.Add(EventMenu);
        MainMenuPool.Add(ScConfigMenu);
        MainMenuPool.Add(SeConfigMenu);
        MainMenuPool.Add(DwConfigMenu);
        MainMenu.AddItems(
            Extras.UiSeparator(Extras.CenterText(MainMenu, "Installed Plugins")), 
            Extras.SuperCallouts(), Extras.SuperEvents(), Extras.DeadlyWeapons(),
            Extras.UiSeparator(Extras.CenterText(MainMenu, "SuperCallouts")), 
            ScConfig, CalloutList, EndCallout, 
            Extras.UiSeparator(Extras.CenterText(MainMenu, "SuperEvents")), 
            SeConfig, EventList, PauseEvent, EndEvent, 
            Extras.UiSeparator(Extras.CenterText(MainMenu, "DeadlyWeapons")),
            DwConfig);
        MainMenu.BindMenuToItem(CalloutMenu, CalloutList);
        MainMenu.BindMenuToItem(ScConfigMenu, ScConfig);
        MainMenu.BindMenuToItem(EventMenu, EventList);
        MainMenu.BindMenuToItem(SeConfigMenu, SeConfig);
        MainMenu.BindMenuToItem(DwConfigMenu, DwConfig);
        
        ScConfigMenu.AddItems(ScCfgInteract, ScCfgEndCall, ScCfgNumber, ScCfgSave);
        SeConfigMenu.AddItems(SeCfgBlips, SeCfgHints, SeCfgTimer, SeCfgInteract, SeCfgEndEvent, SeCfgSave);
        DwConfigMenu.AddItems(DwCfgPdamage, DwCfgNdamage, DwCfgRandomizer, DwCfgPanic, DwCfgCooldown, Extras.UiSeparator(Extras.CenterText(DwConfigMenu, "Backup Options")), 
            DwCfgCode3, DwCfgSwat, DwCfgNoose, Extras.UiSeparator(Extras.CenterText(DwConfigMenu, "Debug Mode")), DwCfgDebug, DwCfgSave);
        
        MainMenu.RefreshIndex();
        CalloutMenu.RefreshIndex();
        EventMenu.RefreshIndex();
        ScConfigMenu.RefreshIndex();
        SeConfigMenu.RefreshIndex();
        DwConfigMenu.RefreshIndex();
        
        MainMenu.OnItemSelect += MenuSelected;
        ScConfigMenu.OnItemSelect += MenuSelected;
        SeConfigMenu.OnItemSelect += MenuSelected;
        DwConfigMenu.OnItemSelect += MenuSelected;
        
        if ( !Main.UsingSc )
        {
            ScConfig.RightLabel = "Not Installed!";
            ScConfig.Skipped = true;
            CalloutList.Skipped = true;
            EndCallout.Skipped = true;
        }
        
        if ( !Main.UsingSe )
        {
            SeConfig.RightLabel = "Not Installed!";
            SeConfig.Skipped = true;
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
            UpdateNotifications, ErrorReporting, ManagerKey,
            Extras.UiSeparator(Extras.CenterText(FirstMenu, "Saves PyroCommon.ini")),
            SaveButton);
        FirstMenu.RefreshIndex();
        FirstMenu.OnItemSelect += MenuSelected;
        UpdateNotifications.Checked = Settings.UpdateNotifications;
        UpdateNotifications.Description = "Shows update notifications on startup.";
        ErrorReporting.Checked = Settings.ErrorReporting;
        ErrorReporting.Description = "Reports errors automatically to help better my plugins. No personal data is shared!";
        ManagerKey.WithTextEditing(Settings.Manager.ToString, s => { Settings.Manager = PyroFunctions.PyroFunctions.ConvertStringToClosestKey(s, Settings.Manager); });
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
            foreach (var t in PyroFunctions.PyroFunctions.RegisteredScCallouts)
            {
                var s = new UIMenuItem(t.Name);
                CalloutMenu.AddItem(s);
                s.Activated += (_, _) => Functions.StartCallout(t.Name);
            }
        }
        if ( Main.UsingSe )
        {
            foreach (var t in SuperEvents.GetAllEvents())
            {
                var s = new UIMenuItem($"[{t.Namespace!.Split('.').First()}] {t.Name}");
                EventMenu.AddItem(s);
                s.Activated += (_,_) => SuperEvents.ForceEvent(t.FullName);
            }
        }
        Style.ApplyStyle(MainMenuPool, true);
        if ( Main.UsingSc )
        {
            ScSettings.GetSettings();
            //SuperCallouts Text buttons
            ScCfgInteract.WithTextEditing(ScSettings.Interact.ToString, s => { ScSettings.Interact = PyroFunctions.PyroFunctions.ConvertStringToClosestKey(s, ScSettings.Interact); });
            ScCfgEndCall.WithTextEditing(ScSettings.EndCall.ToString, s => { ScSettings.EndCall = PyroFunctions.PyroFunctions.ConvertStringToClosestKey(s, ScSettings.EndCall); });
            ScCfgNumber.WithTextEditing(ScSettings.EmergencyNumber.ToString, s => { ScSettings.EmergencyNumber = s; });
        }

        if ( Main.UsingSe )
        {
            SeSettings.GetSettings();
            //SuperEvents Text buttons
            SeCfgTimer.WithTextEditing(SeSettings.TimeBetweenEvents.ToString, s =>
            {
                if (int.TryParse(s, out var value)) SeSettings.TimeBetweenEvents = value; 
                else Game.DisplayHelp("~r~That is not a number!");
            });
            SeCfgInteract.WithTextEditing(SeSettings.Interact.ToString, s => { SeSettings.Interact = PyroFunctions.PyroFunctions.ConvertStringToClosestKey(s, SeSettings.Interact); });
            SeCfgEndEvent.WithTextEditing(SeSettings.EndEvent.ToString, s => { SeSettings.EndEvent = PyroFunctions.PyroFunctions.ConvertStringToClosestKey(s, SeSettings.EndEvent); });
        }

        if ( Main.UsingDw )
        {
            DwSettings.GetSettings();
            //DeadlyWeapons Text buttons
            DwCfgRandomizer.WithTextEditing(DwSettings.DamageRandomizer.ToString, s =>
            {
                if (int.TryParse(s, out var value)) DwSettings.DamageRandomizer = value; 
                else Game.DisplayHelp("~r~That is not a number!");
            });
            DwCfgCooldown.WithTextEditing(DwSettings.PanicCooldown.ToString, s =>
            {
                if (int.TryParse(s, out var value)) DwSettings.PanicCooldown = value; 
                else Game.DisplayHelp("~r~That is not a number!");
            });
        }
    }

    private static void ToggleManagerMenu()
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
    
    private static void MenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        //MainMenu
        if ( selecteditem == PauseEvent )
        {
            SuperEvents.PauseEvents();
            PauseEvent.Checked = !Main.EventsPaused;
        }
        if (selecteditem == EndCallout) Functions.StopCurrentCallout();
        if (selecteditem == EndEvent) SuperEvents.EndEvent();
        //FirstMenu
        if ( selecteditem == UpdateNotifications ) Settings.UpdateNotifications = !UpdateNotifications.Checked;
        if ( selecteditem == ErrorReporting ) Settings.ErrorReporting = !ErrorReporting.Checked;
        if (selecteditem == SaveButton)
        {
            Settings.SaveSettings();
            FirstMenu.Visible = false;
        }
        //ScConfigMenu
        if ( selecteditem == ScCfgSave )
        {
            ScSettings.ApplySettings();
            ScSettings.SaveSettings();
            ScConfigMenu.Close();
            Game.DisplayHelp("~g~SuperCallouts.ini saved!");
        }
        //SeConfigMenu
        if ( selecteditem == SeCfgBlips ) SeSettings.ShowBlips = !SeCfgBlips.Checked;
        if ( selecteditem == SeCfgHints ) SeSettings.ShowHints = !SeCfgHints.Checked;
        if ( selecteditem == SeCfgSave )
        {
            SeSettings.ApplySettings();
            SeSettings.SaveSettings();
            SeConfigMenu.Close();
            Game.DisplayHelp("~g~SuperEvents.ini saved!");
        }
        //DwConfigMenu
        if ( selecteditem == DwCfgPdamage ) DwSettings.PlayerDamage = !DwCfgPdamage.Checked;
        if ( selecteditem == DwCfgNdamage ) DwSettings.NpcDamage = !DwCfgNdamage.Checked;
        if ( selecteditem == DwCfgPanic ) DwSettings.Panic = !DwCfgPanic.Checked;
        if ( selecteditem == DwCfgCode3 ) DwSettings.Code3Backup = !DwCfgCode3.Checked;
        if ( selecteditem == DwCfgSwat ) DwSettings.SwatBackup = !DwCfgSwat.Checked;
        if ( selecteditem == DwCfgNoose ) DwSettings.NooseBackup = !DwCfgNoose.Checked;
        if ( selecteditem == DwCfgDebug ) DwSettings.Debug = !DwCfgDebug.Checked;
        if ( selecteditem == DwCfgSave )
        {
            DwSettings.ApplySettings();
            DwSettings.SaveSettings();
            DwConfigMenu.Close();
            Game.DisplayHelp("~g~DeadlyWeapons.ini saved!");
        }
    }
}