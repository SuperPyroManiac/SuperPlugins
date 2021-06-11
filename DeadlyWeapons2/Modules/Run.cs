#region

using System;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons2.Modules
{
    internal class Run
    {
        private GameFiber _processFiber;
        private Ped Player => Game.LocalPlayer.Character;

        internal void Start()
        {
            try
            {
                if (Settings.EnablePulloverAi) Events.OnPulloverStarted += Pullover.PulloverModule;
                if (Settings.EnableDamageSystem)
                {
                    var ps = new PlayerShot();
                    ps.StartEvent();
                }

                //if (Settings.EnableBetterAi)
                //{
                //    var ps = new PedShot();
                //    ps.StartPedEvent();
                //}
                Process();
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

        private void Process()
        {
            _processFiber = new GameFiber(delegate
                {
                    Game.LogTrivial("DeadlyWeapons: Starting ProcessFiber.");
                    while (true)
                    {
                        MainFiber();
                        GameFiber.Wait(Settings.PluginDelay);
                    }
                    
                });
            _processFiber.Start();
        }

        private void MainFiber()
        {
            if (Player.IsShooting && Player.Inventory.EquippedWeapon.Hash != WeaponHash.StunGun &&
                    Player.Inventory.EquippedWeapon.Hash != WeaponHash.FireExtinguisher && Player.Inventory.EquippedWeapon.Hash != WeaponHash.Flare && Settings.EnablePanic)
                    StartPanic.PanicHit();
            if (Settings.EnableBetterAi)
            {
                var peds = Game.LocalPlayer.Character.GetNearbyPeds(16);
                //var peds = World.GetAllPeds();
                foreach (Ped ped in peds)
                {
                    if (ped != Player || ped.IsHuman || !ped.IsInAnyVehicle(true) || !ped.IsDead)
                        PedShot.PedAi(ped);
                }
            }
        }

        internal void Stop()
        {
            _processFiber.Abort();
            Game.LogTrivial(
                "Deadly Weapons: ProccessFiber has been terminated. You may see an error here but it is normal.");
        }
    }
}