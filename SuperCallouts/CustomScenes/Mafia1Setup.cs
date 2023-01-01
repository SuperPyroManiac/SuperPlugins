#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes;

internal static class Mafia1Setup
{
    internal static void BuildScene(out Ped xdb, out Ped xbeb, out Ped xa, out Ped xcdcb, out Ped xbaa2,
        out Ped xcdcb2, out Ped xbaa, out Ped xbaa3, out Vehicle limo, out Vehicle boxville, out Vehicle btype,
        out Vehicle benson)
    {
        xdb = new Ped(new Model(0x1422d45b), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 2,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(-0.0005306705f, -0.0007321094f, 0.8096673f, 0.5868884f),
            Position = new Vector3(891.6799f, 20.89992f, 78.89739f)
        };
        xdb.SetVariation(0, 1, 1);
        xdb.SetVariation(1, 0, 0);
        xdb.SetVariation(2, 1, 1);
        xdb.SetVariation(3, 0, 5);
        xdb.SetVariation(4, 0, 0);
        xdb.SetVariation(6, 1, 0);
        xdb.SetVariation(7, 0, 0);
        xdb.SetVariation(8, 1, 0);
        xdb.SetVariation(10, 1, 0);
        xdb.SetVariation(11, 1, 0);
        xdb.Tasks.ClearImmediately();
        xdb.Heading = 108.1269f;

        xbeb = new Ped(new Model(0x2be886b2), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 17,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.3590865f, 0.9333043f),
            Position = new Vector3(884.157f, 9.278169f, 78.90435f)
        };
        xbeb.SetVariation(0, 0, 0);
        xbeb.SetVariation(1, 0, 0);
        xbeb.SetVariation(2, 0, 0);
        xbeb.SetVariation(3, 0, 0);
        xbeb.SetVariation(4, 0, 0);
        xbeb.SetVariation(6, 0, 0);
        xbeb.SetVariation(8, 0, 0);
        xbeb.SetVariation(10, 0, 0);
        xbeb.SetVariation(11, 0, 0);
        xbeb.Tasks.ClearImmediately();
        xbeb.Heading = 42.0882f;

