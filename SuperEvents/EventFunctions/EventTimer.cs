using Rage;

namespace SuperEvents.EventFunctions
{
    internal abstract class EventTimer
    {
        private static GameFiber TimerFiber;
        internal static void TimerStart()
        {
            TimerFiber = GameFiber.StartNew(EventTimer.TimerRun);
        }

        internal static void TimerRun()
        {
            if (AmbientEvent.TimeStart) return;
            GameFiber.Wait(Settings.TimeBetweenEvents * 1000);
            AmbientEvent.TimeStart = true;
            Game.LogTrivial("SuperEvents: New events can now generate...");
        }
    }
}