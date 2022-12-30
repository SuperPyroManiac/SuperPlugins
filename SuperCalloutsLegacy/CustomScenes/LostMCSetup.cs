#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCalloutsLegacy.CustomScenes;

internal static class LostMc
{
    internal static void ConstructBikersScene(out Vehicle sheriff, out Vehicle sheriff2, out Ped sheriff4,
        out Ped sheriff5, out Ped sheriff6, out Vehicle ratbike, out Vehicle ratbike2, out Vehicle deathbike,
        out Vehicle deathbike2, out Vehicle deathbike3, out Vehicle ratbike3, out Vehicle daemon, out Ped lostB7,
        out Ped lostB3, out Ped lostB8, out Ped lostB4, out Ped lostB6, out Ped lostB9, out Ped lostB1,
        out Ped lostB10, out Ped lostB5, out Ped lostB2)
    {
        sheriff = new Vehicle("SHERIFF", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 8.127531E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.24f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)5,
            DirtLevel = 9f,
            LicensePlate = "88MGA666",
            IsSirenSilent = true,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.640557f, 0.7679107f),
            Position = new Vector3(2350.661f, 4920.378f, 41.7339f)
        };

        sheriff2 = new Vehicle("SHERIFF", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.24f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            FuelTankHealth = 974.917f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)5,
            DirtLevel = 0f,
            LicensePlate = "41VRH671",
            IsSirenSilent = true,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.2392817f, 0.9709502f),
            Position = new Vector3(2353.223f, 4923.135f, 41.9646f)
        };

        sheriff4 = new Ped("S_F_Y_SHERIFF_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            RelationshipGroup = new RelationshipGroup("COP"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.963404f, -0.2680537f),
            Position = new Vector3(2349.931f, 4926.722f, 42.23621f)
        };
        sheriff4.SetVariation(0, 1, 0);
        sheriff4.SetVariation(2, 0, 2);
        sheriff4.SetVariation(3, 1, 0);
        sheriff4.SetVariation(4, 0, 0);
        sheriff4.SetVariation(5, 1, 0);
        sheriff4.SetVariation(9, 1, 0);
        sheriff4.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        sheriff4.Tasks.ClearImmediately();
        sheriff4.Heading = 211.097f;

        sheriff5 = new Ped("S_M_Y_SHERIFF_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            Money = 17,
            RelationshipGroup = new RelationshipGroup("COP"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.8941039f, -0.4478595f),
            Position = new Vector3(2351.418f, 4923.281f, 42.02858f)
        };
        sheriff5.SetVariation(0, 1, 1);
        sheriff5.SetVariation(3, 1, 0);
        sheriff5.SetVariation(4, 0, 0);
        sheriff5.SetVariation(5, 1, 0);
        sheriff5.SetVariation(9, 1, 0);
        sheriff5.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        sheriff5.Tasks.ClearImmediately();
        sheriff5.Heading = 233.2129f;

        sheriff6 = new Ped("S_F_Y_SHERIFF_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            Money = 7,
            RelationshipGroup = new RelationshipGroup("COP"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9476103f, -0.3194287f),
            Position = new Vector3(2347.824f, 4922.299f, 42.04942f)
        };
        sheriff6.SetVariation(0, 0, 0);
        sheriff6.SetVariation(2, 1, 0);
        sheriff6.SetVariation(3, 0, 0);
        sheriff6.SetVariation(4, 0, 0);
        sheriff6.SetVariation(5, 0, 0);
        sheriff6.SetVariation(9, 0, 0);
        sheriff6.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        sheriff6.Tasks.ClearImmediately();
        sheriff6.Heading = 217.2568f;

        ratbike = new Vehicle("RATBIKE", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.215f,
            RimColor = Color.FromArgb(255, 8, 8, 8),
            PearlescentColor = Color.FromArgb(255, 240, 240, 240),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 174, 0, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 3f,
            LicensePlate = "41AFQ337",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.8005757f, 0.5992316f),
            Position = new Vector3(2366.054f, 4924.566f, 41.77275f)
        };

        ratbike2 = new Vehicle("RATBIKE", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 6.858249E-40f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.215f,
            RimColor = Color.FromArgb(255, 8, 8, 8),
            PearlescentColor = Color.FromArgb(255, 240, 240, 240),
            SecondaryColor = Color.FromArgb(255, 132, 150, 255),
            PrimaryColor = Color.FromArgb(255, 95, 219, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            LicensePlate = "80PCV348",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2357.717f, 4910.729f, 41.58598f)
        };

        deathbike = new Vehicle("DEATHBIKE", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.3125f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 8, 8, 8),
            PearlescentColor = Color.FromArgb(255, 28, 30, 33),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 95, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 0f,
            LicensePlate = "87ZBJ416",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2364.864f, 4915.367f, 41.50339f)
        };

        deathbike2 = new Vehicle("DEATHBIKE", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.3125f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 8, 8, 8),
            PearlescentColor = Color.FromArgb(255, 28, 30, 33),
            SecondaryColor = Color.FromArgb(255, 200, 118, 255),
            PrimaryColor = Color.FromArgb(255, 19, 50, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 3f,
            LicensePlate = "87YZF883",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.7932395f, 0.6089098f),
            Position = new Vector3(2375.015f, 4916.935f, 41.59571f)
        };

        deathbike3 = new Vehicle("DEATHBIKE3", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.3125f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 194, 102, 16),
            PearlescentColor = Color.FromArgb(255, 28, 30, 33),
            SecondaryColor = Color.FromArgb(255, 103, 85, 255),
            PrimaryColor = Color.FromArgb(255, 206, 42, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 9f,
            LicensePlate = "00AQC624",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9507156f, 0.3100643f),
            Position = new Vector3(2373.968f, 4907.563f, 41.83934f)
        };

        ratbike3 = new Vehicle("RATBIKE", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 6.726233E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 73.666f,
            DriveForce = 0.215f,
            RimColor = Color.FromArgb(255, 8, 8, 8),
            PearlescentColor = Color.FromArgb(255, 240, 240, 240),
            SecondaryColor = Color.FromArgb(255, 0, 255, 255),
            PrimaryColor = Color.FromArgb(255, 206, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            LicensePlate = "60HNN672",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.6663206f, 0.7456654f),
            Position = new Vector3(2358.166f, 4915.24f, 41.52239f)
        };

        daemon = new Vehicle("DAEMON", Vector3.Zero, 0f)
        {
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringAngle = 28.64789f,
            DriveForce = 0.26f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 11, 32, 255),
            PrimaryColor = Color.FromArgb(255, 203, 5, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 6f,
            LicensePlate = "67VJN647",
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2366.531f, 4903.5f, 41.53547f)
        };

        lostB7 = new Ped("U_F_Y_BIKERCHIC", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 8,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2360.811f, 4912.511f, 42.01424f)
        };
        lostB7.SetVariation(0, 0, 0);
        lostB7.SetVariation(2, 0, 0);
        lostB7.SetVariation(3, 0, 0);
        lostB7.SetVariation(4, 0, 0);
        lostB7.Inventory.Weapons.Add(WeaponHash.MG).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB7.Tasks.ClearImmediately();
        lostB7.Heading = 0f;

        lostB3 = new Ped("G_F_Y_LOST_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 20,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2366.028f, 4918.902f, 42.02856f)
        };
        lostB3.SetVariation(0, 1, 1);
        lostB3.SetVariation(2, 1, 0);
        lostB3.SetVariation(3, 1, 1);
        lostB3.SetVariation(4, 1, 1);
        lostB3.SetVariation(8, 1, 0);
        lostB3.Inventory.Weapons.Add(WeaponHash.Molotov).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB3.Tasks.ClearImmediately();
        lostB3.Heading = 0f;

        lostB8 = new Ped("G_F_Y_LOST_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            MaxHealth = 500,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2360.141f, 4907.815f, 42.04505f)
        };
        lostB8.SetVariation(0, 1, 0);
        lostB8.SetVariation(2, 3, 2);
        lostB8.SetVariation(3, 0, 1);
        lostB8.SetVariation(4, 0, 1);
        lostB8.SetVariation(8, 0, 0);
        lostB8.Inventory.Weapons.Add(WeaponHash.SawnOffShotgun).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB8.Tasks.ClearImmediately();
        lostB8.Heading = 0f;

        lostB4 = new Ped("G_M_Y_LOST_03", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 1,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2366.271f, 4913.52f, 42.02041f)
        };
        lostB4.SetVariation(0, 0, 1);
        lostB4.SetVariation(3, 1, 0);
        lostB4.SetVariation(4, 0, 2);
        lostB4.SetVariation(5, 0, 0);
        lostB4.SetVariation(6, 1, 0);
        lostB4.SetVariation(8, 0, 0);
        lostB4.Inventory.Weapons.Add(WeaponHash.PumpShotgun).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB4.Tasks.ClearImmediately();
        lostB4.Heading = 0f;

        lostB6 = new Ped("G_M_Y_LOST_03", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 1,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2367.115f, 4908.125f, 42.05349f)
        };
        lostB6.SetVariation(0, 1, 0);
        lostB6.SetVariation(3, 0, 1);
        lostB6.SetVariation(4, 1, 0);
        lostB6.SetVariation(5, 0, 0);
        lostB6.SetVariation(6, 0, 0);
        lostB6.SetVariation(8, 1, 0);
        lostB6.Inventory.Weapons.Add(WeaponHash.Crowbar).Ammo = 0;
        // Newly spawned Peds will try to face north.
        lostB6.Tasks.ClearImmediately();
        lostB6.Heading = 0f;

        lostB9 = new Ped("G_M_Y_LOST_03", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 20,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2355.245f, 4910.954f, 41.99485f)
        };
        lostB9.SetVariation(0, 0, 2);
        lostB9.SetVariation(3, 1, 2);
        lostB9.SetVariation(4, 0, 1);
        lostB9.SetVariation(5, 0, 0);
        lostB9.SetVariation(6, 0, 0);
        lostB9.SetVariation(8, 0, 0);
        lostB9.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB9.Tasks.ClearImmediately();
        lostB9.Heading = 0f;

        lostB1 = new Ped("G_M_Y_LOST_03", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 6,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2360.816f, 4918.467f, 42.02508f)
        };
        lostB1.SetVariation(0, 1, 1);
        lostB1.SetVariation(3, 0, 0);
        lostB1.SetVariation(4, 1, 1);
        lostB1.SetVariation(5, 0, 0);
        lostB1.SetVariation(6, 1, 0);
        lostB1.SetVariation(8, 1, 0);
        lostB1.Inventory.Weapons.Add(WeaponHash.Pistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB1.Tasks.ClearImmediately();
        lostB1.Heading = 0f;

        lostB10 = new Ped("G_M_Y_LOST_02", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 0,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2355.828f, 4912.651f, 42.08524f)
        };
        lostB10.SetVariation(0, 1, 1);
        lostB10.SetVariation(2, 0, 1);
        lostB10.SetVariation(3, 1, 5);
        lostB10.SetVariation(4, 0, 2);
        lostB10.SetVariation(10, 1, 1);
        lostB10.Inventory.Weapons.Add(WeaponHash.Smg).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB10.Tasks.ClearImmediately();
        lostB10.Heading = 0f;

        lostB5 = new Ped("G_M_Y_LOST_02", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 16,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2365.196f, 4910.373f, 42.01021f)
        };
        lostB5.SetVariation(0, 1, 0);
        lostB5.SetVariation(2, 1, 0);
        lostB5.SetVariation(3, 1, 3);
        lostB5.SetVariation(4, 0, 1);
        lostB5.SetVariation(10, 1, 0);
        lostB5.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB5.Tasks.ClearImmediately();
        lostB5.Heading = 0f;

        lostB2 = new Ped("G_M_Y_LOST_02", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 6,
            RelationshipGroup = new RelationshipGroup("LOSTERS"),
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(2373.466f, 4918.67f, 42.09394f)
        };
        lostB2.SetVariation(0, 0, 2);
        lostB2.SetVariation(2, 1, 1);
        lostB2.SetVariation(3, 1, 2);
        lostB2.SetVariation(4, 1, 0);
        lostB2.SetVariation(10, 1, 0);
        lostB2.Inventory.Weapons.Add(WeaponHash.MicroSMG).Ammo = -1;
        // Newly spawned Peds will try to face north.
        lostB2.Tasks.ClearImmediately();
        lostB2.Heading = 0f;
    }
}