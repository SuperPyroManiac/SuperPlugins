using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using PyroCommon.Models;
using PyroCommon.Utils;
using Rage;
using Rage.Native;

namespace PyroCommon.Extensions;

public static class Peds
{
    public static void SetWalkAnimation(this Ped ped, Enums.ScAnimationsSet animationsSet)
    {
        var chosenSet = string.Empty;
        switch (animationsSet)
        {
            case Enums.ScAnimationsSet.Drunk:
                chosenSet = "move_m@drunk@verydrunk";
                break;
            case Enums.ScAnimationsSet.Injured:
                chosenSet = "move_injured_ground";
                break;
        }
        GameFiber.StartNew(
            delegate
            {
                GameFiber.Yield();
                var anim = new AnimationSet(chosenSet);
                anim.LoadAndWait();
                ped.MovementAnimationSet = anim;
            }
        );
    }

    public static void SetResistance(
        this Ped ped,
        Enums.ResistanceAction resistanceAction,
        bool walkAway = false,
        int resistanceChance = 50
    )
    {
        // if (Main.UsingPr)
        // {
        //     PedInfo.SetResistance(ped, resistanceAction, walkAway, resistanceChance);
        //     return;
        // }
        switch (resistanceAction)
        {
            case Enums.ResistanceAction.Flee:
                CommonUtils.StartPursuit(false, false, ped);
                break;
            case Enums.ResistanceAction.Attack:
                ped.RelationshipGroup = new RelationshipGroup("ANGRY");
                Game.SetRelationshipBetweenRelationshipGroups("ANGRY", "COP", Relationship.Hate);
                ped.Tasks.FightAgainstClosestHatedTarget(50f);
                break;
            case Enums.ResistanceAction.Uncooperative:
                ped.RelationshipGroup = new RelationshipGroup("ANGRY");
                Game.SetRelationshipBetweenRelationshipGroups("ANGRY", "COP", Relationship.Dislike);
                break;
            case Enums.ResistanceAction.None:
                break;
        }
    }

    public static void SetWanted(this Ped ped, bool isWanted)
    {
        if (!ped.Exists())
            return;
        Functions.GetPersonaForPed(ped).Wanted = isWanted;
    }

    public static bool GetWanted(this Ped ped)
    {
        return Functions.GetPersonaForPed(ped).Wanted;
    }

    public static void SetDrunk(this Ped ped, Enums.DrunkState drunkState)
    {
        if (!ped)
            return;
        GameFiber.StartNew(
            delegate
            {
                GameFiber.Yield();
                // if (Main.UsingPr)
                // {
                //     PedInfo.SetDrunk(ped, drunkState);
                //     return;
                // }
                ped.Metadata.stpAlcoholDetected = true;
                ped.SetWalkAnimation(Enums.ScAnimationsSet.Drunk);
                NativeFunction.Natives.x95D2D383D5396B8A(ped, true);
            }
        );
    }

    public static void SetLicenseStatus(this Ped ped, Enums.Permits permits, Enums.PermitStatus status)
    {
        //if (Main.UsingPr) PedInfo.SetPermit(ped, permits, status);
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
                }
                break;
            case Enums.Permits.Guns:
                switch (status)
                {
                    case Enums.PermitStatus.None
                    or Enums.PermitStatus.Revoked
                    or Enums.PermitStatus.Expired:
                        ped.Metadata.hasGunPermit = false;
                        break;
                    case Enums.PermitStatus.Valid:
                        ped.Metadata.hasGunPermit = true;
                        break;
                }
                ped.Metadata.hasGunPermit = false;
                break;
            case Enums.Permits.Hunting:
                break;
            case Enums.Permits.Fishing:
                break;
        }
    }
}
