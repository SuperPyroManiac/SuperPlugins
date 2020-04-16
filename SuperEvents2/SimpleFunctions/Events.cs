using System;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents2.Events;


namespace SuperEvents2.SimpleFunctions
{
    public class Events : AmbientEvent
    {
                private static readonly Random RNd = new Random();

        public static void InitEvents()
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    while (Main.PluginRunning)
                    {
                        GameFiber.Wait(1000);
                        if (!Functions.IsCalloutRunning() && !Functions.IsPlayerPerformingPullover() && Functions.GetActivePursuit() == null && TimeStart && !EventRunning)
                        {
                            Game.LogTrivial("SuperEvents: Generating random event.");
                            var choices = RNd.Next(1, 2);
                            
                            switch(choices)
                            {
                                case 1:
                                    if (Settings.Fight)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting fight event.");
                                        var fight = new Fight();
                                        fight.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: Fight event disabled in config.. Trying again for another event."); }
                                    break;
                                default:
                                    Game.LogTrivial("SuperEvents: If you see this error please tell SuperPyroManiac he is a fool. This error should never pop up unless I forget how to count.");
                                    break;
                            }
                        }else
                        {
                            GameFiber.Wait(10000);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                        Game.LogTrivial("Oops there was a MAJOR error here. Please send this log to SuperPyroManiac!");
                        Game.LogTrivial("SuperEvents Error Report Start");
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial(e.ToString());
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial("SuperEvents Error Report End");
                        Game.DisplaySubtitle("~r~SuperEvents: Plugin has found a major error. Please send your RagePluginHook.log to SuperPyroManiac on the LSPDFR website!");
            }
        }
    }
}