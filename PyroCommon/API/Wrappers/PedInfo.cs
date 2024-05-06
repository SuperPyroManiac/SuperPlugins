using System;
using PolicingRedefined.Interaction.Assets.PedAttributes;
using Rage;

namespace PyroCommon.API.Wrappers;

public static class PedInfo
{
    internal static void SetDrunk(Ped ped, Enums.DrunkState drunkState)
    {
        PolicingRedefined.API.PedAPI.SetPedDrunk(ped, (EDrunkLevel)drunkState);
    }

    internal static void SetPermit(Ped ped, Enums.Permits permit, Enums.PermitStatus status)
    {
        switch (permit)
        {
            case Enums.Permits.Guns:
                if (Main.UsingPr) PolicingRedefined.API.PedDocumentationAPI.SetWeaponPermitStatus(ped, (EDocumentStatus)status);
                break;
            case Enums.Permits.Hunting:
                if (Main.UsingPr) PolicingRedefined.API.PedDocumentationAPI.SetHuntingPermitStatus(ped, (EDocumentStatus)status);
                break;
            case Enums.Permits.Fishing:
                if (Main.UsingPr) PolicingRedefined.API.PedDocumentationAPI.SetFishingPermitStatus(ped, (EDocumentStatus)status);
                break;
            case Enums.Permits.Drivers:
            default:
                throw new ArgumentOutOfRangeException(nameof(permit), permit, null);
        }
    }
}