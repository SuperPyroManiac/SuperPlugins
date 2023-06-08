using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using PyroCommon.Events;
using Rage;

namespace SuperEvents.EventFunctions;

public static class EventManager
{
    internal static List<Type> RegisteredEvents = new();
    internal static List<Type> AllEvents = new();
    internal static AmbientEvent CurrentEvent;

    internal static bool PlayerIsBusy =>
        Functions.IsCalloutRunning() || Functions.IsPlayerPerformingPullover() ||
        Functions.GetActivePursuit() != null || CurrentEvent != null;

    public enum Priority
    {
        Low,
        Normal,
        High
    }

    public static void RegisterEvent(Type type, Priority EventPriority = Priority.Normal)
    {
        AllEvents.Add(type);
        var PRI = EventPriority switch
        {
            Priority.Low => 1,
            Priority.Normal => 2,
            Priority.High => 3,
            _ => 0
        };
        while (PRI > 0)
        {
            RegisteredEvents.Add(type);
            PRI--;
        }
    }


    internal static void InitEvents()
    {
        try
        {
            EventTimer.Start();
            while (Main.PluginRunning)
            {
                GameFiber.Wait(1000);
                if (CurrentEvent?.HasEnded == true)
                {
                    CurrentEvent = null;
                    EventTimer.Start();
                }

                EventTimer.Paused = PlayerIsBusy || Main.PluginPaused;
                if (EventTimer.Finished && CurrentEvent == null) StartRandomEvent(); // TODO: Timer Ended
            }
        }
        catch (Exception e)
        {
            Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.LogTrivial("SuperEvents Error Report Start");
            Game.LogTrivial("======================================================");
            Game.LogTrivial(e.ToString());
            Game.LogTrivial("======================================================");
            Game.LogTrivial("SuperEvents Error Report End");
        }
    }

    private static void StartRandomEvent()
    {
        Game.LogTrivial("SuperEvents: Generating random event.");
        var rnD = new Random().Next(RegisteredEvents.Count);
        var eventType = RegisteredEvents[rnD];
        StartEvent(eventType);
    }

    internal static void ForceEvent(string eventName)
    {
        if (PlayerIsBusy)
        {
            Game.LogTrivial($"SuperEvents: Failed to start \"{eventName}\", player is busy.");
            return;
        }

        Game.LogTrivial("SuperEvents: Generating eventName event.");
        foreach (var currentEvent in RegisteredEvents)
            if (currentEvent.Name == eventName)
                StartEvent(currentEvent);
        Game.LogTrivial($"SuperEvents: Event \"{eventName}\" not found.");
    }

    private static void StartEvent(Type eventType)
    {
        Game.LogTrivial("SuperEvents: Loading " + eventType.Name + " from " + eventType.Assembly.FullName);
        CurrentEvent = Activator.CreateInstance(eventType) as AmbientEvent;
        CurrentEvent.StartEvent();
    }
}