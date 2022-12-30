#region

using Rage;

#endregion

namespace SuperCalloutsLegacy.CustomScenes;

internal static class PrisonbreakSetup
{
    internal static void ConstructPrisonBreakSetupScene(out Ped prisoner1, out Ped prisoner5, out Ped prisoner3,
        out Ped prisoner2, out Ped prisoner4)
    {
        prisoner1 = new Ped("S_M_Y_PRISMUSCL_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("PRISONERS"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1970.794f, 2624.078f, 46.00704f)
        };
        prisoner1.SetVariation(0, 0, 2);
        prisoner1.SetVariation(2, 1, 0);
        prisoner1.SetVariation(3, 0, 2);
        prisoner1.SetVariation(4, 1, 0);
        prisoner1.SetVariation(10, 1, 0);
        prisoner1.Inventory.Weapons.Add(WeaponHash.Knife).Ammo = 0;
        prisoner1.Tasks.ClearImmediately();
        prisoner1.Heading = 0f;
        prisoner5 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("PRISONERS"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1969.886f, 2625.143f, 46.01746f)
        };
        prisoner5.SetVariation(0, 0, 2);
        prisoner5.SetVariation(2, 2, 0);
        prisoner5.SetVariation(3, 0, 0);
        prisoner5.SetVariation(4, 0, 0);
        prisoner5.SetVariation(10, 1, 0);
        prisoner5.Tasks.ClearImmediately();
        prisoner5.Heading = 0f;
        prisoner3 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("PRISONERS"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1972.917f, 2624.884f, 45.96499f)
        };
        prisoner3.SetVariation(0, 2, 0);
        prisoner3.SetVariation(2, 0, 2);
        prisoner3.SetVariation(3, 1, 5);
        prisoner3.SetVariation(4, 1, 0);
        prisoner3.SetVariation(10, 1, 0);
        prisoner3.Inventory.Weapons.Add(WeaponHash.Flashlight).Ammo = 0;
        prisoner3.Tasks.ClearImmediately();
        prisoner3.Heading = 0f;
        prisoner2 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("PRISONERS"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1971.28f, 2625.871f, 46.04052f)
        };
        prisoner2.SetVariation(0, 1, 1);
        prisoner2.SetVariation(2, 0, 1);
        prisoner2.SetVariation(3, 1, 3);
        prisoner2.SetVariation(4, 1, 0);
        prisoner2.SetVariation(10, 1, 0);
        prisoner2.Inventory.Weapons.Add(WeaponHash.Nightstick).Ammo = 0;
        prisoner2.Tasks.ClearImmediately();
        prisoner2.Heading = 0f;
        prisoner4 = new Ped("S_M_Y_PRISMUSCL_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("PRISONERS"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1969.529f, 2623.888f, 45.99443f)
        };
        prisoner4.SetVariation(0, 1, 1);
        prisoner4.SetVariation(2, 2, 0);
        prisoner4.SetVariation(3, 1, 0);
        prisoner4.SetVariation(4, 1, 1);
        prisoner4.SetVariation(10, 1, 0);
        prisoner4.Inventory.Weapons.Add(WeaponHash.Flashlight).Ammo = 0;
        prisoner4.Tasks.ClearImmediately();
        prisoner4.Heading = 0f;
        Game.SetRelationshipBetweenRelationshipGroups("PRISONERS", "COP", Relationship.Hate);
    }
}