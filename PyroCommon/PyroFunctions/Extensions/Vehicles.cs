using System;
using Rage;
using Rage.Native;

namespace PyroCommon.PyroFunctions.Extensions;

public static class Vehicles
{
    public static void ApplyDamage(this Vehicle vehicle, float radius, float amount)
    {
        var model = vehicle.Model;
        model.GetDimensions(out var vector31, out var vector32);
        var num = new Random(DateTime.Now.Millisecond).Next(10, 45);
        for ( var index = 0; index < num; ++index )
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

        if ( !possibleExplode ) return;
        if ( new Random(DateTime.Now.Millisecond).Next(1, 4) == 2 ) NativeFunction.Natives.x70DB57649FA8D0D8(vehicle, -1f);
    }
}