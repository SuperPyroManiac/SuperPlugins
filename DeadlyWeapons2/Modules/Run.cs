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
                if (Settings.EnablePulloverAI) Events.OnPulloverStarted += Pullover.PulloverModule;
                _processFiber = new GameFiber(delegate
                {
                    if (Settings.EnableDamageSystem)
                    {
                        var ps = new PlayerShot();
                        ps.StartEvent();
                    }

                    if (Settings.EnableBetterAi)
                    {
                        var ps = new PedShot();
                        ps.StartPedEvent();
                    }

                    Game.LogTrivial("DeadlyWeapons: Starting ProcessFiber.");
                    while (true)
                    {
                        if (Game.IsKeyDown(Settings.RubberBullets)) RubberBullet.RubberBullets();
                        if (Player.IsShooting && Player.Inventory.EquippedWeapon.Hash != WeaponHash.StunGun &&
                            Player.Inventory.EquippedWeapon.Hash != WeaponHash.FireExtinguisher && Settings.EnablePanic)
                            StartPanic.PanicHit();
                        if (Settings.EnableBetterAi)
                        {
                            var pedEntity = Game.LocalPlayer.GetFreeAimingTarget();
                            if (pedEntity == null) return;
                            if (NativeFunction.Natives.IS_ENTITY_A_PED<bool>(pedEntity))
                            {
                                var ped = pedEntity as Ped;
                                if (ped == null) return;
                                if (!ped == Player || ped.IsHuman || !ped.IsInAnyVehicle(true) || !ped.IsDead ||
                                    ped.RelationshipGroup != RelationshipGroup.Cop ||
                                    ped.RelationshipGroup != RelationshipGroup.Medic ||
                                    ped.RelationshipGroup != RelationshipGroup.Fireman)
                                    PedShot.PedAimedAt(ped);
                            }
                        }
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
            Game.LogTrivial(
                "Deadly Weapons: ProccessFiber has been terminated. You may see an error here but it is normal.");
        }
    }
}