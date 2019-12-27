#region

using Rage;
using SuperEvents.SimpleFunctions;

#endregion

namespace SuperEvents
{
    public class AmbientEvent
    {
        protected static bool EventsActive { get; set; }
        protected static bool TimeStart { get; set; }

        protected virtual void StartEvent()
        {
            EventsActive = true;
            MainLogic();
        }

        protected virtual void MainLogic()
        {
            if (!EventsActive) return;
        }

        protected virtual void End()
        {
            EventsActive = false;
            Game.LogTrivial("SuperEvents: Ending Event.");
            EventTimer.TimerStart();
        }
        
        protected virtual void Failed()
        {
            EventsActive = false;
            Game.LogTrivial("SuperEvents: Unable to start event. Skipping.");
            EventTimer.TimerStart();
        }
    }
}
