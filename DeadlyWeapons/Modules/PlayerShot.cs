#region

using System;
using DeadlyWeapons.DFunctions;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeapons.Modules
{
    internal class PlayerShot
    {
        private GameFiber _playerShotFiber;
        private Ped Player => Game.LocalPlayer.Character;

        internal void StartEvent()
        {
            _playerShotFiber = new GameFiber(delegate
            {
                while (true)
                {
                    PlayerShotEvent();
                    GameFiber.Yield();
                }
            });
            Game.LogTrivial("DeadlyWeapons: Starting PlayerShotFiber.");
            _playerShotFiber.Start();
        }

        private void PlayerShotEvent()
        {
            foreach (var w in WeaponHashs.WeaponHashes)
                if (NativeFunction.Natives.x131D401334815E94<bool>(Player, (uint) w, 0) &&
                    Settings.EnableDamageSystem)
                {
                    try
                    {
                        var rnd = new Random().Next(1, 5);

                        if (Player.Armor < 5)
                        {
                            Game.LogTrivial("Deadly Weapons: Player shot, chose: 0 - " + rnd);

                            switch (rnd)
                            {
                                case 1:
                                    Player.Health = 5;
                                    break;
                                case 2:
                                    Player.Kill();
                                    break;
                                case 3:
                                    Player.Health -= 40;
                                    break;
                                case 4:
                                    Player.Health -= 50;
                                    SimpleFunctions.Ragdoll(Player);
                                    break;
                            }
                        }
                        
                        if (Player.Armor >= 5)
                        {
                            Game.LogTrivial("Deadly Weapons: Player shot, chose: 1 - " + rnd);

                            switch (rnd)
                            {
                                case 1:
                                    Player.Armor = 0;
                                    break;
                                case 2:
                                    Player.Health -= 45;
                                    Player.Armor = 0;
                                    SimpleFunctions.Ragdoll(Player);
                                    break;
                                case 3:
                                    Player.Armor -= 35;
                                    break;
                                case 4:
                                    Player.Armor -= 45;
                                    break;
                            }
                        }

                        NativeFunction.Natives.xAC678E40BE7C74D2(Player); //CLEAR_ENTITY_LAST_WEAPON_DAMAGE
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
}