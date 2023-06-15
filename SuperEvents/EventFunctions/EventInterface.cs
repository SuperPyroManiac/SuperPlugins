using System;
using LSPD_First_Response.Engine;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
// ReSharper disable IdentifierTypo

namespace SuperEvents.EventFunctions;

internal class EventInterface
{
    private readonly MenuPool eventMenuPool = new();
    private readonly UIMenu MainMenu = new("SuperEvents", "Choose an option.");
    private readonly UIMenu EventMenu = new("SuperEvents", "Choose an event to spawn.");
    private readonly UIMenuItem EventList = new("Force Event", "Spawn any selected event.");
    private readonly UIMenuItem PauseEvent = new("~y~Pause Events", "Pause events from spawning. Force Event bypasses this.");
    private readonly UIMenuItem EndEvent = new("~r~End Event", "Ends the current event.");

    internal void StartInterface()
    {
        try
        {
            new GameFiber(delegate
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

    private void SetupUI()
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
        EventList.Enabled = false; //TODO: Add all events as buttons to EventMenu later..
        MainMenu.AddItem(PauseEvent);
        MainMenu.AddItem(EndEvent);
        //Bind the menu to a button
        MainMenu.BindMenuToItem(EventMenu, EventList);
        //Refresh the index of the menus
        MainMenu.RefreshIndex();
        EventMenu.RefreshIndex();
        //Add events to the menus!
        MainMenu.OnItemSelect += MainMenuSelected;
        EventMenu.OnItemSelect += EventMenuSelected;
    }
    
    private void Process()
    {
        eventMenuPool.ProcessMenus();
    }
    
    private void MainMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        if (selecteditem == PauseEvent) Main.PausePlugin();
        if (selecteditem == EndEvent)
        {
            EventManager.CurrentEvent?.End();
            EventManager.CurrentEvent = null;
        }
    }
    
    private void EventMenuSelected(UIMenu sender, UIMenuItem selecteditem, int index)
    {
        throw new NotImplementedException();
    }

}