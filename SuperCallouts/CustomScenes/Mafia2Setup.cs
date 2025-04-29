#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes;

internal static class Mafia2Setup
{
    internal static void ConstructMafia2Scene(
        out Vehicle mafiaCars4,
        out Vehicle mafiaCars2,
        out Vehicle mafiaCars1,
        out Vehicle mafiaCars3,
        out Ped mafiaDudes7,
        out Ped mafiaDudes12,
        out Ped mafiaDudes3,
        out Ped mafiaDudes10,
        out Ped mafiaDudes13,
        out Ped mafiaDudes11,
        out Ped mafiaDudes14,
        out Ped mafiaDudes1,
        out Ped mafiaDudes2,
        out Ped mafiaDudes5,
        out Ped mafiaDudes4,
        out Ped mafiaDudes15,
        out Ped mafiaDudes6,
        out Ped mafiaDudes9,
        out Ped mafiaDudes8
    )
    {
        mafiaDudes2 = new Ped("IG_BESTMEN", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 8,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.6129205f, 0.7901446f),
            Position = new Vector3(1543.855f, 3607.511f, 35.2162f),
        };
        mafiaDudes2.SetVariation(0, 1, 0);
        mafiaDudes2.SetVariation(3, 0, 0);
        mafiaDudes2.SetVariation(4, 0, 0);
        mafiaDudes2.SetVariation(6, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes2.Tasks.ClearImmediately();
        mafiaDudes2.Heading = 75.60194f;

        mafiaDudes5 = new Ped("IG_BESTMEN", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.4196779f, 0.9076731f),
            Position = new Vector3(1543.173f, 3606.55f, 35.19303f),
        };
        mafiaDudes5.SetVariation(0, 0, 0);
        mafiaDudes5.SetVariation(3, 0, 0);
        mafiaDudes5.SetVariation(4, 0, 0);
        mafiaDudes5.SetVariation(6, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes5.Tasks.ClearImmediately();
        mafiaDudes5.Heading = 49.62851f;

        mafiaDudes4 = new Ped("IG_BANKMAN", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 5,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9087628f, -0.4173131f),
            Position = new Vector3(1542.263f, 3609.036f, 35.26722f),
        };
        mafiaDudes4.SetVariation(0, 0, 0);
        mafiaDudes4.SetVariation(2, 0, 0);
        mafiaDudes4.SetVariation(3, 0, 0);
        mafiaDudes4.SetVariation(4, 0, 0);
        mafiaDudes4.SetVariation(5, 0, 0);
        mafiaDudes4.SetVariation(6, 0, 0);
        mafiaDudes4.SetVariation(7, 0, 0);
        mafiaDudes4.SetVariation(8, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes4.Tasks.ClearImmediately();
        mafiaDudes4.Heading = 229.3301f;

        mafiaDudes15 = new Ped("CS_PAPER", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 7,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.7833742f, -0.6215504f),
            Position = new Vector3(1541.049f, 3607.429f, 35.24071f),
        };
        mafiaDudes15.SetVariation(0, 0, 0);
        mafiaDudes15.SetVariation(3, 0, 0);
        mafiaDudes15.SetVariation(4, 0, 0);
        mafiaDudes15.SetVariation(5, 0, 0);
        mafiaDudes15.SetVariation(6, 0, 0);
        mafiaDudes15.SetVariation(7, 0, 0);
        mafiaDudes15.SetVariation(8, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes15.Tasks.ClearImmediately();
        mafiaDudes15.Heading = 256.8589f;

        mafiaDudes6 = new Ped("IG_MILTON", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 8,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.2748725f, 0.9614807f),
            Position = new Vector3(1546.936f, 3602.605f, 38.73494f),
        };
        mafiaDudes6.SetVariation(0, 0, 0);
        mafiaDudes6.SetVariation(2, 0, 0);
        mafiaDudes6.SetVariation(3, 0, 0);
        mafiaDudes6.SetVariation(4, 0, 0);
        mafiaDudes6.SetVariation(5, 0, 0);
        mafiaDudes6.SetVariation(6, 1, 0);
        mafiaDudes6.SetVariation(7, 0, 0);
        mafiaDudes6.SetVariation(11, 1, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes6.Tasks.ClearImmediately();
        mafiaDudes6.Heading = 31.90884f;

        mafiaDudes9 = new Ped("CS_MOVPREMMALE", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 23,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1544.582f, 3601.32f, 38.73379f),
        };
        mafiaDudes9.SetVariation(0, 3, 2);
        mafiaDudes9.SetVariation(3, 1, 0);
        mafiaDudes9.SetVariation(4, 1, 0);
        mafiaDudes9.SetVariation(6, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes9.Tasks.ClearImmediately();
        mafiaDudes9.Heading = 0f;

        mafiaDudes8 = new Ped("CS_MOVPREMMALE", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.5450023f, 0.8384345f),
            Position = new Vector3(1551.669f, 3605.39f, 38.74341f),
        };
        mafiaDudes8.SetVariation(0, 1, 0);
        mafiaDudes8.SetVariation(3, 2, 0);
        mafiaDudes8.SetVariation(4, 0, 0);
        mafiaDudes8.SetVariation(6, 2, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes8.Tasks.ClearImmediately();
        mafiaDudes8.Heading = 66.04964f;

        mafiaCars4 = new Vehicle(new Model(0x897afc65), Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 9.809089E-45f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.16f,
            NumberOfGears = 6,
            RimColor = Color.FromArgb(255, 71, 57, 27),
            PearlescentColor = Color.FromArgb(255, 71, 57, 27),
            SecondaryColor = Color.FromArgb(255, 1, 0, 0),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 11f,
            LicensePlate = "85QON527",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.4669424f, 0.8842877f),
            Position = new Vector3(1531.97f, 3602.66f, 35.39088f),
        };

