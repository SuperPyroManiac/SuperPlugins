using System;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents2.Events;


namespace SuperEvents2.SimpleFunctions
{
    internal class Events : AmbientEvent
    {
                private static readonly Random RNd = new Random();

        internal static void InitEvents()
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    while (Main.PluginRunning)
                    {
                        GameFiber.Wait(1000);
                        if (!Functions.IsCalloutRunning() && !Functions.IsPlayerPerformingPullover() && Functions.GetActivePursuit() == null && TimeStart && !EventRunning && !Main.PluginPaused)
                        {
                            Game.LogTrivial("SuperEvents: Generating random event.");
                            var choices = RNd.Next(1, 15);
                            
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
                                case 2:
                                    if (Settings.CarFire)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting car fire event.");
                                        var fire = new CarFire();
                                        fire.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: Fire event disabled in config.. Trying again for another event."); }
                                    break;
                                case 3:
                                    if (Settings.PulloverShooting)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting pullover shooting event.");
                                        var shoot = new PulloverShooting();
                                        shoot.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: pullover shooting event disabled in config.. Trying again for another event."); }
                                    break;
                                case 4:
                                    if (Settings.RecklessDriver)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting reckless driver event.");
                                        var reckless = new RecklessDriver();
                                        reckless.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: reckless driver event disabled in config.. Trying again for another event."); }
                                    break;
                                case 5:
                                    goto case 4;
                                case 6:
                                    goto case 4;
                                case 7:
                                    goto case 2;
                                case 8:
                                    goto case 1;
                                case 9:
                                    if (Settings.CarAccident)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting car accident event.");
                                        var accident = new CarAccident();
                                        accident.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: accident event disabled in config.. Trying again for another event."); }
                                    break;
                                case 10:
                                    goto case 9;
                                case 11:
                                    goto case 9;
                                case 12:
                                    if (Settings.AbandonedCar)
                                    {
                                        Game.LogTrivial("SuperEvents: Starting car accident event.");
                                        var weridcar = new CarAccident();
                                        weridcar.StartEvent(default, 0);
                                    }
                                    else { Game.LogTrivial("SuperEvents: abandoned car event disabled in config.. Trying again for another event."); }
                                    break;
                                case 13:
                                    goto case 12;
                                case 14:
                                    goto case 12;
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