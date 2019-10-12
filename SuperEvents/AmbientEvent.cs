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
        private static bool KillEvent { get; set; }

        protected virtual void StartEvent()
        {
            EventsActive = true;
            MainLogic();
            //GameFiber.StartNew(MainLogic);
        }

        protected virtual void MainLogic()
        {
                //GameFiber.Yield();
                if (KillEvent) { KillEvent = false; End(); }
                if (!EventsActive) return;
                //MainLogic();
        }

        protected virtual void End()
        {
            EventsActive = false;
            Game.LogTrivial("SuperEvents: Ending Event.");
            //Game.DisplayHelp("Scene ~g~CODE-4");
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
