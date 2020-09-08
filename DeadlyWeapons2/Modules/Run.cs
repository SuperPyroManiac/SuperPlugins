using System;
using Rage;

namespace DeadlyWeapons2.Modules
{
    internal class Run
    {
        internal GameFiber ProcessFiber;
        
        internal void Start()
        {
            try
            {
                ProcessFiber = new GameFiber(delegate
                {
                    Game.LogTrivial("DeadlyWeapons: Starting main ProcessFiber...");
                    while (true)
                    {
                        GameFiber.Yield();
                    }
                });
                ProcessFiber.Start();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("Deadly Weapons Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Deadly Weapons Error Report End");
            }
        }

        internal void Stop()
        {
            ProcessFiber.Abort();
            Game.LogTrivial("Deadly Weapons: ProccessFiber has been terminated. You may see an error here but it is normal.");
        }
    }
}