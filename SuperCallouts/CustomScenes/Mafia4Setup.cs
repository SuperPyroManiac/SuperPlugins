#region

using System;
using System.Drawing;
using Rage;
using Object = Rage.Object;

#endregion

namespace SuperCallouts.CustomScenes;

public class Mafia4Setup
{
//ConstructMafia4Scene(out fiboffice, out fiboffice2, out fiboffice3, out fiboffice4, out doctor, out doctor2, out fiboffice5, out fiboffice6, out fiboffice7, out benson, out burrito, out burrito2, out burrito3, out propMbCargoa);

    internal static void ConstructMafia4Scene(out Ped fiboffice, out Ped fiboffice2, out Ped fiboffice3,
        out Ped fiboffice4, out Ped doctor, out Ped doctor2, out Ped fiboffice5, out Ped fiboffice6, out Ped fiboffice7,
        out Vehicle benson, out Vehicle burrito, out Vehicle burrito2, out Vehicle burrito3, out Object propMbCargoa)
    {
        fiboffice = new Ped("S_M_M_FIBOFFICE_01", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 17,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(294.6198f, -1581.93f, 30.53215f)
        };
        fiboffice.SetVariation(0, 0, 1);
        fiboffice.SetVariation(3, 0, 1);
        fiboffice.SetVariation(4, 0, 1);
        fiboffice.Tasks.ClearImmediately();
        fiboffice.Heading = 0f;

        fiboffice2 = new Ped("S_M_M_FIBOFFICE_01", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 24,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(278.633f, -1583.595f, 30.53215f)
        };
        fiboffice2.SetVariation(0, 2, 0);
        fiboffice2.SetVariation(3, 0, 2);
        fiboffice2.SetVariation(4, 0, 2);
        fiboffice2.Tasks.ClearImmediately();
        fiboffice2.Heading = 0f;

        fiboffice3 = new Ped("S_M_M_FIBOFFICE_01", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 20,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(284.0651f, -1598.953f, 31.16202f)
        };
        fiboffice3.SetVariation(0, 1, 0);
        fiboffice3.SetVariation(3, 0, 0);
        fiboffice3.SetVariation(4, 0, 0);
        fiboffice3.Tasks.ClearImmediately();
        fiboffice3.Heading = 0f;

        fiboffice4 = new Ped("S_M_M_FIBOFFICE_02", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 22,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0.05535255f, -0.03823947f, -1.661589E-07f),
                Orientation = new Quaternion(0f, 0f, 0.999487f, -0.03202566f),
                Position = new Vector3(289.0267f, -1585.247f, 30.53215f)
        };
        fiboffice4.SetVariation(0, 2, 0);
        fiboffice4.SetVariation(3, 0, 2);
        fiboffice4.SetVariation(4, 0, 2);
        fiboffice4.Tasks.ClearImmediately();
        fiboffice4.Heading = 183.6705f;

