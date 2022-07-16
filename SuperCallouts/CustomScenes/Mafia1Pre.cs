#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes
{
    internal static class Mafia1Pre
    {
        internal static void BuildPreScene(out Ped fibarchitect, out Ped mpFibsec, out Ped swat, out Ped swat2,
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
}