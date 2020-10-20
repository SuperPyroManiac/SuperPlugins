#region

using Rage;

#endregion

namespace SuperCallouts2.CustomScenes
{
    //This class will be used as a refference to Prison Break class. Just here to keep things clean in the main classes.
    //This is also a test of an in game script maker to allow me to precisly make custom scenes. It's messy and gross but works.
    internal class PrisonbreakSetup
    {
        /* internal Ped Prisoner1;
         internal Ped Prisoner5;
         internal Ped Prisoner3;
         internal Ped Prisoner2;
         internal Ped Prisoner4;*/
        // ConstructPrisonBreakSetupScene(out Prisoner1, out Prisoner5, out Prisoner3, out Prisoner2, out Prisoner4);
        internal static void ConstructPrisonBreakSetupScene(out Ped prisoner1, out Ped prisoner5, out Ped prisoner3,
            out Ped prisoner2, out Ped prisoner4)
        {
            prisoner1 = new Ped("S_M_Y_PRISMUSCL_01", Vector3.Zero, 0f);
            prisoner1.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            prisoner1.Money = 0;
            prisoner1.RelationshipGroup = new RelationshipGroup("PRISONERS");
            prisoner1.CollisionIgnoredEntity = null;
            prisoner1.AngularVelocity = new Rotator(0f, 0f, 0f);
            prisoner1.Velocity = new Vector3(0f, 0f, 0f);
            prisoner1.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            prisoner1.Position = new Vector3(1970.794f, 2624.078f, 46.00704f);
            prisoner1.SetVariation(0, 0, 2);
            prisoner1.SetVariation(2, 1, 0);
            prisoner1.SetVariation(3, 0, 2);
            prisoner1.SetVariation(4, 1, 0);
            prisoner1.SetVariation(10, 1, 0);
            prisoner1.Inventory.Weapons.Add(WeaponHash.Knife).Ammo = 0;
            prisoner1.Tasks.ClearImmediately();
            prisoner1.Heading = 0f;
            prisoner5 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f);
            prisoner5.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            prisoner5.Money = 0;
            prisoner5.RelationshipGroup = new RelationshipGroup("PRISONERS");
            prisoner5.CollisionIgnoredEntity = null;
            prisoner5.AngularVelocity = new Rotator(0f, 0f, 0f);
            prisoner5.Velocity = new Vector3(0f, 0f, 0f);
            prisoner5.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            prisoner5.Position = new Vector3(1969.886f, 2625.143f, 46.01746f);
            prisoner5.SetVariation(0, 0, 2);
            prisoner5.SetVariation(2, 2, 0);
            prisoner5.SetVariation(3, 0, 0);
            prisoner5.SetVariation(4, 0, 0);
            prisoner5.SetVariation(10, 1, 0);
            prisoner5.Tasks.ClearImmediately();
            prisoner5.Heading = 0f;
            prisoner3 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f);
            prisoner3.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            prisoner3.Money = 0;
            prisoner3.RelationshipGroup = new RelationshipGroup("PRISONERS");
            prisoner3.CollisionIgnoredEntity = null;
            prisoner3.AngularVelocity = new Rotator(0f, 0f, 0f);
            prisoner3.Velocity = new Vector3(0f, 0f, 0f);
            prisoner3.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            prisoner3.Position = new Vector3(1972.917f, 2624.884f, 45.96499f);
            prisoner3.SetVariation(0, 2, 0);
            prisoner3.SetVariation(2, 0, 2);
            prisoner3.SetVariation(3, 1, 5);
            prisoner3.SetVariation(4, 1, 0);
            prisoner3.SetVariation(10, 1, 0);
            prisoner3.Inventory.Weapons.Add(WeaponHash.Flashlight).Ammo = 0;
            prisoner3.Tasks.ClearImmediately();
            prisoner3.Heading = 0f;
            prisoner2 = new Ped("S_M_Y_PRISONER_01", Vector3.Zero, 0f);
            prisoner2.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            prisoner2.Money = 0;
            prisoner2.RelationshipGroup = new RelationshipGroup("PRISONERS");
            prisoner2.CollisionIgnoredEntity = null;
            prisoner2.AngularVelocity = new Rotator(0f, 0f, 0f);
            prisoner2.Velocity = new Vector3(0f, 0f, 0f);
            prisoner2.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            prisoner2.Position = new Vector3(1971.28f, 2625.871f, 46.04052f);
            prisoner2.SetVariation(0, 1, 1);
            prisoner2.SetVariation(2, 0, 1);
            prisoner2.SetVariation(3, 1, 3);
            prisoner2.SetVariation(4, 1, 0);
            prisoner2.SetVariation(10, 1, 0);
            prisoner2.Inventory.Weapons.Add(WeaponHash.Nightstick).Ammo = 0;
            prisoner2.Tasks.ClearImmediately();
            prisoner2.Heading = 0f;
            prisoner4 = new Ped("S_M_Y_PRISMUSCL_01", Vector3.Zero, 0f);
            prisoner4.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            prisoner4.Money = 0;
            prisoner4.RelationshipGroup = new RelationshipGroup("PRISONERS");
            prisoner4.CollisionIgnoredEntity = null;
            prisoner4.AngularVelocity = new Rotator(0f, 0f, 0f);
            prisoner4.Velocity = new Vector3(0f, 0f, 0f);
            prisoner4.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            prisoner4.Position = new Vector3(1969.529f, 2623.888f, 45.99443f);
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
}