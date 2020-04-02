using Rage;

namespace SuperEvents2.SimpleFunctions
{
    public class EventTimer
    {
        public static void TimerStart()
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