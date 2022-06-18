#region

using System;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class PedCustomAi
    {
        internal static void PedReact(Ped ped)
        {
            if (!ped.Exists() || ped.IsDead) return;
            GameFiber.StartNew(delegate
            {
                Game.LogTrivial("DeadlyWeapons: PedAI Started...");
                var rnd = new Random().Next(0, 8);
                try
                {
                    switch (rnd)
                    {
                        case 0:
                            Game.LogTrivial("Deadly Weapons: " +
                                            Functions.GetPersonaForPed(ped).FullName +
                                            " is fleeing!");
                            ped.BlockPermanentEvents = true;
                            ped.IsPersistent = true;
                            ped.Tasks.ClearImmediately();
                            ped.Tasks.Flee(Game.LocalPlayer.Character, 120, 20000);
                            GameFiber.Wait(15000);
                            if (ped)
                            {
                                ped.BlockPermanentEvents = false;
                                ped.IsPersistent = false;
                            }

                            break;
                        case 1:
                            Game.LogTrivial("Deadly Weapons: " +
                                            Functions.GetPersonaForPed(ped).FullName +
                                            " is hiding!");
                            ped.BlockPermanentEvents = true;
                            ped.IsPersistent = true;
                            ped.Tasks.ClearImmediately();
                            ped.Tasks.TakeCoverFrom(Game.LocalPlayer.Character, 20000, false);
                            GameFiber.Wait(15000);
                            if (ped)
                            {
                                ped.BlockPermanentEvents = false;
                                ped.IsPersistent = false;
                            }

                            break;
                        case 2:
                            Game.LogTrivial("Deadly Weapons: " +
                                            Functions.GetPersonaForPed(ped).FullName +
                                            " is cowering!");
                            ped.BlockPermanentEvents = true;
                            ped.IsPersistent = true;
                            ped.Tasks.ClearImmediately();
                            ped.Tasks.Cower(20000);
                            GameFiber.Wait(15000);
                            if (ped)
                            {
                                ped.BlockPermanentEvents = false;
                                ped.IsPersistent = false;
                            }

                            break;
                    }
                }
                catch (Exception e)
                {
                    Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                    Game.LogTrivial("Deadly Weapons Error Report Start");
                    Game.LogTrivial("======================================================");
                    Game.LogTrivial(e.ToString());
                    Game.LogTrivial("======================================================");
                    Game.LogTrivial("Deadly Weapons Error Report End");
                }
            });
        }
    }
}