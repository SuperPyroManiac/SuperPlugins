using System;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons2.DFunctions
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
            catch (Exception e)
            {
                Game.LogTrivial("Deadly Weapons: Unable to remove ragdoll. Most likely the subject died first.");
            }
        }
        
        internal static Ped SetWanted(Ped wPed, bool isWanted = true)
        {
            Persona thePersona = Functions.GetPersonaForPed(wPed);
            thePersona.Wanted = isWanted;
            return wPed;
        }

        internal static bool IsWanted(Ped oPed)
        {
            Persona persona = Functions.GetPersonaForPed(oPed);
            Game.LogTrivial("Ped is Wanted? = " + persona.Wanted);
            return persona.Wanted;
        }

        internal static void SetDrunk(Ped Bad, bool isDrunk)
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Yield();
                Bad.Metadata.stpAlcoholDetected = isDrunk;
                var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
                drunkAnimset.LoadAndWait();
                Bad.MovementAnimationSet = drunkAnimset;
                Rage.Native.NativeFunction.Natives.SET_PED_IS_DRUNK(Bad, isDrunk);
            });
        }
    }
}