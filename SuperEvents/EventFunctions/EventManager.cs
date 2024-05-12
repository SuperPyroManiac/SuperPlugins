using System;
using System.Collections.Generic;
using System.Threading;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;
using SuperEvents.Attributes;

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

    public static void RegisterEvent(Type type, Priority eventPriority = Priority.Normal)
    {
        Log.Info($"Registering event - [{type.Name}] from {type.Assembly.FullName}");
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
        var pri = eventPriority switch
        {
            Priority.Low => 1,
            Priority.Normal => 2,
            Priority.High => 3,
            _ => 0
        };
        while (pri > 0)
        {
            RegisteredEvents.Add(type);
            pri--;
        }
    }


    internal static void InitEvents()
    {
        try
        {
            if (BrokenEvents.Count > 0) LogBrokenEvents();
            CurrentEvent?.EndEvent(true);
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
                if (EventTimer.Finished && CurrentEvent == null) StartRandomEvent();
            }
        }
        catch (Exception e) when (e is not ThreadAbortException)
        {
            Log.Error(e.ToString());
        }
    }

    private static void LogBrokenEvents()
    {
        var message = "~b~SuperEvents\n~w~The following events could not be loaded:\n";
        foreach (var type in BrokenEvents) message += $"{type.FullName} ";
        Game.DisplayHelp(message);
        Log.Warning(message);
    }

    private static void StartRandomEvent()
    {
        Log.Info("Generating random event.");
        var rnD = new Random(DateTime.Now.Millisecond).Next(RegisteredEvents.Count);
        var eventType = RegisteredEvents[rnD];
        StartEvent(eventType);
    }

    internal static void ForceEvent(string eventName)
    {
        if (PlayerIsBusy)
        {
            Log.Info($"Failed to start \"{eventName}\", player is busy.");
            return;
        }

        Log.Info($"Generating {eventName} event.");
        foreach (var currentEvent in RegisteredEvents)
            if (currentEvent.FullName == eventName)
            {
                StartEvent(currentEvent);
                return;
            }

        Log.Warning($"Event \"{eventName}\" not found.");
    }

    private static void StartEvent(Type eventType)
    {
        Log.Info("Loading " + eventType.Name + " from " + eventType.Assembly.FullName);
        CurrentEvent = Activator.CreateInstance(eventType) as AmbientEvent;
        CurrentEvent.StartEvent();
        EventTimer.Stop();
    }
}