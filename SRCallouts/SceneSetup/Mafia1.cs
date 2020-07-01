#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SRCallouts.SceneSetup
{
    internal class Mafia1
    {
        internal static void BuildScene(out Ped xdb, out Ped xbeb, out Ped xa, out Ped xcdcb, out Ped xbaa2, out Ped xcdcb2, out Ped xbaa, out Ped xbaa3, out Vehicle limo, out Vehicle boxville, out Vehicle btype, out Vehicle benson)
        {
            xdb = new Ped(new Model(0x1422d45b), Vector3.Zero, 0f);
            xdb.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xdb.Money = 2;
            xdb.RelationshipGroup = new RelationshipGroup("MAFIA");
            xdb.CollisionIgnoredEntity = null;
            xdb.AngularVelocity = new Rotator(0f, 0f, 0f);
            xdb.Velocity = new Vector3(0f, 0f, 0f);
            xdb.Orientation = new Quaternion(-0.0005306705f, -0.0007321094f, 0.8096673f, 0.5868884f);
            xdb.Position = new Vector3(891.6799f, 20.89992f, 78.89739f);
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

            xbeb = new Ped(new Model(0x2be886b2), Vector3.Zero, 0f);
            xbeb.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xbeb.Money = 17;
            xbeb.RelationshipGroup = new RelationshipGroup("MAFIA");
            xbeb.CollisionIgnoredEntity = null;
            xbeb.AngularVelocity = new Rotator(0f, 0f, 0f);
            xbeb.Velocity = new Vector3(0f, 0f, 0f);
            xbeb.Orientation = new Quaternion(0f, 0f, 0.3590865f, 0.9333043f);
            xbeb.Position = new Vector3(884.157f, 9.278169f, 78.90435f);
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

            limo = new Vehicle("LIMO2", Vector3.Zero, 0f);
            limo.VerticalFlightPhase = 0f;
            limo.DesiredVerticalFlightPhase = 6.586103E-44f;
            limo.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
            limo.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            limo.DriveForce = 0.27f;
            limo.NumberOfGears = 4;
            limo.RimColor = Color.FromArgb(255, 65, 67, 71);
            limo.PearlescentColor = Color.FromArgb(255, 33, 8, 4);
            limo.SecondaryColor = Color.FromArgb(255, 78, 49, 255);
            limo.PrimaryColor = Color.FromArgb(255, 162, 177, 255);
            limo.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            limo.LockStatus = (VehicleLockStatus) 1;
            limo.DirtLevel = 0f;
            limo.LicensePlate = "21MFC584";
            limo.CollisionIgnoredEntity = null;
            limo.IsGravityDisabled = false;
            limo.AngularVelocity = new Rotator(0f, 0f, 0f);
            limo.Velocity = new Vector3(0f, 0f, 0f);
            limo.Orientation = new Quaternion(0f, 0f, 0.8763167f, -0.4817355f);
            limo.Position = new Vector3(898.3679f, 18.40567f, 78.39939f);

            boxville = new Vehicle("BOXVILLE3", Vector3.Zero, 0f);
            boxville.VerticalFlightPhase = 0f;
            boxville.DesiredVerticalFlightPhase = 6.586103E-44f;
            boxville.LicensePlateStyle = LicensePlateStyle.BlueOnWhite1;
            boxville.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            boxville.DriveForce = 0.11f;
            boxville.NumberOfGears = 4;
            boxville.RimColor = Color.FromArgb(255, 65, 67, 71);
            boxville.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            boxville.SecondaryColor = Color.FromArgb(255, 0, 255, 255);
            boxville.PrimaryColor = Color.FromArgb(255, 181, 255, 255);
            boxville.FuelLevel = 75f;
            boxville.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            boxville.LockStatus = (VehicleLockStatus) 1;
            boxville.DirtLevel = 4f;
            boxville.LicensePlate = "24NFG392";
            boxville.CollisionIgnoredEntity = null;
            boxville.IsGravityDisabled = false;
            boxville.AngularVelocity = new Rotator(0f, 0f, 0f);
            boxville.Velocity = new Vector3(0f, 0f, 0f);
            boxville.Orientation = new Quaternion(0f, 0f, 0.4547044f, 0.8906424f);
            boxville.Position = new Vector3(909.564f, 4.041251f, 78.67892f);

            btype = new Vehicle("BTYPE3", Vector3.Zero, 0f);
            btype.VerticalFlightPhase = 0f;
            btype.DesiredVerticalFlightPhase = 7.707142E-44f;
            btype.LicensePlateStyle = LicensePlateStyle.BlueOnWhite2;
            btype.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            btype.DriveForce = 0.27f;
            btype.NumberOfGears = 4;
            btype.RimColor = Color.FromArgb(255, 65, 67, 71);
            btype.PearlescentColor = Color.FromArgb(255, 59, 70, 84);
            btype.SecondaryColor = Color.FromArgb(255, 111, 147, 255);
            btype.PrimaryColor = Color.FromArgb(255, 181, 212, 255);
            btype.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            btype.LockStatus = (VehicleLockStatus) 1;
            btype.LicensePlate = "43MPN584";
            btype.CollisionIgnoredEntity = null;
            btype.IsGravityDisabled = false;
            btype.AngularVelocity = new Rotator(0f, 0f, 0f);
            btype.Velocity = new Vector3(0f, 0f, 0f);
            btype.Orientation = new Quaternion(0f, 0f, 0f, 1f);
            btype.Position = new Vector3(903.9366f, 4.442619f, 78.22449f);

            xa = new Ped(new Model(0x035456a4), Vector3.Zero, 0f);
            xa.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xa.Money = 13;
            xa.RelationshipGroup = new RelationshipGroup("MAFIA");
            xa.CollisionIgnoredEntity = null;
            xa.AngularVelocity = new Rotator(0f, 0f, 0f);
            xa.Velocity = new Vector3(0f, 0f, 0f);
            xa.Orientation = new Quaternion(0f, 0f, 0.9996499f, -0.02645799f);
            xa.Position = new Vector3(903.2559f, 7.793941f, 78.77438f);
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

            xcdcb = new Ped(new Model(0x3cd28cb3), Vector3.Zero, 0f);
            xcdcb.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xcdcb.Money = 5;
            xcdcb.RelationshipGroup = new RelationshipGroup("MAFIA");
            xcdcb.CollisionIgnoredEntity = null;
            xcdcb.AngularVelocity = new Rotator(0f, 0f, 0f);
            xcdcb.Velocity = new Vector3(0f, 0f, 0f);
            xcdcb.Orientation = new Quaternion(0f, 0f, -0.2186501f, 0.9758033f);
            xcdcb.Position = new Vector3(910.5573f, 32.27618f, 80.27606f);
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

            xbaa2 = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f);
            xbaa2.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xbaa2.Money = 14;
            xbaa2.RelationshipGroup = new RelationshipGroup("MAFIA");
            xbaa2.CollisionIgnoredEntity = null;
            xbaa2.AngularVelocity = new Rotator(0f, 0f, 0f);
            xbaa2.Velocity = new Vector3(0f, 0f, 0f);
            xbaa2.Orientation = new Quaternion(0f, 0f, -0.2696833f, 0.962949f);
            xbaa2.Position = new Vector3(911.2944f, 43.06391f, 80.89889f);
            xbaa2.SetVariation(0, 2, 0);
            xbaa2.SetVariation(2, 2, 0);
            xbaa2.SetVariation(3, 2, 0);
            xbaa2.SetVariation(4, 0, 0);
            xbaa2.SetVariation(6, 2, 0);
            xbaa2.Tasks.ClearImmediately();
            xbaa2.Heading = 328.7092f;

            benson = new Vehicle("BENSON", Vector3.Zero, 0f);
            benson.VerticalFlightPhase = 0f;
            benson.DesiredVerticalFlightPhase = 6.165713E-44f;
            benson.LicensePlateStyle = LicensePlateStyle.BlueOnWhite1;
            benson.AlarmTimeLeft = TimeSpan.FromSeconds(0d);
            benson.DriveForce = 0.16f;
            benson.RimColor = Color.FromArgb(255, 65, 67, 71);
            benson.PearlescentColor = Color.FromArgb(255, 8, 8, 8);
            benson.SecondaryColor = Color.FromArgb(255, 0, 255, 255);
            benson.PrimaryColor = Color.FromArgb(255, 181, 255, 255);
            benson.FuelLevel = 80f;
            benson.ConvertibleRoofState = VehicleConvertibleRoofState.Raised;
            benson.LockStatus = (VehicleLockStatus) 1;
            benson.DirtLevel = 8f;
            benson.LicensePlate = "22CRW231";
            benson.CollisionIgnoredEntity = null;
            benson.IsGravityDisabled = false;
            benson.AngularVelocity = new Rotator(0f, 0f, 0f);
            benson.Velocity = new Vector3(0f, 0f, 0f);
            benson.Orientation = new Quaternion(0f, 0f, -0.295084f, 0.9554713f);
            benson.Position = new Vector3(916.3431f, 47.8009f, 80.89904f);

            xcdcb2 = new Ped(new Model(0x3cd28cb3), Vector3.Zero, 0f);
            xcdcb2.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xcdcb2.Money = 20;
            xcdcb2.RelationshipGroup = new RelationshipGroup("MAFIA");
            xcdcb2.CollisionIgnoredEntity = null;
            xcdcb2.AngularVelocity = new Rotator(0f, 0f, 0f);
            xcdcb2.Velocity = new Vector3(0f, 0f, 0f);
            xcdcb2.Orientation = new Quaternion(0f, 0f, 0.9779977f, 0.208616f);
            xcdcb2.Position = new Vector3(911.3863f, 33.33047f, 80.37089f);
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

            xbaa = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f);
            xbaa.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xbaa.Money = 3;
            xbaa.RelationshipGroup = new RelationshipGroup("MAFIA");
            xbaa.CollisionIgnoredEntity = null;
            xbaa.AngularVelocity = new Rotator(0f, 0f, 0f);
            xbaa.Velocity = new Vector3(0f, 0f, 0f);
            xbaa.Orientation = new Quaternion(0f, 0f, 0.9649342f, 0.2624918f);
            xbaa.Position = new Vector3(903.2636f, 27.42938f, 79.66326f);
            xbaa.SetVariation(0, 4, 0);
            xbaa.SetVariation(2, 2, 0);
            xbaa.SetVariation(3, 1, 0);
            xbaa.SetVariation(4, 1, 0);
            xbaa.SetVariation(6, 1, 0);
            xbaa.Tasks.ClearImmediately();
            xbaa.Heading = 149.5641f;

            xbaa3 = new Ped(new Model(0x2102ba2a), Vector3.Zero, 0f);
            xbaa3.DecisionMaker = new DecisionMaker(0xe4df46d5u);
            xbaa3.Money = 14;
            xbaa3.RelationshipGroup = new RelationshipGroup("MAFIA");
            xbaa3.CollisionIgnoredEntity = null;
            xbaa3.AngularVelocity = new Rotator(0f, 0f, 0f);
            xbaa3.Velocity = new Vector3(0f, 0f, 0f);
            xbaa3.Orientation = new Quaternion(0f, 0f, -0.598518f, 0.8011094f);
            xbaa3.Position = new Vector3(900.7629f, 3.734434f, 78.77782f);
            xbaa3.SetVariation(0, 2, 1);
            xbaa3.SetVariation(2, 2, 0);
            xbaa3.SetVariation(3, 0, 0);
            xbaa3.SetVariation(4, 0, 0);
            xbaa3.SetVariation(6, 0, 0);
            xbaa3.Tasks.ClearImmediately();
            xbaa3.Heading = 286.4724f;
        }
    }
}