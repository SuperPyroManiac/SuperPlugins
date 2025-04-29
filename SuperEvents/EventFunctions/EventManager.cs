using System;
using System.Collections.Generic;
using System.Threading;
using LSPD_First_Response.Mod.API;
using PyroCommon.Utils;
using Rage;
using Rage.Exceptions;
using SuperEvents.Attributes;

namespace SuperEvents.EventFunctions;

public static class EventManager
{
    internal static readonly List<Type> RegisteredEvents = [];
    internal static readonly List<Type> AllEvents = [];
    private static readonly List<Type> BrokenEvents = [];
    internal static AmbientEvent CurrentEvent;

    internal static bool PlayerIsBusy =>
        Functions.IsCalloutRunning()
        || Functions.IsPlayerPerformingPullover()
        || Functions.GetActivePursuit() != null
        || CurrentEvent != null;

    public enum Priority
    {
        Low,
        Normal,
        High,
    }

    public static void RegisterEvent(Type type, Priority eventPriority = Priority.Normal)
    {
        LogUtils.Info($"Registering event - [{type.Name}] from {type.Assembly.FullName}");
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
            _ => 0,
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
            if (BrokenEvents.Count > 0)
                LogBrokenEvents();
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

                EventTimer.Paused = PlayerIsBusy || PyroCommon.Main.EventsPaused;
                if (EventTimer.Finished && CurrentEvent == null)
                    StartRandomEvent();
            }
        }
        catch (Exception e) when (e is not ThreadAbortException)
        {
            if (e.Message.Contains("Could not spawn new vehicle"))
                LogUtils.Error(
                    "Vehicle spawn failed! This is likely a mods folder issue and not the plugins fault!\r\n" + e.Message,
                    false
                );
            if (e.Message.Contains("Cannot load invalid model with hash"))
                LogUtils.Error(
                    "Vehicle spawn failed! This is likely a mods folder issue and not the plugins fault!\r\n" + e.Message,
                    false
                );
            if (e is InvalidHandleableException)
                LogUtils.Error("Failed to start event! Welcome to modded GTA. Not much I can do here.\r\n" + e.Message, false);
            else
                LogUtils.Error(e.ToString());
            EndEvent();
        }
    }

    private static void LogBrokenEvents()
    {
        var message = "~b~SuperEvents\n~w~The following events could not be loaded:\n";
        foreach (var type in BrokenEvents)
            message += $"{type.FullName} ";
        Game.DisplayHelp(message);
        LogUtils.Warning(message);
    }

    private static void StartRandomEvent()
    {
        LogUtils.Info("Generating random event.");
        var rnD = new Random(DateTime.Now.Millisecond).Next(RegisteredEvents.Count);
        var eventType = RegisteredEvents[rnD];
        StartEvent(eventType);
    }

    internal static void ForceEvent(string eventName)
    {
        if (PlayerIsBusy)
        {
            LogUtils.Info($"Failed to start \"{eventName}\", player is busy.");
            return;
        }

        LogUtils.Info($"Generating {eventName} event.");
        foreach (var currentEvent in RegisteredEvents)
            if (currentEvent.FullName == eventName)
            {
                StartEvent(currentEvent);
                return;
            }

        LogUtils.Warning($"Event \"{eventName}\" not found.");
    }

    private static void StartEvent(Type eventType)
    {
        LogUtils.Info("Loading " + eventType.Name + " from " + eventType.Assembly.FullName);
        CurrentEvent = Activator.CreateInstance(eventType) as AmbientEvent;
        CurrentEvent?.StartEvent();
        EventTimer.Stop();
    }

    internal static void PauseEvents()
    {
        PyroCommon.Main.EventsPaused = !PyroCommon.Main.EventsPaused;
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~r~SuperEvents",
            "~g~Plugin Status:",
            "SuperEvents paused: " + PyroCommon.Main.EventsPaused
        );
        LogUtils.Info("Plugin paused: " + PyroCommon.Main.EventsPaused);
    }

    internal static void EndEvent()
    {
        CurrentEvent?.EndEvent();
    }
}
