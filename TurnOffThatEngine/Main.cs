using System;
using Rage;

namespace TurnOffThatEngine
{
    internal class Main
    {
        internal static void MainFiber()
        {
            var process = new GameFiber(delegate
            {
                while (true)
                {
                    try
                    {
                        if (Game.IsKeyDown(Settings.turnoffengine))
                        {
                            if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                            {
                                Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn = false;
                            }
                            else Game.DisplayHelp("~r~You are not in a vehicle!", 3000);
                        }
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                        Game.LogTrivial("TurnOffThatEngine Error Report Start");
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial(e.ToString());
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial("TurnOffThatEngine Error Report End");
                        Game.LogTrivial("Seriously, if you see this error. Pyro is an idiot.");
                    }
                }
            });
            process.Start();
        }
    }
}