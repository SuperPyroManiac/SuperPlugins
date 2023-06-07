using System;
using PyroCommon.Events;
using Rage;

namespace SuperEvents.EventFunctions
{
    internal abstract class EventTimer
    {
        private static GameFiber _timerFiber;
        private static int _timerDuration;
        
        internal static void TimerStart()
        {
            _timerDuration = Settings.TimeBetweenEvents * 1000;
            var rndDuration = new Random().Next(-15000, 15000);
            _timerDuration = _timerDuration + rndDuration;
            _timerFiber = GameFiber.StartNew(TimerRun);
        }

        private static void TimerRun()
        {
            if (AmbientEvent.TimeStart) return;
            Game.LogTrivial("SuperEvents: Event Timer started for: " + _timerDuration / 1000 + " seconds.");
            GameFiber.Wait(_timerDuration);
            AmbientEvent.TimeStart = true;
            Game.LogTrivial("SuperEvents: New events can now generate...");
        }
    }
}