using System;
using Rage;
using Rage.Native;

namespace TurnOffThatEngine
{
    internal class Main
    {
        internal static void MainFiber()
        {
            var isDisabled = false;
            var process = new GameFiber(delegate
            {
                while (true)
                {
                    GameFiber.Yield();
                    try
                    {
                        if (Game.IsKeyDown(Settings.turnoffengine) && !isDisabled)
                        {
                            if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                            {
                                NativeFunction.Natives.SET_VEHICLE_ENGINE_ON(Game.LocalPlayer.Character.CurrentVehicle,
                                    false, false, true);
                                isDisabled = true;
                            }
                            else Game.DisplayHelp("~r~You are not in a vehicle!", 3000);
                        }
                        else if (Game.IsControlPressed(0, GameControl.VehicleAccelerate) && isDisabled)
                        {
                            NativeFunction.Natives.SET_VEHICLE_ENGINE_ON(Game.LocalPlayer.Character.CurrentVehicle,
                                true, false, false);
                            isDisabled = false;
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