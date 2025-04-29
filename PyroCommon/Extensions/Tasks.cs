using System;
using System.Reflection;
using PyroCommon.Utils;
using Rage;
using Rage.Native;

namespace PyroCommon.Extensions;

public static class Tasks
{
    private static Ped GetPedFromTaskInvoker(this TaskInvoker taskInvoker)
    {
        try
        {
            var property = taskInvoker.GetType().GetProperty("Ped", BindingFlags.Instance | BindingFlags.NonPublic);
            return property != null ? (Ped)property.GetMethod.Invoke(taskInvoker, null) : null;
        }
        catch (Exception e)
        {
            LogUtils.Warning($".Failed to get ped from task!\r\n{e}");
            return null;
        }
    }

    public static void FaceEntity(this TaskInvoker taskInvoker, Entity target, int duration = -1)
    {
        var ped = taskInvoker.GetPedFromTaskInvoker();
        if (ped)
            NativeFunction.Natives.x5AD23D40115353AC(ped, target, duration);
    }

    public static void GoToEntity(this TaskInvoker taskInvoker, Entity target)
    {
        var ped = taskInvoker.GetPedFromTaskInvoker();
        if (ped)
            NativeFunction.Natives.x6A071245EB0D1882(ped, target, -1, 2f, 2f, 0, 0);
    }

    public static void Ragdoll(this TaskInvoker taskInvoker)
    {
        try
        {
            var ped = taskInvoker.GetPedFromTaskInvoker();
            if (ped)
                NativeFunction.Natives.xD76632D99E4966C8(
                    ped,
                    1000,
                    3500,
                    0,
                    ped.Direction,
                    World.GetGroundZ(ped.Position, false, false),
                    0,
                    0
                );
        }
        catch (Exception e)
        {
            LogUtils.Info("Ragdoll issue.");
            LogUtils.Error(e.ToString());
        }
    }
}
