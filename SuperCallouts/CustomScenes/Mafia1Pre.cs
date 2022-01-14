#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes
{
    internal class Mafia1Pre
    {
        internal static void BuildPreScene(out Ped fibarchitect, out Ped mpFibsec, out Ped swat, out Ped swat2, out Ped fiboffice, out Vehicle fbi, out Vehicle riot)
        {
            fibarchitect = new Ped("U_M_M_FIBARCHITECT", Vector3.Zero, 0f);
            fibarchitect.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            fibarchitect.Money = 6;
            fibarchitect.RelationshipGroup = new RelationshipGroup("COP");
            fibarchitect.CollisionIgnoredEntity = null;
            fibarchitect.AngularVelocity = new Rotator(0f, 0f, 0f);
            fibarchitect.Velocity = new Vector3(0f, 0f, 0f);
            fibarchitect.Orientation = new Quaternion(0f, 0f, 0.9721142f, 0.2345079f);
            fibarchitect.Position = new Vector3(-342.0603f, -962.7352f, 31.08061f);
            fibarchitect.SetVariation(0, 0, 0);
            fibarchitect.SetVariation(3, 0, 0);
            fibarchitect.SetVariation(4, 0, 0);
            fibarchitect.SetVariation(8, 0, 0);
            fibarchitect.Tasks.ClearImmediately();
            fibarchitect.Heading = 152.8748f;

            mpFibsec = new Ped("MP_M_FIBSEC_01", Vector3.Zero, 0f);
            mpFibsec.DecisionMaker = new DecisionMaker(0xa49e591cu);
            mpFibsec.Money = 3;
            mpFibsec.RelationshipGroup = new RelationshipGroup("COP");
            mpFibsec.CollisionIgnoredEntity = null;
            mpFibsec.AngularVelocity = new Rotator(0f, 0f, 0f);
            mpFibsec.Velocity = new Vector3(0f, 0f, 0f);
            mpFibsec.Orientation = new Quaternion(0f, 0f, 0.8452248f, 0.5344109f);
            mpFibsec.Position = new Vector3(-339.8573f, -963.5591f, 31.08061f);
            mpFibsec.SetVariation(0, 1, 0);
            mpFibsec.SetVariation(2, 0, 0);
            mpFibsec.SetVariation(3, 0, 1);
            mpFibsec.SetVariation(4, 0, 0);
            mpFibsec.SetVariation(10, 0, 0);
            mpFibsec.Tasks.ClearImmediately();
            mpFibsec.Heading = 115.3921f;

            fbi = new Vehicle("FBI", Vector3.Zero, 0f);
            fbi.VerticalFlightPhase = 0f;
            fbi.DesiredVerticalFlightPhase = 6.586103E-44f;
            fbi.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
            fbi.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            fbi.SteeringAngle = 40f;
            fbi.SteeringScale = 1.222248E-16f;
            fbi.DriveForce = 0.28f;
            fbi.RimColor = Color.FromArgb(255, 65, 67, 71);
            fbi.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            fbi.SecondaryColor = Color.FromArgb(255, 15, 15, 15);
            fbi.PrimaryColor = Color.FromArgb(255, 15, 15, 15);
            fbi.IsDeformationEnabled = false;
            fbi.CanTiresBurst = false;
            fbi.FuelTankHealth = 2000f;
            fbi.EngineHealth = 2000f;
            fbi.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            fbi.LockStatus = (VehicleLockStatus) 1;
            fbi.DirtLevel = 0.04603858f;
            fbi.LicensePlate = "89LTV217";
            fbi.IsSirenSilent = true;
            fbi.IsMeleeProof = true;
            fbi.IsCollisionProof = true;
            fbi.IsExplosionProof = true;
            fbi.IsBulletProof = true;
            fbi.CollisionIgnoredEntity = null;
            fbi.AngularVelocity = new Rotator(0f, 0f, 0f);
            fbi.Velocity = new Vector3(0f, 0f, 0f);
            fbi.Orientation = new Quaternion(-0.005689827f, -0.00384988f, 0.5616957f, 0.8273154f);
            fbi.Position = new Vector3(-340.1805f, -961.6083f, 30.57838f);

            riot = new Vehicle("RIOT", Vector3.Zero, 0f);
            riot.VerticalFlightPhase = 0f;
            riot.DesiredVerticalFlightPhase = 7.006492E-44f;
            riot.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
            riot.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            riot.SteeringAngle = 40f;
            riot.SteeringScale = -0.0002079709f;
            riot.DriveForce = 0.12f;
            riot.RimColor = Color.FromArgb(255, 65, 67, 71);
            riot.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            riot.SecondaryColor = Color.FromArgb(255, 8, 8, 8);
            riot.PrimaryColor = Color.FromArgb(255, 240, 240, 240);
            riot.IsDeformationEnabled = false;
            riot.CanTiresBurst = false;
            riot.FuelTankHealth = 2000f;
            riot.EngineHealth = 2000f;
            riot.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            riot.LockStatus = (VehicleLockStatus) 1;
            riot.DirtLevel = 0.004502276f;
            riot.LicensePlate = "83NSZ428";
            riot.IsMeleeProof = true;
            riot.IsCollisionProof = true;
            riot.IsExplosionProof = true;
            riot.IsBulletProof = true;
            riot.CollisionIgnoredEntity = null;
            riot.AngularVelocity = new Rotator(0f, 0f, 0f);
            riot.Velocity = new Vector3(0f, 0f, 0f);
            riot.Orientation = new Quaternion(-2.960484E-05f, 0.0006024266f, -0.1478644f, 0.9890075f);
            riot.Position = new Vector3(-342.9797f, -972.0889f, 30.73344f);

            swat = new Ped("S_M_Y_SWAT_01", Vector3.Zero, 0f);
            swat.DecisionMaker = new DecisionMaker(0x98787966u);
            swat.Armor = 100;
            swat.Money = 0;
            swat.RelationshipGroup = new RelationshipGroup("COP");
            swat.CollisionIgnoredEntity = null;
            swat.AngularVelocity = new Rotator(0f, 0f, 0f);
            swat.Velocity = new Vector3(0f, 0f, 0f);
            swat.Orientation = new Quaternion(0f, 0f, -0.1203384f, 0.9927329f);
            swat.Position = new Vector3(-347.8616f, -968.1098f, 31.08061f);
            swat.SetVariation(0, 0, 0);
            swat.SetVariation(3, 0, 0);
            swat.SetVariation(4, 0, 0);
            swat.SetVariation(10, 0, 0);
            swat.Tasks.ClearImmediately();
            swat.Heading = 346.1767f;

            swat2 = new Ped("S_M_Y_SWAT_01", Vector3.Zero, 0f);
            swat2.DecisionMaker = new DecisionMaker(0x98787966u);
            swat2.Armor = 100;
            swat2.Money = 5;
            swat2.RelationshipGroup = new RelationshipGroup("COP");
            swat2.CollisionIgnoredEntity = null;
            swat2.AngularVelocity = new Rotator(0f, 0f, 0f);
            swat2.Velocity = new Vector3(0f, 0f, 0f);
            swat2.Orientation = new Quaternion(0f, 0f, 0.2061085f, 0.9785292f);
            swat2.Position = new Vector3(-347.0247f, -967.9576f, 31.08061f);
            swat2.SetVariation(0, 0, 1);
            swat2.SetVariation(3, 0, 0);
            swat2.SetVariation(4, 0, 0);
            swat2.SetVariation(10, 0, 0);
            swat2.Tasks.ClearImmediately();
            swat2.Heading = 23.7888f;

            fiboffice = new Ped("S_M_M_FIBOFFICE_01", Vector3.Zero, 0f);
            fiboffice.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            fiboffice.Money = 20;
            fiboffice.RelationshipGroup = new RelationshipGroup("COP");
            fiboffice.CollisionIgnoredEntity = null;
            fiboffice.AngularVelocity = new Rotator(0f, 0f, 0f);
            fiboffice.Velocity = new Vector3(0f, 0f, 0f);
            fiboffice.Orientation = new Quaternion(0f, 0f, 0.9892623f, 0.1461505f);
            fiboffice.Position = new Vector3(-347.8155f, -966.1456f, 31.08061f);
            fiboffice.SetVariation(0, 0, 0);
            fiboffice.SetVariation(3, 0, 0);
            fiboffice.SetVariation(4, 0, 0);
            fiboffice.Tasks.ClearImmediately();
            fiboffice.Heading = 163.1922f;
        }
    }
}