#region

using System;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace DeadlyWeaponsLegacy.DFunctions
{
    internal static class SimpleFunctions
    {
        internal static void Ragdoll(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    if (!ped) return;
                    ped.IsRagdoll = true;
                    GameFiber.Wait(2000);
                    if (!ped) return;
                    ped.IsRagdoll = false;
                });
            }
            catch (Exception)
            {
                Game.LogTrivial("Deadly Weapons: Unable to remove ragdoll. Most likely the subject died first.");
            }
        }

        internal static Ped SetWanted(Ped wPed, bool isWanted = true)
        {
            var thePersona = Functions.GetPersonaForPed(wPed);
            thePersona.Wanted = isWanted;
            return wPed;
        }

        internal static bool IsWanted(Ped oPed)
        {
            var persona = Functions.GetPersonaForPed(oPed);
            Game.LogTrivial("Ped is Wanted? = " + persona.Wanted);
            return persona.Wanted;
        }

        internal static void SetDrunk(Ped bad, bool isDrunk)
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Yield();
                bad.Metadata.stpAlcoholDetected = isDrunk;
                var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
                drunkAnimset.LoadAndWait();
                bad.MovementAnimationSet = drunkAnimset;
                NativeFunction.Natives.x95D2D383D5396B8A(bad, isDrunk);
            });
        }
    }
}