        limo = new Vehicle("LIMO2", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 6.586103E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.27f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 33, 8, 4),
            SecondaryColor = Color.FromArgb(255, 78, 49, 255),
            PrimaryColor = Color.FromArgb(255, 162, 177, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 0f,
            LicensePlate = "21MFC584",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.8763167f, -0.4817355f),
            Position = new Vector3(898.3679f, 18.40567f, 78.39939f)
        };

        boxville = new Vehicle("BOXVILLE3", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 6.586103E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.11f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 181, 255, 255),
            FuelLevel = 75f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 4f,
            LicensePlate = "24NFG392",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.4547044f, 0.8906424f),
            Position = new Vector3(909.564f, 4.041251f, 78.67892f)
        };

        btype = new Vehicle("BTYPE3", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 7.707142E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.27f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 59, 70, 84),
            SecondaryColor = Color.FromArgb(255, 111, 147, 255),
            PrimaryColor = Color.FromArgb(255, 181, 212, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            LicensePlate = "43MPN584",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(903.9366f, 4.442619f, 78.22449f)
        };

        xa = new Ped(new Model(0x035456a4), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 13,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9996499f, -0.02645799f),
            Position = new Vector3(903.2559f, 7.793941f, 78.77438f)
        };
        xa.SetVariation(0, 0, 0);
        xa.SetVariation(2, 0, 0);
        xa.SetVariation(3, 0, 0);
        xa.SetVariation(4, 0, 0);
        xa.SetVariation(6, 0, 0);
        xa.SetVariation(7, 0, 0);
        xa.SetVariation(8, 0, 0);
        xa.SetVariation(10, 0, 0);
        xa.SetVariation(11, 0, 0);
        xa.Tasks.ClearImmediately();
        xa.Heading = 183.0322f;

        xcdcb = new Ped(new Model(0x3cd28cb3), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 5,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.2186501f, 0.9758033f),
            Position = new Vector3(910.5573f, 32.27618f, 80.27606f)
        };
        xcdcb.SetVariation(0, 1, 0);
        xcdcb.SetVariation(2, 1, 0);
        xcdcb.SetVariation(3, 0, 0);
        xcdcb.SetVariation(4, 0, 5);
        xcdcb.SetVariation(6, 0, 0);
        xcdcb.SetVariation(7, 0, 5);
        xcdcb.SetVariation(8, 0, 3);
        xcdcb.SetVariation(11, 0, 5);
        xcdcb.Tasks.ClearImmediately();
        xcdcb.Heading = 334.7405f;

        xbaa2 = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 14,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.2696833f, 0.962949f),
            Position = new Vector3(911.2944f, 43.06391f, 80.89889f)
        };
        xbaa2.SetVariation(0, 2, 0);
        xbaa2.SetVariation(2, 2, 0);
        xbaa2.SetVariation(3, 2, 0);
        xbaa2.SetVariation(4, 0, 0);
        xbaa2.SetVariation(6, 2, 0);
        xbaa2.Tasks.ClearImmediately();
        xbaa2.Heading = 328.7092f;

        benson = new Vehicle("BENSON", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 6.165713E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.16f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 181, 255, 255),
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 8f,
            LicensePlate = "22CRW231",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.295084f, 0.9554713f),
            Position = new Vector3(916.3431f, 47.8009f, 80.89904f)
        };

        xcdcb2 = new Ped(new Model(0x3cd28cb3), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 20,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9779977f, 0.208616f),
            Position = new Vector3(911.3863f, 33.33047f, 80.37089f)
        };
        xcdcb2.SetVariation(0, 0, 2);
        xcdcb2.SetVariation(2, 0, 2);
        xcdcb2.SetVariation(3, 0, 0);
        xcdcb2.SetVariation(4, 1, 4);
        xcdcb2.SetVariation(6, 0, 0);
        xcdcb2.SetVariation(7, 1, 0);
        xcdcb2.SetVariation(8, 2, 4);
        xcdcb2.SetVariation(11, 0, 4);
        xcdcb2.Tasks.ClearImmediately();
        xcdcb2.Heading = 155.9175f;

        xbaa = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 3,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9649342f, 0.2624918f),
            Position = new Vector3(903.2636f, 27.42938f, 79.66326f)
        };
        xbaa.SetVariation(0, 4, 0);
        xbaa.SetVariation(2, 2, 0);
        xbaa.SetVariation(3, 1, 0);
        xbaa.SetVariation(4, 1, 0);
        xbaa.SetVariation(6, 1, 0);
        xbaa.Tasks.ClearImmediately();
        xbaa.Heading = 149.5641f;

        xbaa3 = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 14,
            RelationshipGroup = new RelationshipGroup("MAFIA"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.598518f, 0.8011094f),
            Position = new Vector3(900.7629f, 3.734434f, 78.77782f)
        };
        xbaa3.SetVariation(0, 2, 1);
        xbaa3.SetVariation(2, 2, 0);
        xbaa3.SetVariation(3, 0, 0);
        xbaa3.SetVariation(4, 0, 0);
        xbaa3.SetVariation(6, 0, 0);
        xbaa3.Tasks.ClearImmediately();
        xbaa3.Heading = 286.4724f;
    }

    internal static void BuildMafia1PreScene(out Ped fibarchitect, out Ped mpFibsec, out Ped swat, out Ped swat2,
        out Ped fiboffice, out Vehicle fbi, out Vehicle riot)
    {
        fibarchitect = new Ped("U_M_M_FIBARCHITECT", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 6,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9721142f, 0.2345079f),
            Position = new Vector3(-342.0603f, -962.7352f, 31.08061f)
        };
        fibarchitect.SetVariation(0, 0, 0);
        fibarchitect.SetVariation(3, 0, 0);
        fibarchitect.SetVariation(4, 0, 0);
        fibarchitect.SetVariation(8, 0, 0);
        fibarchitect.Tasks.ClearImmediately();
        fibarchitect.Heading = 152.8748f;

        mpFibsec = new Ped("MP_M_FIBSEC_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            Money = 3,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.8452248f, 0.5344109f),
            Position = new Vector3(-339.8573f, -963.5591f, 31.08061f)
        };
        mpFibsec.SetVariation(0, 1, 0);
        mpFibsec.SetVariation(2, 0, 0);
        mpFibsec.SetVariation(3, 0, 1);
        mpFibsec.SetVariation(4, 0, 0);
        mpFibsec.SetVariation(10, 0, 0);
        mpFibsec.Tasks.ClearImmediately();
        mpFibsec.Heading = 115.3921f;

        fbi = new Vehicle("FBI", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 6.586103E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 40f,
            SteeringScale = 1.222248E-16f,
            DriveForce = 0.28f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 15, 15, 15),
            PrimaryColor = Color.FromArgb(255, 15, 15, 15),
            IsDeformationEnabled = false,
            CanTiresBurst = false,
            FuelTankHealth = 2000f,
            EngineHealth = 2000f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 0.04603858f,
            LicensePlate = "89LTV217",
            IsSirenSilent = true,
            IsMeleeProof = true,
            IsCollisionProof = true,
            IsExplosionProof = true,
            IsBulletProof = true,
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(-0.005689827f, -0.00384988f, 0.5616957f, 0.8273154f),
            Position = new Vector3(-340.1805f, -961.6083f, 30.57838f)
        };

        riot = new Vehicle("RIOT", Vector3.Zero, 0f)
        {
            VerticalFlightPhase = 0f,
            DesiredVerticalFlightPhase = 7.006492E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 40f,
            SteeringScale = -0.0002079709f,
            DriveForce = 0.12f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 8, 8, 8),
            PrimaryColor = Color.FromArgb(255, 240, 240, 240),
            IsDeformationEnabled = false,
            CanTiresBurst = false,
            FuelTankHealth = 2000f,
            EngineHealth = 2000f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 0.004502276f,
            LicensePlate = "83NSZ428",
            IsMeleeProof = true,
            IsCollisionProof = true,
            IsExplosionProof = true,
            IsBulletProof = true,
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(-2.960484E-05f, 0.0006024266f, -0.1478644f, 0.9890075f),
            Position = new Vector3(-342.9797f, -972.0889f, 30.73344f)
        };

        swat = new Ped("S_M_Y_SWAT_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0x98787966u),
            Armor = 100,
            Money = 0,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.1203384f, 0.9927329f),
            Position = new Vector3(-347.8616f, -968.1098f, 31.08061f)
        };
        swat.SetVariation(0, 0, 0);
        swat.SetVariation(3, 0, 0);
        swat.SetVariation(4, 0, 0);
        swat.SetVariation(10, 0, 0);
        swat.Tasks.ClearImmediately();
        swat.Heading = 346.1767f;

        swat2 = new Ped("S_M_Y_SWAT_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0x98787966u),
            Armor = 100,
            Money = 5,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.2061085f, 0.9785292f),
            Position = new Vector3(-347.0247f, -967.9576f, 31.08061f)
        };
        swat2.SetVariation(0, 0, 1);
        swat2.SetVariation(3, 0, 0);
        swat2.SetVariation(4, 0, 0);
        swat2.SetVariation(10, 0, 0);
        swat2.Tasks.ClearImmediately();
        swat2.Heading = 23.7888f;

        fiboffice = new Ped("S_M_M_FIBOFFICE_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 20,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9892623f, 0.1461505f),
            Position = new Vector3(-347.8155f, -966.1456f, 31.08061f)
        };
        fiboffice.SetVariation(0, 0, 0);
        fiboffice.SetVariation(3, 0, 0);
        fiboffice.SetVariation(4, 0, 0);
        fiboffice.Tasks.ClearImmediately();
        fiboffice.Heading = 163.1922f;
    }
}