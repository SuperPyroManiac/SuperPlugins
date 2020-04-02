#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents2.SimpleFunctions;
#endregion

namespace SuperEvents2
{
    public class AmbientEvent
    {
        public static bool EventRunning { get; set; }
        public static bool TimeStart { get; set; }
        public static List<Entity> EntitiesToClear { get; set; }
        public static List<Blip> BlipsToClear { get; set; }
        public GameFiber ProcessFiber { get; set; }
        public Ped Player => Game.LocalPlayer.Character;

        public AmbientEvent()
        {
            try
            {
                EntitiesToClear = new List<Entity>();
                BlipsToClear = new List<Blip>();
                ProcessFiber = new GameFiber(delegate
                {
                    while (EventRunning)
                    {
                        Process();
                        GameFiber.Yield();
                    }
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                End(true);
            }
        }
        
        public virtual void StartEvent(Vector3 spawnPoint, float spawnPointH)
        {
            AmbientEvent.TimeStart = false;
            if (Settings.ShowBlips)
            {
                var eventBlip = new Blip(spawnPoint, 15f);
                eventBlip.Color = Color.Red;
                eventBlip.Alpha /= 2;
                eventBlip.Name = "Event";
                eventBlip.Flash(500, 5000);
                BlipsToClear.Add(eventBlip);
            }
            
            EventRunning = true;
            ProcessFiber.Start();
        }

        public virtual void Process()
        {
        }

        public virtual void End(bool forceCleanup)
        {
            EventRunning = false;
            
            if (forceCleanup)
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    entity.Delete();
            }
            else
            {
                foreach (var entity in EntitiesToClear.Where(entity => entity))
                    entity.Dismiss(); 
            }
            
            foreach (var blip in BlipsToClear.Where(blip => blip))
                blip.Delete();
            
            Game.LogTrivial("SuperEvents: Ending Event.");
            EventTimer.TimerStart();
        }
    }
}