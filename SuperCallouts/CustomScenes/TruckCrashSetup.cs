#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes
{
    internal static class TruckCrashSetup
    {
        /* internal Vehicle pounder;
         internal Vehicle bison;
         internal Vehicle felon;
         internal Ped mpStripperlite;
         internal Ped mpStripperlite2;
         internal Ped mpStripperlite3DEAD;*/
        internal static void ConstructTrucksScene(out Ped mpStripperlite, out Ped mpStripperlite2,
            out Ped mpStripperlite3Dead, out Vehicle pounder, out Vehicle bison, out Vehicle felon)
        {
            pounder = new Vehicle("POUNDER", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 6.586103E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                SteeringAngle = -0.4969919f,
                DriveForce = 0.14f,
                NumberOfGears = 4,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 8, 8, 8),
                PrimaryColor = Color.FromArgb(255, 31, 34, 38),
                IsEngineOn = true,
                FuelLevel = 80f,
                FuelTankHealth = 776.2667f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 3f,
                LicensePlate = "85XIG328",
                CollisionIgnoredEntity = null,
                Health = 449,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(-0.2163455f, 0.6532158f, 0.2232166f, 0.6904187f),
                Rotation = new Rotator(-7, 90, 28),
                Position = new Vector3(2447.409f, -177.647f, 88.64472f),
                IsPersistent = true
            };

            bison = new Vehicle("BISON", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 5.605194E-45f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                DriveForce = 0.2f,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 8, 8, 8),
                PrimaryColor = Color.FromArgb(255, 90, 94, 102),
                FuelLevel = 70f,
                FuelTankHealth = 710.7716f,
                EngineHealth = 10f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 6f,
                LicensePlate = "64UIZ986",
                IsDriveable = false,
                CollisionIgnoredEntity = null,
                Health = 10,
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0.01615246f, -0.01930955f, -0.246657f, 0.9687758f),
                Position = new Vector3(2444.1f, -179.6697f, 87.55006f),
                IsPersistent = true
            };

            felon = new Vehicle("FELON", Vector3.Zero, 0f)
            {
                DesiredVerticalFlightPhase = 5.885454E-44f,
                LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
                AlarmTimeLeft = TimeSpan.FromSeconds(0d),
                SteeringAngle = -19.82369f,
                DriveForce = 0.24f,
                NumberOfGears = 6,
                RimColor = Color.FromArgb(255, 65, 67, 71),
                PearlescentColor = Color.FromArgb(255, 8, 8, 8),
                SecondaryColor = Color.FromArgb(255, 8, 8, 8),
                PrimaryColor = Color.FromArgb(255, 28, 30, 33),
                IsEngineOn = true,
                FuelTankHealth = 700f,
                EngineHealth = 390.5054f,
                ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
                LockStatus = (VehicleLockStatus)1,
                DirtLevel = 0f,
                LicensePlate = "88RTA013",
                CollisionIgnoredEntity = null,
                Health = 0,
                AngularVelocity = new Rotator(-0.026221f, 0.01796695f, 1.589858f),
                Velocity = new Vector3(-0.07780585f, -0.130072f, -0.004479066f),
                Orientation = new Quaternion(0.0157059f, -0.0009472959f, -0.2736966f, 0.9616874f),
                Position = new Vector3(2440.831f, -172.6008f, 87.82504f),
                IsPersistent = true
            };

            mpStripperlite = new Ped(Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 24,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0f, 1f),
                Position = new Vector3(2455.644f, -186.7955f, 87.83904f)
            };
            mpStripperlite.Tasks.ClearImmediately();
            mpStripperlite.Heading = 0f;
            mpStripperlite.IsPersistent = true;

            mpStripperlite2 = new Ped(Vector3.Zero, 0f)
            {
                DecisionMaker = new DecisionMaker(0xe4df46d5u),
                Money = 3,
                RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
                AngularVelocity = new Rotator(0f, 0f, 0f),
                Velocity = new Vector3(0f, 0f, 0f),
                Orientation = new Quaternion(0f, 0f, 0.9894379f, -0.1449577f),
                Position = new Vector3(2455.884f, -183.9076f, 87.95329f)
            };
            mpStripperlite2.Tasks.ClearImmediately();
            mpStripperlite2.Heading = 196.6697f;
            mpStripperlite2.IsPersistent = true;

            mpStripperlite3Dead = new Ped(Vector3.Zero, 0f);
            mpStripperlite3Dead.Kill();
            mpStripperlite3Dead.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            mpStripperlite3Dead.Money = 19;
            mpStripperlite3Dead.RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP");
            //mpStripperlite3DEAD.CollisionIgnoredEntity = null;
            mpStripperlite3Dead.AngularVelocity = new Rotator(0f, 0f, 0f);
            mpStripperlite3Dead.Velocity = new Vector3(0f, 0f, 0f);
            mpStripperlite3Dead.Orientation = new Quaternion(0f, 0f, 0.7368686f, 0.676036f);
            mpStripperlite3Dead.Position = new Vector3(2455.753f, -185.5025f, 87.8923f);
            // mpStripperlite3DEAD.SetVariation(0, 0, 0);
            // mpStripperlite3DEAD.SetVariation(2, 0, 0);
            // mpStripperlite3DEAD.SetVariation(3, 0, 0);
            // mpStripperlite3DEAD.SetVariation(4, 0, 0);
            // mpStripperlite3DEAD.SetVariation(8, 0, 0);
            // mpStripperlite3DEAD.SetVariation(9, 0, 0);
            mpStripperlite3Dead.Tasks.ClearImmediately();
            mpStripperlite3Dead.Heading = 94.93069f;
            mpStripperlite3Dead.IsPersistent = true;
        }
    }
}