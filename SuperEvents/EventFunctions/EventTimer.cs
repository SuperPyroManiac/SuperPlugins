using System;
using Rage;

namespace SuperEvents.EventFunctions
{
    internal static class EventTimer
    {
        private static GameFiber _timerFiber;
        private static int _timerDuration;
        private static uint _elapsedMilliseconds;
        internal static bool Finished { get; private set; }
        internal static bool Paused { get; set; }

        internal static void Start()
        {
            _timerDuration = Settings.TimeBetweenEvents * 1000 + new Random().Next(-15000, 15000);
            Finished = false;
            _elapsedMilliseconds = 0;
            _timerFiber = GameFiber.StartNew(Run);
        }

        private static void Run()
        {
            while (!Finished)
            {
                if (Paused || Finished) continue;
                Game.LogTrivial("SuperEvents: Event Timer started for: " + _timerDuration / 1000 + " seconds.");
                var prevTime = Game.GameTime;
                GameFiber.Yield();
                _elapsedMilliseconds += Game.GameTime - prevTime;
                if (_elapsedMilliseconds >= _timerDuration) continue;
                Finished = true;
                Game.LogTrivial("SuperEvents: New events can now generate...");
            }
        }
    }
}