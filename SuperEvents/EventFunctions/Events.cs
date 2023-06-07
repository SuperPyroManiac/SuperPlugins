using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using PyroCommon.Events;
using Rage;

namespace SuperEvents.EventFunctions;

internal class Events : AmbientEvent
{
    
    
    internal static List<Type> RegisteredEvents = new();
    internal static List<Type> AllEvents = new();
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
            while (Main.PluginRunning)
            {
                GameFiber.Wait(1000);
                if (!Functions.IsCalloutRunning() && !Functions.IsPlayerPerformingPullover() &&
                    Functions.GetActivePursuit() == null && TimeStart && !EventRunning && !Main.PluginPaused)
                {
                    Game.LogTrivial("SuperEvents: Generating random event.");
                    var rnD = new Random().Next(RegisteredEvents.Count);
                    var sEvent = RegisteredEvents[rnD];
                    Game.LogTrivial("SuperEvents: Loading " + sEvent.Name + " from " + sEvent.Assembly.FullName);
                    var theMethod = sEvent.GetMethod("StartEvent");
                    var eventClass = Activator.CreateInstance(sEvent);
                    if (theMethod != null) theMethod.Invoke(eventClass, null);
                }
                else
                {
                    GameFiber.Wait(10000);
                    if (!TimeStart) continue;
                    TimeStart = false;
                    Game.LogTrivial("SuperEvents: Player is busy, restarting timer...");
                    EventTimer.TimerStart();
                }
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
}