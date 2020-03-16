#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes
{
    //This class will be used as a refference to Mafia Robbery class. Just here to keep things clean in the main classes.
    //This is also a test of an in game script maker to allow me to precisly make custom scenes. It's messy and gross but works.
    public class MafiaSetup
    {
        /*public Ped MafiaDudes1;
        public Ped MafiaDudes2;
        public Ped MafiaDudes3;
        public Ped MafiaDudes4;
        public Ped MafiaDudes5;
        public Ped MafiaDudes6;
        public Vehicle MafiaCars1;
        public Vehicle MafiaCars2;
        public Vehicle MafiaCars3;
        public Vehicle MafiaCars4;*/
        public static void ConstructMafia1Scene(out Ped mafiaDudes2, out Ped mafiaDudes6, out Ped mafiaDudes3,
            out Ped mafiaDudes1, out Ped mafiaDudes4, out Vehicle mafiaCars1, out Vehicle mafiaCars2,
            out Vehicle mafiaCars4, out Vehicle mafiaCars3, out Ped mafiaDudes5)
        {
            mafiaDudes2 = new Ped(new Model(0xfb6c0b97), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 7,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.5263036f, 0.8502967f),
                Position = new Vector3(918.8804f, 38.91176f, 81.09602f)
            };
            mafiaDudes2.SetVariation(0, 0, 0);
            mafiaDudes2.SetVariation(2, 0, 0);
            mafiaDudes2.SetVariation(3, 0, 0);
            mafiaDudes2.SetVariation(4, 0, 0);
            mafiaDudes2.SetVariation(6, 0, 0);
            mafiaDudes2.SetVariation(7, 0, 0);
            mafiaDudes2.SetVariation(8, 0, 0);
            mafiaDudes2.SetVariation(11, 0, 0);
            mafiaDudes2.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            mafiaDudes2.Tasks.ClearImmediately();
            mafiaDudes2.Heading = 63.5121f;
            mafiaDudes6 = new Ped(new Model(0xa217f345), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 4,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9439237f, 0.3301639f),
                Position = new Vector3(935.259f, -4.957746f, 78.764f)
            };
            mafiaDudes6.SetVariation(0, 2, 0);
            mafiaDudes6.SetVariation(1, 1, 0);
            mafiaDudes6.SetVariation(2, 2, 0);
            mafiaDudes6.SetVariation(3, 1, 0);
            mafiaDudes6.SetVariation(4, 0, 1);
            mafiaDudes6.SetVariation(6, 0, 1);
            mafiaDudes6.SetVariation(7, 1, 0);
            mafiaDudes6.SetVariation(8, 0, 0);
            mafiaDudes6.SetVariation(11, 2, 0);
            mafiaDudes6.Inventory.Weapons.Add(WeaponHash.APPistol).Ammo = -1;
            mafiaDudes6.Tasks.ClearImmediately();
            mafiaDudes6.Heading = 141.4425f;
            mafiaDudes3 = new Ped(new Model(0x1422d45b), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Armor = 100,
                Money = 15,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.8629501f, 0.5052892f),
                Position = new Vector3(919.1486f, 39.85356f, 81.09602f)
            };
            mafiaDudes3.SetVariation(0, 4, 0);
            mafiaDudes3.SetVariation(1, 1, 0);
            mafiaDudes3.SetVariation(2, 4, 1);
            mafiaDudes3.SetVariation(3, 1, 0);
            mafiaDudes3.SetVariation(4, 0, 0);
            mafiaDudes3.SetVariation(6, 1, 0);
            mafiaDudes3.SetVariation(7, 2, 0);
            mafiaDudes3.SetVariation(8, 2, 0);
            mafiaDudes3.SetVariation(10, 2, 0);
            mafiaDudes3.SetVariation(11, 2, 0);
            mafiaDudes3.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
            mafiaDudes3.Tasks.ClearImmediately();
            mafiaDudes3.Heading = 119.2989f;
            mafiaDudes1 = new Ped(new Model(0x1f0846a7), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 8,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(918.3936f, 38.30908f, 81.09602f)
            };
            mafiaDudes1.SetVariation(0, 0, 0);
            mafiaDudes1.SetVariation(1, 0, 0);
            mafiaDudes1.SetVariation(2, 0, 0);
            mafiaDudes1.SetVariation(3, 0, 0);
            mafiaDudes1.SetVariation(4, 0, 0);
            mafiaDudes1.SetVariation(6, 0, 0);
            mafiaDudes1.SetVariation(8, 0, 0);
            mafiaDudes1.SetVariation(10, 0, 0);
            mafiaDudes1.SetVariation(11, 0, 0);
            mafiaDudes1.Inventory.Weapons.Add(WeaponHash.AssaultSMG).Ammo = -1;
            mafiaDudes1.Tasks.ClearImmediately();
            mafiaDudes1.Heading = 0f;
            mafiaDudes4 = new Ped(new Model(0xcff0d4bb), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Armor = 100,
                Money = 9,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(3.967496E-07f, -1.201174E-06f, 0.9495434f, -0.3136357f),
                Position = new Vector3(926.7933f, -3.501604f, 78.76405f)
            };
            mafiaDudes4.SetVariation(0, 0, 0);
            mafiaDudes4.SetVariation(2, 0, 0);
            mafiaDudes4.SetVariation(3, 0, 0);
            mafiaDudes4.SetVariation(4, 0, 0);
            mafiaDudes4.SetVariation(6, 0, 0);
            mafiaDudes4.SetVariation(7, 0, 0);
            mafiaDudes4.SetVariation(8, 0, 0);
            mafiaDudes4.SetVariation(10, 0, 0);
            mafiaDudes4.SetVariation(11, 0, 0);
            mafiaDudes4.Inventory.Weapons.Add(WeaponHash.CarbineRifle).Ammo = -1;
            mafiaDudes4.Tasks.ClearImmediately();
            mafiaDudes4.Heading = 216.5569f;
            mafiaCars1 = new Vehicle("BURRITO3", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 7.426882E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 255, 255, 255),
                PrimaryColor = Color.FromArgb(255, 255, 255, 255),
                FuelLevel = 60f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus) 1,
                DirtLevel = 8f,
                LicensePlate = "89OHP113",
                CollisionIgnoredEntity = null,
                IsGravityDisabled = false,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9585775f, 0.284832f),
                Position = new Vector3(933.5016f, -3.333754f, 78.5926f)
            };
            mafiaCars2 = new Vehicle("BURRITO3", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 1.401298E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 255, 255, 255),
                PrimaryColor = Color.FromArgb(255, 255, 255, 255),
                FuelLevel = 60f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus) 1,
                DirtLevel = 5f,
                LicensePlate = "27NZR335",
                CollisionIgnoredEntity = null,
                IsGravityDisabled = false,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.67198f, 0.7405694f),
                Position = new Vector3(933.3724f, -11.5976f, 78.59264f)
            };
            mafiaCars4 = new Vehicle("WINDSOR2", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 2.802597E-45f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.279f,
                RimColor = Color.FromArgb(255, 8, 8, 8),
                PearlescentColor = Color.FromArgb(255, 105, 0, 0),
                SecondaryColor = Color.FromArgb(255, 255, 255, 255),
                PrimaryColor = Color.FromArgb(255, 255, 255, 255),
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus) 1,
                DirtLevel = 3f,
                LicensePlate = "45PKC424",
                CollisionIgnoredEntity = null,
                IsGravityDisabled = false,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.8904694f, 0.4550431f),
                Position = new Vector3(915.1289f, 42.44905f, 80.3551f)
            };
            mafiaCars3 = new Vehicle("SUPERD", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 5.605194E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.26f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 255, 255, 255),
                PrimaryColor = Color.FromArgb(255, 0, 0, 0),
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus) 1,
                DirtLevel = 0f,
                LicensePlate = "00VLT493",
                CollisionIgnoredEntity = null,
                IsGravityDisabled = false,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9927279f, -0.1203798f),
                Position = new Vector3(921.0477f, -3.685453f, 78.30756f)
            };
            mafiaDudes5 = new Ped(new Model(0xfb6c0b97), Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 6,
                RelationshipGroup = new RelationshipGroup("MAFIA"),
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(-2.078405E-06f, -7.161344E-06f, 0.9603711f, 0.2787243f),
                Position = new Vector3(930.896f, -2.548651f, 78.76403f)
            };
            mafiaDudes5.SetVariation(0, 0, 0);
            mafiaDudes5.SetVariation(2, 0, 0);
            mafiaDudes5.SetVariation(3, 0, 0);
            mafiaDudes5.SetVariation(4, 0, 0);
            mafiaDudes5.SetVariation(6, 0, 0);
            mafiaDudes5.SetVariation(7, 0, 0);
            mafiaDudes5.SetVariation(8, 0, 0);
            mafiaDudes5.SetVariation(11, 0, 0);
            mafiaDudes5.Inventory.Weapons.Add(WeaponHash.Pistol50).Ammo = -1;
            mafiaDudes5.Tasks.ClearImmediately();
            mafiaDudes5.Heading = 147.6318f;
            Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
        }
    }
}