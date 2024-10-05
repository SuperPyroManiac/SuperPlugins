using System;
using System.Reflection;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using PyroCommon.Objects;
using PyroCommon.UIManager;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace PyroCommon.PyroFunctions;

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

    public static void SetResistance(this Ped ped, Enums.ResistanceAction resistanceAction, bool walkAway = false, int resistanceChance = 50)
    {
        // if (Main.UsingPr)
        // {
        //     PedInfo.SetResistance(ped, resistanceAction, walkAway, resistanceChance);
        //     return;
        // }
        switch (resistanceAction)
        {
            case Enums.ResistanceAction.Flee:
                PyroFunctions.StartPursuit(false, false, ped);
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
        if (!ped.Exists()) return;
        Functions.GetPersonaForPed(ped).Wanted = isWanted;
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
            // if (Main.UsingPr)
            // {
            //     PedInfo.SetDrunk(ped, drunkState);
            //     return;
            // }
            ped.Metadata.stpAlcoholDetected = true;
            ped.SetWalkAnimation(Enums.ScAnimationsSet.Drunk);
            NativeFunction.Natives.x95D2D383D5396B8A(ped, true);
        });
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
                    case Enums.PermitStatus.None or Enums.PermitStatus.Revoked or Enums.PermitStatus.Expired:
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
    
    // TASK EXTENSIONS
    public static void FaceEntity(this TaskInvoker taskInvoker, Entity target, int duration = -1)
    {
        NativeFunction.Natives.x5AD23D40115353AC(taskInvoker.GetPedFromTaskInvoker(), target, duration);
    }

    public static void GoToEntity(this TaskInvoker taskInvoker, Entity target)
    {
        var ped = taskInvoker.GetPedFromTaskInvoker();
        NativeFunction.Natives.x6A071245EB0D1882(ped, target, -1, 2f, 2f, 0, 0);
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
    
    //RNUI EXTENSIONS
    private static T WithTextEditingBase<T>(T item, int maxLength, Func<string> strGetter, Action<string> resultCallback) where T : UIMenuItem
    {
        item = item ?? throw new ArgumentNullException(nameof(item));
        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException("Length cannot be negative", nameof(maxLength));
        }

        item.Activated += (m, s) =>
        {
            Manager.MainMenuPool.Draw();
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "", "", strGetter(), "", "", "", maxLength);
            int state;
            while ((state = NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>()) == 0)
            {
                GameFiber.Yield();
                Manager.MainMenuPool.Draw();
            }
            if (state == 1)
            {
                string str = NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
                resultCallback(str);
            }
        };
        return item;
    }
    /// <summary>
    /// Allows to edit a string by selecting the item. The current string is displayed in the item's <see cref="UIMenuItem.RightLabel"/>.
    /// </summary>
    /// <param name="getter">Gets the string to display to the user.</param>
    /// <param name="setter">Takes the string edited by the user.</param>
    /// <param name="maxLength">The maximum length of the string.</param>
    /// <param name="maxLengthInItem">
    /// The maximum length of the string when set to the <see cref="UIMenuItem.RightLabel"/> property.
    /// If the string length exceeds this value, the string is cut and "..." is appended.
    /// </param>
    public static UIMenuItem WithTextEditing(this UIMenuItem item, Func<string> getter, Action<string> setter, int maxLengthInItem = 16, int maxLength = 32)
    {
        getter = getter ?? throw new ArgumentNullException(nameof(getter));
        setter = setter ?? throw new ArgumentNullException(nameof(setter));

        WithTextEditingBase(item, maxLength,
            getter,
            str =>
            {
                TrimAndSetRightLabel(item, str, maxLengthInItem);
                setter(str);
            });

        TrimAndSetRightLabel(item, getter(), maxLengthInItem);
        return item;

        static void TrimAndSetRightLabel(UIMenuItem item, string str, int maxLength)
            => item.RightLabel = str.Length > maxLength ? (str.Substring(0, maxLength) + "...") : str;
    }
}