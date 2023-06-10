using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using PyroCommon.Events;
using Rage;

namespace SuperEvents.EventFunctions;

public static class EventManager
{
    internal static readonly List<Type> RegisteredEvents = new();
    internal static readonly List<Type> AllEvents = new();
    private static readonly List<Type> BrokenEvents = new();
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
        Game.Console.Print("SuperEvents: Registering event - " + type.Assembly.FullName);
        try
        {
            type.GetEventInfo();
        }
        catch (AttributeExpectedException)
        {
            BrokenEvents.Add(type);
            return;
        }

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
            if (BrokenEvents.Count > 0) LogBrokenEvents();
            CurrentEvent?.End(true);
            CurrentEvent = null;
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
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperEvents Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print(e.ToString());
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperEvents Error Report End");
        }
    }

    private static void LogBrokenEvents()
    {
        var message = "~b~SuperEvents\n~w~The following events could not be loaded:\n";
        foreach (var type in BrokenEvents) message += $"{type.FullName} ";
        Game.DisplayHelp(message);
        // TODO: Add RPH Console Logging.
    }

    private static void StartRandomEvent()
    {
        Game.Console.Print("SuperEvents: Generating random event.");
        var rnD = new Random().Next(RegisteredEvents.Count);
        var eventType = RegisteredEvents[rnD];
        StartEvent(eventType);
    }

    internal static void ForceEvent(string eventName)
    {
        if (PlayerIsBusy)
        {
            Game.Console.Print($"SuperEvents: Failed to start \"{eventName}\", player is busy.");
            return;
        }

        Game.Console.Print("SuperEvents: Generating eventName event.");
        foreach (var currentEvent in RegisteredEvents)
            if (currentEvent.FullName == eventName)
            {
                StartEvent(currentEvent);
                return;
            }

        Game.Console.Print($"SuperEvents: Event \"{eventName}\" not found.");
    }

    private static void StartEvent(Type eventType)
    {
        Game.Console.Print("SuperEvents: Loading " + eventType.Name + " from " + eventType.Assembly.FullName);
        CurrentEvent = Activator.CreateInstance(eventType) as AmbientEvent;
        CurrentEvent.StartEvent();
        EventTimer.Stop();
    }
}