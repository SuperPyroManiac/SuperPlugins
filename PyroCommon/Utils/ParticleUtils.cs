﻿using Rage;
using Rage.Native;

namespace PyroCommon.Utils;

internal static class ParticleUtils
{
    internal static void InitParticles()
    {
        var particleDict = new[]
        {
            "scr_trevor3", // Large Fire/Smoke
            "scr_agencyheistb" // Misty Smoke
            ,
        };
        foreach (var part in particleDict)
            GameFiber.StartNew(() => LoadParticles(part));
    }

    private static void LoadParticles(string dictName)
    {
        NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET(dictName);
        GameFiber.WaitUntil(() => NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(dictName), 5000);
        if (!NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(dictName))
            LogUtils.Info($"Issue loading {dictName} PTFX asset");
    }

    internal static int StartLoopedParticlesOnEntity(
        string dictName,
        string partName,
        Entity entity,
        Vector3 offset,
        Vector3 rotation,
        float scale
    )
    {
        NativeFunction.Natives.USE_PARTICLE_FX_ASSET(dictName);
        return NativeFunction.Natives.START_PARTICLE_FX_LOOPED_ON_ENTITY<int>(
            partName,
            entity,
            offset,
            rotation,
            scale,
            false,
            false,
            false
        );
    }

    internal static void StopLoopedParticles(int handle)
    {
        if (NativeFunction.Natives.DOES_PARTICLE_FX_LOOPED_EXIST<bool>(handle))
            NativeFunction.Natives.STOP_PARTICLE_FX_LOOPED(handle, false);
    }
}
