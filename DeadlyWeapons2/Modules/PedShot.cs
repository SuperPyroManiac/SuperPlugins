#region

using System;
using System.Collections.Generic;
using DeadlyWeapons2.DFunctions;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons2.Modules
{
    internal class PedShot
    {
        //private static List<Ped> _possibleTargets = new List<Ped>();

        /*internal void StartPedEvent()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Checks();
                    GameFiber.Wait(100);
                }
            });
            Game.LogTrivial("DeadlyWeapons: Starting CustomAIFiber.");
        }

        private void Checks()
        {
            foreach (var ped in _possibleTargets)
            {
                try
                {
                    if (ped.IsDead || ped.IsDiving || ped.IsCuffed ||
                        ped.DistanceTo(Game.LocalPlayer.Character) > 200f)
                    {
                        _possibleTargets.Remove(ped);
                        Game.LogTrivial("DeadlyWeapons: DEBUG!!! !!!! !!!!");
                        Game.LogTrivial("DeadlyWeapons: Removed " + Functions.GetPersonaForPed(ped).FullName + " from the check list!");
                        return;
                    }
                    else
                    {
                        PedAi(ped);
                    }
                }
                catch (Exception e)
                {
                    Game.LogTrivial("DeadlyWeapons: Failed to remove ped from list. Skipping.");
                }
            }
        }

        internal static void PedAimedAt(Ped ped)
        {
            if (_possibleTargets.Contains(ped)) return;
            _possibleTargets.Add(ped);
            Game.LogTrivial("DeadlyWeapons: DEBUG!!! !!!! !!!!");
            Game.LogTrivial("DeadlyWeapons: Added " + Functions.GetPersonaForPed(ped).FullName + " to the check list!");
        }*/
        
        internal static void PedAi(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    if (!ped)
                    {
                        Game.LogTrivial("DeadlyWeapons: !!! DEBUG !!! : PED IS NULL? Hopefully we don't see this spammed....");
                        return;
                    }
                    if (ped.IsDead) return;
                    ped.Accuracy = Settings.AiAccuracy;
                    if (Game.LocalPlayer.Character.IsRagdoll)
                    {
                        ped.Tasks.Flee(Game.LocalPlayer.Character, 50, 10);
                    }

                    foreach (var w in WeaponHashs.WeaponHashes)
                        if (NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, (uint) w, 0) &&
                                Settings.EnableDamageSystem)
                            {
                                if (ped.LastDamageBone == PedBoneId.LeftUpperArm ||
                                    ped.LastDamageBone == PedBoneId.LeftForeArm ||
                                    ped.LastDamageBone == PedBoneId.RightUpperArm ||
                                    ped.LastDamageBone == PedBoneId.RightForearm)
                                {
                                    if (!ped) return;
                                    if (ped.Inventory.HasLoadedWeapon) ped.Inventory.EquippedWeapon.Drop();
                                }

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
                                            SimpleFunctions.Ragdoll(ped);
                                            break;
                                        case 3:
                                            ped.Health = 80;
                                            ped.Armor = 0;
                                            PedCustomAI.PedReact(ped);
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
                                            SimpleFunctions.Ragdoll(ped);
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
                                            PedCustomAI.PedReact(ped);
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
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("Deadly Weapons Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Deadly Weapons Error Report End");
            }
        }
    }
}