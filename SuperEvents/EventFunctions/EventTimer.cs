using Rage;

namespace SuperEvents.EventFunctions
{
    internal class EventTimer
    {
        internal static void TimerStart()
        {
            GameFiber.StartNew(delegate
            {
                if (AmbientEvent.TimeStart) return;
                GameFiber.Wait(Settings.TimeBetweenEvents * 1000);
                AmbientEvent.TimeStart = true;
                Game.LogTrivial("SuperEvents: New events can now generate...");
            });
        }
    }
}