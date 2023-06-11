#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents.EventFunctions;

#endregion

namespace PyroCommon.Events;

public abstract class AmbientEvent
{
    internal bool HasEnded { get; set; }
    internal static bool ShowBlips { get; set; }
    internal static bool ShowHints { get; set; }
    internal static Keys EndEvent { get; set; }
    internal static Keys Interact { get; set; }
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
                    OnProcess();
                    GameFiber.Yield();
                }
            });
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            // ReSharper disable once VirtualMemberCallInConstructor
            HasEnded = true;
            End(true);
        }
    }

    internal void StartEvent()
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
        OnStartEvent();
        ProcessFiber.Start();
    }
    
    protected virtual void OnStartEvent()
    {
    }

    protected virtual void OnScene()
    {
    }

    private void Process()
    {
        if (Game.IsKeyDown(EndEvent)) End(false);
        if (Game.IsKeyDown(Interact)) MainMenu.Visible = !MainMenu.Visible;
        if (EventLocation.DistanceTo(Player) > 200f)
        {
            End();
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
        OnProcess();
        Interaction.ProcessMenus();
    }
    
    protected virtual void OnProcess()
    {
    }

    protected internal void End(bool forceCleanup = false)
    {
        EventRunning = false;
        End();
        if (forceCleanup)
        {
            foreach (var entity in EntitiesToClear.Where(entity => entity))
                if (entity.Exists()) entity.Delete();
            Log.Info("Event has been forcefully cleaned up.");
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
        Log.Info("Ending Event.");
        HasEnded = true;
    }

    protected abstract void OnCleanup();

    protected virtual void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == EndCall) End();
    }

    protected virtual void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
    }
}