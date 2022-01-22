#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes
{
    internal class TruckCrashSetup
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
            pounder = new Vehicle("POUNDER", Vector3.Zero, 0f);
            pounder.DesiredVerticalFlightPhase = 6.586103E-44f;
            pounder.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
            pounder.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            pounder.SteeringAngle = -0.4969919f;
            pounder.DriveForce = 0.14f;
            pounder.NumberOfGears = 4;
            pounder.RimColor = Color.FromArgb(255, 65, 67, 71);
            pounder.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            pounder.SecondaryColor = Color.FromArgb(255, 8, 8, 8);
            pounder.PrimaryColor = Color.FromArgb(255, 31, 34, 38);
            pounder.IsEngineOn = true;
            pounder.FuelLevel = 80f;
            pounder.FuelTankHealth = 776.2667f;
            pounder.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            pounder.LockStatus = (VehicleLockStatus)1;
            pounder.DirtLevel = 3f;
            pounder.LicensePlate = "85XIG328";
            pounder.CollisionIgnoredEntity = null;
            pounder.Health = 449;
            pounder.AngularVelocity = new Rotator(0f, 0f, 0f);
            pounder.Velocity = new Vector3(0f, 0f, 0f);
            pounder.Orientation = new Quaternion(-0.2163455f, 0.6532158f, 0.2232166f, 0.6904187f);
            pounder.Rotation = new Rotator(-7, 90, 28);
            pounder.Position = new Vector3(2447.409f, -177.647f, 88.64472f);
            pounder.IsPersistent = true;

            bison = new Vehicle("BISON", Vector3.Zero, 0f);
            bison.DesiredVerticalFlightPhase = 5.605194E-45f;
            bison.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
            bison.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            bison.DriveForce = 0.2f;
            bison.RimColor = Color.FromArgb(255, 65, 67, 71);
            bison.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            bison.SecondaryColor = Color.FromArgb(255, 8, 8, 8);
            bison.PrimaryColor = Color.FromArgb(255, 90, 94, 102);
            bison.FuelLevel = 70f;
            bison.FuelTankHealth = 710.7716f;
            bison.EngineHealth = 10f;
            bison.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            bison.LockStatus = (VehicleLockStatus)1;
            bison.DirtLevel = 6f;
            bison.LicensePlate = "64UIZ986";
            bison.IsDriveable = false;
            bison.CollisionIgnoredEntity = null;
            bison.Health = 10;
            bison.AngularVelocity = new Rotator(0f, 0f, 0f);
            bison.Velocity = new Vector3(0f, 0f, 0f);
            bison.Orientation = new Quaternion(0.01615246f, -0.01930955f, -0.246657f, 0.9687758f);
            bison.Position = new Vector3(2444.1f, -179.6697f, 87.55006f);
            bison.IsPersistent = true;

            felon = new Vehicle("FELON", Vector3.Zero, 0f);
            felon.DesiredVerticalFlightPhase = 5.885454E-44f;
            felon.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
            felon.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            felon.SteeringAngle = -19.82369f;
            felon.DriveForce = 0.24f;
            felon.NumberOfGears = 6;
            felon.RimColor = Color.FromArgb(255, 65, 67, 71);
            felon.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            felon.SecondaryColor = Color.FromArgb(255, 8, 8, 8);
            felon.PrimaryColor = Color.FromArgb(255, 28, 30, 33);
            felon.IsEngineOn = true;
            felon.FuelTankHealth = 700f;
            felon.EngineHealth = 390.5054f;
            felon.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            felon.LockStatus = (VehicleLockStatus)1;
            felon.DirtLevel = 0f;
            felon.LicensePlate = "88RTA013";
            felon.CollisionIgnoredEntity = null;
            felon.Health = 0;
            felon.AngularVelocity = new Rotator(-0.026221f, 0.01796695f, 1.589858f);
            felon.Velocity = new Vector3(-0.07780585f, -0.130072f, -0.004479066f);
            felon.Orientation = new Quaternion(0.0157059f, -0.0009472959f, -0.2736966f, 0.9616874f);
            felon.Position = new Vector3(2440.831f, -172.6008f, 87.82504f);
            felon.IsPersistent = true;

            mpStripperlite = new Ped(Vector3.Zero, 0f);
            mpStripperlite.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            mpStripperlite.Money = 24;
            mpStripperlite.RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP");
            mpStripperlite.AngularVelocity = new Rotator(0f, 0f, 0f);
            mpStripperlite.Velocity = new Vector3(0f, 0f, 0f);
            mpStripperlite.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            mpStripperlite.Position = new Vector3(2455.644f, -186.7955f, 87.83904f);
            mpStripperlite.Tasks.ClearImmediately();
            mpStripperlite.Heading = 0f;
            mpStripperlite.IsPersistent = true;

            mpStripperlite2 = new Ped(Vector3.Zero, 0f);
            mpStripperlite2.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            mpStripperlite2.Money = 3;
            mpStripperlite2.RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP");
            mpStripperlite2.AngularVelocity = new Rotator(0f, 0f, 0f);
            mpStripperlite2.Velocity = new Vector3(0f, 0f, 0f);
            mpStripperlite2.Orientation = new Quaternion(0f, 0f, 0.9894379f, -0.1449577f);
            mpStripperlite2.Position = new Vector3(2455.884f, -183.9076f, 87.95329f);
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