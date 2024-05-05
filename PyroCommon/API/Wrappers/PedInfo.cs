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
        Tipsy = 0,
        ModeratelyDrunk = 1,
        VeryDrunk = 2,
        ExtremelyDrunk = 4,
        Sloshed = 8
    }

    internal static void SetDrunk(Ped ped, DrunkState drunkState)
    {
        PolicingRedefined.API.PedAPI.SetPedDrunk(ped, (EDrunkLevel)drunkState);
    }
    
}