using System;
using System.Reflection;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using PyroCommon.API.Wrappers;
using Rage;
using Rage.Native;

namespace PyroCommon.API;

public static class EntityExtensions
{
    private static Ped GetPedFromTaskInvoker(this TaskInvoker taskInvoker)
    {
        var property = taskInvoker.GetType().GetProperty("Ped", BindingFlags.Instance | BindingFlags.NonPublic);
        return property != null ? (Ped)property.GetMethod.Invoke(taskInvoker, null) : null;
    }
    
    // PED EXTENSIONS
    public static void SetWalkAnimation(this Ped ped, Enums.ScAnimationsSet animationsSet)
    {
        string chosenSet = null;
        switch (animationsSet)
        {
            case Enums.ScAnimationsSet.Drunk:
                chosenSet = "move_m@drunk@verydrunk";
                break;
            case Enums.ScAnimationsSet.Injured:
                chosenSet = "move_injured_ground";
                break;
        }
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            var anim = new AnimationSet(chosenSet);
            anim.LoadAndWait();
            ped.MovementAnimationSet = anim;
        });
    }
    
    public static void SetWanted(this Ped ped, bool isWanted)
    {
        if (!ped.Exists()) return;
        var pedP = Functions.GetPersonaForPed(ped).Wanted = isWanted;
    }
    
    public static bool GetWanted(this Ped ped)
    {
        return Functions.GetPersonaForPed(ped).Wanted;
    }

    public static void SetDrunk(this Ped ped, Enums.DrunkState drunkState)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            if (!ped.Exists()) return;
            if (Main.UsingPr)
            {
                PedInfo.SetDrunk(ped, drunkState);
                return;
            }
            ped.Metadata.stpAlcoholDetected = true;
            ped.SetWalkAnimation(Enums.ScAnimationsSet.Drunk);
            NativeFunction.Natives.x95D2D383D5396B8A(ped, true);
        });
    }
    
    public static void SetLicenseStatus(this Ped ped, Enums.Permits permits, Enums.PermitStatus status)
    {
        if (PyroCommon.Main.UsingPr) PedInfo.SetPermit(ped, permits, status);
        switch (permits)
        {
            case Enums.Permits.Drivers:
                switch (status)
                {
                    case Enums.PermitStatus.None:
                        Functions.GetPersonaForPed(ped).ELicenseState = ELicenseState.None;
                        break;
                    case Enums.PermitStatus.Revoked:
                        Functions.GetPersonaForPed(ped).ELicenseState = ELicenseState.Suspended;
                        break;
                    case Enums.PermitStatus.Expired:
                        Functions.GetPersonaForPed(ped).ELicenseState = ELicenseState.Expired;
                        break;
                    case Enums.PermitStatus.Valid:
                        Functions.GetPersonaForPed(ped).ELicenseState = ELicenseState.Valid;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
                break;
            case Enums.Permits.Guns:
                switch (status)
                {
                    case Enums.PermitStatus.None:
                    case Enums.PermitStatus.Revoked:
                    case Enums.PermitStatus.Expired:
                        ped.Metadata.hasGunPermit = false;
                        break;
                    case Enums.PermitStatus.Valid:
                        ped.Metadata.hasGunPermit = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
                ped.Metadata.hasGunPermit = false;
                break;
            case Enums.Permits.Hunting:
            case Enums.Permits.Fishing:
            default:
                throw new ArgumentOutOfRangeException(nameof(permits), permits, null);
        }
    }
    
    // TASK EXTENSIONS
    public static void FaceEntity(this TaskInvoker taskInvoker, Entity target, int duration)
    {
        NativeFunction.Natives.x5AD23D40115353AC(taskInvoker.GetPedFromTaskInvoker(), target, duration);
    }

    public static void Ragdoll(this TaskInvoker taskInvoker)
    {
        try
        {
            var ped = taskInvoker.GetPedFromTaskInvoker();
            NativeFunction.Natives.xD76632D99E4966C8(ped, 1000, 3500, 0, ped.Direction, World.GetGroundZ(ped.Position, false, false), 0, 0);
        }
        catch (Exception e)
        {
            Log.Info("Ragdoll issue.");
            Log.Error(e.ToString());
        }
    }
    
    // VEHICLE EXTENSIONS
    public static void ApplyDamage(this Vehicle vehicle, float radius, float amount)
    {
        var model = vehicle.Model;
        model.GetDimensions(out var vector31, out var vector32);
        var num = new Random(DateTime.Now.Millisecond).Next(10, 45);
        for (var index = 0; index < num; ++index)
        {
            var randomInt1 = MathHelper.GetRandomSingle(vector31.X, vector32.X);
            var randomInt2 = MathHelper.GetRandomSingle(vector31.Y, vector32.Y);
            var randomInt3 = MathHelper.GetRandomSingle(vector31.Z, vector32.Z);
            vehicle.Deform(new Vector3(randomInt1, randomInt2, randomInt3), radius, amount);
        }
    }

    public static void StartFire(this Vehicle vehicle, bool possibleExplode)
    {
        NativeFunction.Natives.x45F6D8EEF34ABEF1(vehicle, -1f);
        
        if (!possibleExplode) return;
        if (new Random(DateTime.Now.Millisecond).Next(1, 4) == 2) NativeFunction.Natives.x70DB57649FA8D0D8(vehicle, -1f);
    }
}