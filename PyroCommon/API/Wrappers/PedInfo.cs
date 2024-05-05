using System;
using PolicingRedefined.Interaction.Assets.PedAttributes;
using Rage;

namespace PyroCommon.API.Wrappers;

public static class PedInfo
{
    public enum ScAnimationsSet
    {
        Drunk,
        Injured
    }
    public enum DrunkState
    {
        Tipsy,
        ModeratelyDrunk,
        VeryDrunk,
        ExtremelyDrunk,
        Sloshed
    }

    internal static void SetDrunk(Ped ped, DrunkState drunkState)
    {
        switch (drunkState)
        {
            case DrunkState.Tipsy:
                PolicingRedefined.API.PedAPI.SetPedDrunk(ped, EDrunkLevel.Tipsy);
                break;
            case DrunkState.ModeratelyDrunk:
                PolicingRedefined.API.PedAPI.SetPedDrunk(ped, EDrunkLevel.ModeratelyDrunk);
                break;
            case DrunkState.VeryDrunk:
                PolicingRedefined.API.PedAPI.SetPedDrunk(ped, EDrunkLevel.VeryDrunk);
                break;
            case DrunkState.ExtremelyDrunk:
                PolicingRedefined.API.PedAPI.SetPedDrunk(ped, EDrunkLevel.Wasted);
                break;
            case DrunkState.Sloshed:
                PolicingRedefined.API.PedAPI.SetPedDrunk(ped, EDrunkLevel.ShouldBeDead);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(drunkState), drunkState, null);
        }
    }
    
}