#region

using System;
using DeadlyWeapons.DFunctions;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons
{
    internal class DeadlyWeapons
    {
        internal static readonly WeaponHash[] WeaponHashes =
        {
            WeaponHash.CombatPistol,
            WeaponHash.Pistol50,
            WeaponHash.APPistol,
            WeaponHash.Pistol,
            WeaponHash.AssaultSMG,
            WeaponHash.MicroSMG,
            WeaponHash.Smg,
            WeaponHash.MG,
            WeaponHash.CombatMG,
            WeaponHash.AdvancedRifle,
            WeaponHash.AssaultRifle,
            WeaponHash.CarbineRifle,
            WeaponHash.HeavySniper,
            WeaponHash.SniperRifle,
            WeaponHash.AssaultShotgun,
            WeaponHash.BullpupShotgun,
            WeaponHash.PumpShotgun,
            WeaponHash.SawnOffShotgun,
            WeaponHash.Minigun,
            (WeaponHash) 0x61012683, // WEAPON_GUSENBERG
            (WeaponHash) 0xC0A3098D, // WEAPON_SPECIALCARBINE
            (WeaponHash) 0xD205520E, // WEAPON_HEAVYPISTOL
            (WeaponHash) 0xBFD21232, // WEAPON_SNSPISTOL
            (WeaponHash) 0x7F229F94, // WEAPON_BULLPUPRIFLE
            (WeaponHash) 0x83839C4, // WEAPON_VINTAGEPISTOL
            (WeaponHash) 0xA89CB99E, // WEAPON_MUSKET
            (WeaponHash) 0x3AABBBAA, // WEAPON_HEAVYSHOTGUN
            (WeaponHash) 0xC734385A, // WEAPON_MARKSMANRIFLE
            (WeaponHash) 0xAB564B93, // WEAPON_PROXMINE
            (WeaponHash) 0xA3D4D34, // WEAPON_COMBATPDW
            (WeaponHash) 0xDC4DB296 // WEAPON_MARKSMANPISTOL
        };

        internal GameFiber ProcessFiber;
        private Ped Player => Game.LocalPlayer.Character;

        internal void Start()
        {
            try
            {
                ProcessFiber = new GameFiber(delegate
                {
                    while (true)
                    {
                        PlayerShotEvent();
                        GameFiber.Yield();
                    }
                });
                ProcessFiber.Start();
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

        private void PlayerShotEvent()
        {
            if (Player.IsShooting && Player.Inventory.EquippedWeapon.Hash != WeaponHash.StunGun &&
                Player.Inventory.EquippedWeapon.Hash != WeaponHash.FireExtinguisher && Settings.EnablePanic)
                //Player shot their gun, panic!
                Timer.Panic();

            if (Settings.EnableBetterAi)
                //Lets get these peds to react better!
                try
                {
                    //var peds = Player.GetNearbyPeds(15).ToList();
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
                            CustomAI.PedAi(ped);
                    }
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

            foreach (var w in WeaponHashes)
                if (NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(Player, (uint) w, 0) &&
                    Settings.EnableDamageSystem)
                    try
                    {
                        if (Player.Armor >= 60)
//                         If player has armor:
//                         20% chance to be fine, and armor survive for 1 more use.
//                         10% chance to fall over.
//                         10% chance to injure player.
//                         80% chance for armor to be destroyed.
                        {
                            var rnd = new Random().Next(0, 10);
                            switch (rnd)
                            {
                                case 1:
                                    Player.Health = 100;
                                    Player.Armor = 61;
                                    break;
                                case 2:
                                    Player.Health = 100;
                                    Player.Armor = 61;
                                    Timer.Ragdoll(Player);
                                    break;
                                case 3:
                                    Player.Health = 80;
                                    Player.Armor = 0;
                                    break;
                                default:
                                    Player.Health = 100;
                                    Player.Armor = 0;
                                    break;
                            }
                        }
                        else
//                         If player has no armor:
//                         30% chance to loose half health and fall.
//                         10% chance to die.
//                         60% chance to loose 80% of health.
                        {
                            var rnd = new Random().Next(0, 10);
                            switch (rnd)
                            {
                                case 1:
                                    Player.Health -= 50;
                                    Timer.Ragdoll(Player);
                                    break;
                                case 2:
                                    goto case 1;
                                case 3:
                                    goto case 1;
                                case 4:
                                    Player.Kill();
                                    break;
                                default:
                                    Player.Health -= 80;
                                    break;
                            }
                        }

                        NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(Player);
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