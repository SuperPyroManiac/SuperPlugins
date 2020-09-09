using System;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons2.Modules
{
    internal class Run
    {
        private Ped Player => Game.LocalPlayer.Character;
        private GameFiber _processFiber;
        
        internal void Start()
        {
            try
            {
                if (Settings.EnablePulloverAI) Events.OnPulloverStarted += Pullover.PulloverModule;
                _processFiber = new GameFiber(delegate
                {
                    if (Settings.EnableDamageSystem)
                    {
                        var ps = new PlayerShot();
                        ps.StartEvent();
                    }
                    Game.LogTrivial("DeadlyWeapons: Starting ProcessFiber.");
                    while (true)
                    {
                        if (Game.IsKeyDown(Settings.RubberBullets)) RubberBullet.RubberBullets();
                        if (Player.IsShooting && Player.Inventory.EquippedWeapon.Hash != WeaponHash.StunGun &&
                            Player.Inventory.EquippedWeapon.Hash != WeaponHash.FireExtinguisher && Settings.EnablePanic)
                            StartPanic.PanicHit();
                        GameFiber.Yield();
                    }
                });
                _processFiber.Start();
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
            _processFiber.Abort();
            Game.LogTrivial("Deadly Weapons: ProccessFiber has been terminated. You may see an error here but it is normal.");
        }
    }
}