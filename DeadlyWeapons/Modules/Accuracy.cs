using System.Linq;
using PyroCommon.API;
using Rage;
using Rage.Native;

namespace DeadlyWeapons.Modules;

internal static class Accuracy
{
    internal static void StartAccuracyFiber()
    {
       Log.Info("Starting AccuracyFiber.");
       while ( true )
       {
           GameFiber.Sleep(2500);
           SetPedAccuracy();
       }
       // ReSharper disable once FunctionNeverReturns
    }

    private static void SetPedAccuracy()
    {
        var peds = World.GetAllPeds().Where(p => p != Game.LocalPlayer.Character);
        foreach ( var ped in peds ) 
            NativeFunction.Natives.x7AEFB85C1D49DEB6(ped, Settings.AiAccuracy);
        
    }
    
}