#region

using System;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons.DFunctions
{
    internal static class CustomAI
    {
        internal static void PedAi(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    if (!ped || ped.IsDead) return;
                    ped.Accuracy = Settings.AiAccuracy;
                    ped.FiringPattern = FiringPattern.DelayFireByOneSecond;

                    foreach (var w in DeadlyWeapons.WeaponHashes)
                        if (NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, (uint) w, 0) &&
                            Settings.EnableDamageSystem)
                        {
                            if (ped.Armor >= 60)
                            {
                                var rnd = new Random().Next(0, 10);
                                switch (rnd)
                                {
                                    case 1:
                                        ped.Health = 100;
                                        ped.Armor = 61;
                                        break;
                                    case 2:
                                        ped.Health = 100;
                                        ped.Armor = 61;
                                        Timer.Ragdoll(ped);
                                        break;
                                    case 3:
                                        ped.Health = 80;
                                        ped.Armor = 0;
                                        Timer.PedReact(ped);
                                        break;
                                    default:
                                        ped.Health = 100;
                                        ped.Armor = 0;
                                        break;
                                }

                                Game.LogTrivial("Deadly Weapons: " + Functions.GetPersonaForPed(ped).FullName +
                                                " rolled 1-" + rnd);
                            }
                            else
                            {
                                var rnd = new Random().Next(0, 10);
                                switch (rnd)
                                {
                                    case 1:
                                        ped.Health -= 50;
                                        Timer.Ragdoll(ped);
                                        break;
                                    case 2:
                                        goto case 1;
                                    case 3:
                                        goto case 1;
                                    case 4:
                                        ped.Kill();
                                        break;
                                    default:
                                        ped.Health -= 80;
                                        Timer.PedReact(ped);
                                        break;
                                }

                                Game.LogTrivial("Deadly Weapons: " + Functions.GetPersonaForPed(ped).FullName +
                                                " rolled 2-" + rnd);
                            }

                            NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                        }
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("Deadly Weapons Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Deadly Weapons Error Report End");
            }
        }
    }
}