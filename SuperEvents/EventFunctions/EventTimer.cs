using System;
using Rage;

namespace SuperEvents.EventFunctions;

internal static class EventTimer
{
    private static GameFiber _timerFiber;
    private static int _timerDuration;
    private static uint _elapsedMilliseconds;
    internal static bool Finished { get; private set; }
    internal static bool Paused { get; set; }

    internal static void Start()
    {
        _timerDuration = Settings.TimeBetweenEvents * 1000;
        Game.LogTrivial("SuperEvents: Event Timer started for: " + _timerDuration + " milliseconds.");
        Finished = false;
        _elapsedMilliseconds = 0;
        _timerFiber?.Abort();
        _timerFiber = GameFiber.StartNew(Run);
    }

    internal static void Stop() => Finished = true;

    private static void Run()
    {
        while (!Finished)
        {
            GameFiber.Wait(1);
            if (Paused || Finished) continue;
            _elapsedMilliseconds++;
            if (_elapsedMilliseconds <= _timerDuration) continue;
            Finished = true;
            Game.LogTrivial("SuperEvents: New events can now generate...");
            //var prevTime = Game.GameTime;
            //_elapsedMilliseconds += Game.GameTime - prevTime;
            //if (_elapsedMilliseconds < _timerDuration) continue;
        }
    }
}