        mafiaCars2 = new Vehicle(new Model(0x1324e960), Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 4.484155E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.2f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 71, 57, 27),
            PearlescentColor = Color.FromArgb(255, 51, 51, 51),
            SecondaryColor = Color.FromArgb(255, 0, 0, 0),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 8f,
            LicensePlate = "41ZPP757",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.7309447f, 0.6824367f),
            Position = new Vector3(1560.377f, 3615.933f, 34.72619f),
        };

        mafiaCars1 = new Vehicle("LIMO2", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 2.942727E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.27f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 33, 8, 4),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 7f,
            LicensePlate = "43WPW436",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(1566.573f, 3621.845f, 34.69645f),
        };

        mafiaCars3 = new Vehicle("LIMO2", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 4.063766E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.27f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 33, 8, 4),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 4f,
            LicensePlate = "61UOM514",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.847095f, -0.5314415f),
            Position = new Vector3(1541.743f, 3615.468f, 34.87094f),
        };

        mafiaDudes7 = new Ped(new Model(0xfb6c0b97), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.213613f, 0.9769183f),
            Position = new Vector3(1557.716f, 3614.394f, 35.17625f),
        };
        mafiaDudes7.SetVariation(0, 0, 0);
        mafiaDudes7.SetVariation(2, 0, 0);
        mafiaDudes7.SetVariation(3, 0, 0);
        mafiaDudes7.SetVariation(4, 0, 0);
        mafiaDudes7.SetVariation(6, 0, 0);
        mafiaDudes7.SetVariation(7, 0, 0);
        mafiaDudes7.SetVariation(8, 0, 0);
        mafiaDudes7.SetVariation(11, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes7.Tasks.ClearImmediately();
        mafiaDudes7.Heading = 335.3317f;

        mafiaDudes12 = new Ped(new Model(0x1422d45b), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 22,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.07992769f, 0.9968007f),
            Position = new Vector3(1564.479f, 3619.595f, 35.15536f),
        };
        mafiaDudes12.SetVariation(0, 2, 2);
        mafiaDudes12.SetVariation(1, 1, 0);
        mafiaDudes12.SetVariation(2, 2, 0);
        mafiaDudes12.SetVariation(3, 1, 3);
        mafiaDudes12.SetVariation(4, 1, 0);
        mafiaDudes12.SetVariation(6, 0, 0);
        mafiaDudes12.SetVariation(7, 1, 0);
        mafiaDudes12.SetVariation(8, 2, 0);
        mafiaDudes12.SetVariation(10, 2, 0);
        mafiaDudes12.SetVariation(11, 2, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes12.Tasks.ClearImmediately();
        mafiaDudes12.Heading = 350.8312f;

        mafiaDudes3 = new Ped(new Model(0xa217f345), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 2,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.1719038f, 0.9851137f),
            Position = new Vector3(1539.855f, 3612.797f, 35.35184f),
        };
        mafiaDudes3.SetVariation(0, 0, 0);
        mafiaDudes3.SetVariation(1, 0, 0);
        mafiaDudes3.SetVariation(2, 2, 0);
        mafiaDudes3.SetVariation(3, 1, 0);
        mafiaDudes3.SetVariation(4, 0, 1);
        mafiaDudes3.SetVariation(6, 0, 0);
        mafiaDudes3.SetVariation(7, 1, 0);
        mafiaDudes3.SetVariation(8, 0, 0);
        mafiaDudes3.SetVariation(11, 2, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes3.Tasks.ClearImmediately();
        mafiaDudes3.Heading = 19.79705f;

        mafiaDudes10 = new Ped(new Model(0x1422d45b), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 11,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.02693291f, 0.9996372f),
            Position = new Vector3(1534.879f, 3609.748f, 35.35373f),
        };
        mafiaDudes10.SetVariation(0, 5, 2);
        mafiaDudes10.SetVariation(1, 0, 0);
        mafiaDudes10.SetVariation(2, 5, 0);
        mafiaDudes10.SetVariation(3, 0, 4);
        mafiaDudes10.SetVariation(4, 0, 0);
        mafiaDudes10.SetVariation(6, 1, 0);
        mafiaDudes10.SetVariation(7, 0, 0);
        mafiaDudes10.SetVariation(8, 1, 0);
        mafiaDudes10.SetVariation(10, 1, 0);
        mafiaDudes10.SetVariation(11, 1, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes10.Tasks.ClearImmediately();
        mafiaDudes10.Heading = 3.086657f;

        mafiaDudes13 = new Ped(new Model(0x2be886b2), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.3471982f, 0.9377918f),
            Position = new Vector3(1544.377f, 3609.884f, 35.26527f),
        };
        mafiaDudes13.SetVariation(0, 0, 0);
        mafiaDudes13.SetVariation(1, 0, 0);
        mafiaDudes13.SetVariation(2, 0, 0);
        mafiaDudes13.SetVariation(3, 0, 0);
        mafiaDudes13.SetVariation(4, 0, 0);
        mafiaDudes13.SetVariation(6, 0, 0);
        mafiaDudes13.SetVariation(8, 0, 0);
        mafiaDudes13.SetVariation(10, 0, 0);
        mafiaDudes13.SetVariation(11, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes13.Tasks.ClearImmediately();
        mafiaDudes13.Heading = 319.3679f;

        mafiaDudes11 = new Ped(new Model(0x035456a4), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 18,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.1373514f, 0.9905224f),
            Position = new Vector3(1541.052f, 3601.899f, 35.27475f),
        };
        mafiaDudes11.SetVariation(0, 0, 0);
        mafiaDudes11.SetVariation(2, 0, 0);
        mafiaDudes11.SetVariation(3, 0, 0);
        mafiaDudes11.SetVariation(4, 0, 0);
        mafiaDudes11.SetVariation(6, 0, 0);
        mafiaDudes11.SetVariation(7, 0, 0);
        mafiaDudes11.SetVariation(8, 0, 0);
        mafiaDudes11.SetVariation(10, 0, 0);
        mafiaDudes11.SetVariation(11, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes11.Tasks.ClearImmediately();
        mafiaDudes11.Heading = 15.78923f;

        mafiaDudes14 = new Ped(new Model(0x93fecbd7), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.2392313f, 0.9709626f),
            Position = new Vector3(1546.732f, 3604.885f, 35.43145f),
        };
        mafiaDudes14.SetVariation(0, 0, 0);
        mafiaDudes14.SetVariation(2, 0, 0);
        mafiaDudes14.SetVariation(3, 0, 0);
        mafiaDudes14.SetVariation(4, 0, 0);
        mafiaDudes14.SetVariation(6, 0, 0);
        mafiaDudes14.SetVariation(8, 0, 0);
        mafiaDudes14.SetVariation(11, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes14.Tasks.ClearImmediately();
        mafiaDudes14.Heading = 27.68236f;

        mafiaDudes1 = new Ped(new Model(0xe433179f), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 15,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.2514437f, 0.9678719f),
            Position = new Vector3(1541.702f, 3606.088f, 35.20017f),
        };
        mafiaDudes1.SetVariation(0, 0, 0);
        mafiaDudes1.SetVariation(2, 0, 0);
        mafiaDudes1.SetVariation(4, 0, 0);
        mafiaDudes1.SetVariation(5, 0, 0);
        mafiaDudes1.SetVariation(6, 0, 0);
        mafiaDudes1.SetVariation(7, 0, 0);
        mafiaDudes1.SetVariation(8, 0, 0);
        mafiaDudes1.SetVariation(9, 0, 0);
        mafiaDudes1.SetVariation(11, 0, 0);
        // Newly spawned Peds will try to face north.
        mafiaDudes1.Tasks.ClearImmediately();
        mafiaDudes1.Heading = 330.8741f;
    }
}
