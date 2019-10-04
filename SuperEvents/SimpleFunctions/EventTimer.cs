#region

using Rage;

#endregion

namespace SuperEvents.SimpleFunctions
{
    internal class EventTimer : AmbientEvent
    {
        public static void TimerStart()
        {
            GameFiber.StartNew(delegate
            {
                if (TimeStart) return;
                var time = Settings.TimeBetweenEvents * 1000;
                GameFiber.Wait(time);
                TimeStart = true;
                Game.LogTrivial("SuperEvents: New events can now generate...");
            });
        }
    }
}