        doctor = new Ped("S_M_M_DOCTOR_01", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 23,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.8316335f, -0.5553248f),
                Position = new Vector3(286.906f, -1586.453f, 30.53215f)
        };
        doctor.SetVariation(0, 1, 1);
        doctor.SetVariation(3, 0, 0);
        doctor.SetVariation(4, 1, 1);
        doctor.SetVariation(8, 2, 0);
        doctor.SetVariation(11, 1, 2);
        doctor.Tasks.ClearImmediately();
        doctor.Heading = 247.4662f;

        doctor2 = new Ped("S_M_M_DOCTOR_01", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 13,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9339725f, 0.3573447f),
                Position = new Vector3(290.7178f, -1586.556f, 30.53362f)
        };
        doctor2.SetVariation(0, 2, 1);
        doctor2.SetVariation(3, 0, 0);
        doctor2.SetVariation(4, 1, 2);
        doctor2.SetVariation(8, 1, 0);
        doctor2.SetVariation(11, 1, 1);
        doctor2.Tasks.ClearImmediately();
        doctor2.Heading = 138.1255f;

        fiboffice5 = new Ped("S_M_M_FIBOFFICE_02", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 9,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.8592089f, 0.511625f),
                Position = new Vector3(273.0192f, -1592.491f, 30.53215f)
        };
        fiboffice5.SetVariation(0, 0, 1);
        fiboffice5.SetVariation(3, 0, 1);
        fiboffice5.SetVariation(4, 0, 1);
        fiboffice5.Tasks.ClearImmediately();
        fiboffice5.Heading = 118.4557f;

        fiboffice6 = new Ped("S_M_M_FIBOFFICE_02", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9890732f, 0.147425f),
                Position = new Vector3(278.2243f, -1607.907f, 30.53215f)
        };
        fiboffice6.SetVariation(0, 2, 1);
        fiboffice6.SetVariation(3, 0, 0);
        fiboffice6.SetVariation(4, 0, 0);
        fiboffice6.Tasks.ClearImmediately();
        fiboffice6.Heading = 163.0445f;

        fiboffice7 = new Ped("S_M_M_FIBOFFICE_02", Vector3.Zero, 0f)
        {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 1,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                Accuracy = 59,
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9195856f, -0.3928897f),
                Position = new Vector3(299.6021f, -1597.858f, 30.53215f)
        };
        fiboffice7.SetVariation(0, 1, 1);
        fiboffice7.SetVariation(3, 0, 0);
        fiboffice7.SetVariation(4, 0, 0);
        fiboffice7.Tasks.ClearImmediately();
        fiboffice7.Heading = 226.2689f;

        benson = new Vehicle("BENSON", Vector3.Zero, 0f)
        {
                DesiredVerticalFlightPhase = 4.203895E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(94, 36, 30, 26),
                SecondaryColor = Color.FromArgb(255, 15, 15, 15),
                PrimaryColor = Color.FromArgb(255, 240, 240, 240),
                FuelLevel = 80f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 14f,
                LicensePlate = "80VSS783",
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0.0002547515f, 2.06061E-08f, 4.154851E-10f, 0.9999999f),
                Position = new Vector3(283.5089f, -1577.795f, 30.50724f)
        };

        burrito = new Vehicle("BURRITO3", Vector3.Zero, 0f)
        {
                DesiredVerticalFlightPhase = 4.203895E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(94, 18, 17, 16),
                SecondaryColor = Color.FromArgb(255, 18, 17, 16),
                PrimaryColor = Color.FromArgb(255, 18, 17, 16),
                FuelLevel = 60f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 5f,
                LicensePlate = "09VUH533",
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(-0.0001423991f, 7.438333E-10f, -4.877182E-09f, 1f),
                Position = new Vector3(295.8005f, -1588.532f, 30.34846f)
        };

        burrito2 = new Vehicle("BURRITO3", Vector3.Zero, 0f)
        {
                DesiredVerticalFlightPhase = 4.203895E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(94, 15, 15, 15),
                SecondaryColor = Color.FromArgb(255, 15, 15, 15),
                PrimaryColor = Color.FromArgb(255, 8, 8, 8),
                FuelLevel = 60f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 6f,
                LicensePlate = "82LJQ362",
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(-6.228617E-05f, -0.0001305744f, 0.9147639f, 0.403989f),
                Position = new Vector3(274.1656f, -1598.911f, 30.34846f)
        };

        burrito3 = new Vehicle("BURRITO3", Vector3.Zero, 0f)
        {
                DesiredVerticalFlightPhase = 3.783506E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.16f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(94, 240, 240, 240),
                SecondaryColor = Color.FromArgb(255, 240, 240, 240),
                PrimaryColor = Color.FromArgb(255, 240, 240, 240),
                FuelLevel = 60f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                LicensePlate = "61KIP155",
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(-6.215323E-05f, -0.000130518f, 0.9147639f, 0.403989f),
                Position = new Vector3(299.064f, -1613.023f, 30.34846f)
        };

        propMbCargoa = new Object("PROP_MB_CARGO_02A", Vector3.Zero, 0f)
        {
                CollisionIgnoredEntity = null,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(288.916f, -1588.429f, 29.53253f)
        };
    }
}