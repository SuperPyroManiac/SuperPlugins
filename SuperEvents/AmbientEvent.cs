using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.Attributes;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

namespace SuperEvents;

public abstract class AmbientEvent
{
    internal bool HasEnded { get; set; }
    private static bool ShowBlips = Settings.ShowBlips;
    private static bool ShowHints = Settings.ShowHints;
    private static Keys EndKey = Settings.EndEvent;
    private static Keys Interact = Settings.Interact;
    private readonly string _eventTitle;
    private readonly string _eventDescription;
    protected readonly UIMenu ConvoMenu = new("SuperEvents", "~y~Choose a subject to speak with.");
    protected readonly UIMenuItem EndCall = new("~y~End Event", "Ends the event.");
    protected readonly MenuPool Interaction = new();
    protected readonly UIMenu MainMenu = new("SuperEvents", "Choose an option.");
    protected readonly UIMenuItem Questioning = new("Speak With Subjects");
    public static bool EventRunning { get; internal set; }
    protected abstract Vector3 EventLocation { get; set; }
    protected float OnSceneDistance { get; set; } = 20;
    protected float ClearEventDistance { get; set; } = 200;
    public List<Entity> EntitiesToClear { get; } = new();
    public List<Blip> BlipsToClear { get; } = new();
    private GameFiber ProcessFiber { get; }
    protected static Ped Player => Game.LocalPlayer.Character;
    private bool onScene;

    protected AmbientEvent()
    {
        var eventInfo = GetType().GetEventInfo();
        _eventTitle = eventInfo.Title;
        _eventDescription = eventInfo.Description;
        try
        {
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
            Log.Error(e.ToString());
            // ReSharper disable once VirtualMemberCallInConstructor
            HasEnded = true;
            EndEvent(true);
        }
    }

    internal void StartEvent()
    {
        EventRunning = true;
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
        OnStartEvent();
        if (EventLocation.DistanceTo(Player) > ClearEventDistance)
        {
            EndEvent(true);
            Log.Info("Ending event due to player being too far.");
        }
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
        ProcessFiber.Start();
    }
    
    protected abstract void OnStartEvent();

    protected virtual void OnScene() { }

    private void Process()
    {
        if (Game.IsKeyDown(EndKey)) EndEvent();
        if (Game.IsKeyDown(Interact)) MainMenu.Visible = !MainMenu.Visible;
        if (EventLocation.DistanceTo(Player) > ClearEventDistance)
        {
            EndEvent(true);
            Log.Info("Ending event due to player being too far.");
        }
        if (!onScene && Game.LocalPlayer.Character.DistanceTo(EventLocation) < OnSceneDistance)
        {
            onScene = true;
            if (ShowHints)
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                    "~r~" + _eventTitle, _eventDescription);
            Game.DisplayHelp("~y~Press ~r~" + Interact + "~y~ to open interaction menu.");
            OnScene();
        }
        if (Player.IsDead) EndEvent();
        Interaction.ProcessMenus();
        OnProcess();
    }
    
    protected abstract void OnProcess();

    protected internal void EndEvent(bool forceCleanup = false)
    {
        EventRunning = false;
        OnCleanup();
        if (forceCleanup)
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity.Exists())) entity.Delete();
            Log.Info("Event has been forcefully cleaned up.");
        }
        else
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity.Exists())) entity.Dismiss();
            Game.DisplayHelp("~y~Event Ended.");
        }

        foreach (var blip in BlipsToClear.Where(blip => blip.Exists())) blip.Delete();

        Interaction.CloseAllMenus();
        Log.Info("Ending Event.");
        HasEnded = true;
    }

    protected abstract void OnCleanup();

    protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == EndCall) EndEvent();
    }

    protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
    }
}