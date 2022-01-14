#region

using System;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class PedCustomAI
    {
        internal static void PedReact(Ped ped)
        {
            GameFiber.StartNew(delegate
            {
                Game.LogTrivial("DeadlyWeapons: PedAI Started...");
                var rnd = new Random().Next(0, 8);
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
            });
        }
    }
}