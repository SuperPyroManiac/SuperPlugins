using System;
using LSPD_First_Response.Mod.API;
using Rage;

namespace SuperEvents.EventFunctions;

internal class Events : AmbientEvent
{
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
                    var rnD = new Random().Next(API.RegisteredEvents.Count);
                    var sEvent = API.RegisteredEvents[rnD];
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