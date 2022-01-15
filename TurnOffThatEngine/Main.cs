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
                        if ((Game.IsKeyDown(Settings.Turnoffenginekey) || Game.IsControllerButtonDown(Settings.Turnoffenginebutton)) && !isDisabled)
                        {
                            if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                            {
                                NativeFunction.Natives.x2497C4717C8B881E(Game.LocalPlayer.Character.CurrentVehicle,
                                    false, false, true); //Vehicle_Engine_On
                                isDisabled = true;
                            }
                            else Game.DisplayHelp("~r~You are not in a vehicle!", 3000);
                        }
                        else if (Game.IsControlPressed(0, GameControl.VehicleAccelerate) && isDisabled)
                        {
                            NativeFunction.Natives.x2497C4717C8B881E(Game.LocalPlayer.Character.CurrentVehicle,
                                true, false, false); //Vehicle_Engine_On
                            isDisabled = false;
                        }
                    }
                    catch (Exception)
                    {
                        //Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                        //Game.LogTrivial("TurnOffThatEngine Error Report Start");
                        //Game.LogTrivial("======================================================");
                        //Game.LogTrivial(e.ToString());
                        //Game.LogTrivial("======================================================");
                        //Game.LogTrivial("TurnOffThatEngine Error Report End");
                    }
                }
            });
            process.Start();
        }
    }
}