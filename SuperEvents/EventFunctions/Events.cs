using System;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;

namespace SuperEvents.EventFunctions
{
    internal class Events : AmbientEvent
    {
        internal void InitEvents()
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    while (Main.PluginRunning)
                    {
                        GameFiber.Wait(1000);
                        if (!Functions.IsCalloutRunning() && !Functions.IsPlayerPerformingPullover() &&
                            Functions.GetActivePursuit() == null && TimeStart && !EventRunning && !Main.PluginPaused)
                        {
                            Game.LogTrivial("SuperEvents: Generating random event.");
                            var RnD = new Random().Next(API.RegisteredEvents.Count);
                            var Event = API.RegisteredEvents[RnD];
                            var theMethod = Event.GetMethod("StartEvent");
                            if (theMethod != null) theMethod.Invoke(this, default);
                        }
                        else
                        {
                            GameFiber.Wait(10000);
                        }
                    }
                });
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
}