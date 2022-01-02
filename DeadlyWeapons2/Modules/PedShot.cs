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
        internal static void PedAi(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    if (!ped) return;
                    if (ped.IsDead) return;
                    ped.Accuracy = Settings.AiAccuracy;
                    if (Game.LocalPlayer.Character.IsRagdoll)
                    {
                        ped.Tasks.Flee(Game.LocalPlayer.Character, 50, 10);
                    }

                    foreach (var w in WeaponHashs.WeaponHashes)
                        if (NativeFunction.Natives.x131D401334815E94<bool>(ped, (uint) w, 0) &&
                                Settings.EnableDamageSystem) //Has_Entity_Been_Damaged_By_Weapon
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

                                NativeFunction.Natives.xAC678E40BE7C74D2(ped); //CLEAR_ENTITY_LAST_WEAPON_DAMAGE
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