#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes;

internal static class Mafia3Setup
{
    //ConstructMafia3Scene(out limo, out benson, out benson2, out benson3, out mpExarmy, out mpPros, out mpPros2, out mpPros3, out mpPros4, out mpPros5, out mpPros6, out igBankman, out chemwork, out chemwork2, out chemwork3, out korlieut, out insurgent);

    internal static void ConstructMafia3Scene(
        out Vehicle limo,
        out Vehicle insurgent,
        out Vehicle benson,
        out Vehicle benson2,
        out Vehicle benson3,
        out Ped mpExarmy,
        out Ped mpPros,
        out Ped mpPros2,
        out Ped mpPros3,
        out Ped mpPros4,
        out Ped mpPros5,
        out Ped mpPros6,
        out Ped igBankman,
        out Ped chemwork,
        out Ped chemwork2,
        out Ped chemwork3,
        out Ped korlieut
    )
    {
        limo = new Vehicle("LIMO2", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 5.324934E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.27f,
            NumberOfGears = 4,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 65, 67, 71),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 8f,
            LicensePlate = "41GXM801",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(918.6108f, -3114.654f, 5.423703f),
        };

        insurgent = new Vehicle("INSURGENT", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 5.044674E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite2,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            SteeringScale = -2.754193E-15f,
            DriveForce = 0.24f,
            NumberOfGears = 6,
            RimColor = Color.FromArgb(255, 59, 53, 45),
            PearlescentColor = Color.FromArgb(255, 87, 89, 97),
            SecondaryColor = Color.FromArgb(255, 59, 53, 45),
            PrimaryColor = Color.FromArgb(255, 59, 53, 45),
            CanTiresBurst = false,
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 0.007985972f,
            LicensePlate = "02SHJ554",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(-0.0004915932f, 0.000567494f, 0.7221169f, -0.6917706f),
            Position = new Vector3(969.2343f, -3117.488f, 5.818705f),
        };

        benson = new Vehicle("BENSON", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 6.305843E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.16f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 240, 240, 240),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 5f,
            LicensePlate = "81XVR507",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9987034f, -0.05090772f),
            Position = new Vector3(941.5198f, -3128.871f, 5.900991f),
        };

        benson2 = new Vehicle("BENSON", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 5.324934E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.16f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            DirtLevel = 8f,
            LicensePlate = "00BUC112",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9999671f, -0.008110749f),
            Position = new Vector3(949.3857f, -3129.14f, 5.900989f),
        };

        benson3 = new Vehicle("BENSON", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 5.324934E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite1,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.16f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 255, 255, 255),
            PrimaryColor = Color.FromArgb(255, 255, 255, 255),
            FuelLevel = 80f,
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)1,
            LicensePlate = "69HGV923",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9993356f, -0.03644727f),
            Position = new Vector3(957.622f, -3128.011f, 5.900993f),
        };

        mpExarmy = new Ped("MP_M_EXARMY_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 4,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.6732005f, 0.73946f),
            Position = new Vector3(968.8803f, -3120.314f, 5.900803f),
        };
        mpExarmy.SetVariation(0, 0, 0);
        mpExarmy.SetVariation(2, 0, 0);
        mpExarmy.SetVariation(3, 0, 0);
        mpExarmy.SetVariation(4, 0, 0);
        mpExarmy.SetVariation(8, 0, 0);
        mpExarmy.Tasks.ClearImmediately();
        mpExarmy.Heading = 275.3709f;

        mpPros = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 11,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.0325971f, 0.9994686f),
            Position = new Vector3(952.902f, -3121.451f, 5.900805f),
        };
        mpPros.SetVariation(0, 1, 2);
        mpPros.SetVariation(3, 2, 2);
        mpPros.SetVariation(4, 0, 2);
        mpPros.SetVariation(5, 0, 1);
        mpPros.SetVariation(8, 2, 0);
        mpPros.SetVariation(9, 2, 0);
        mpPros.SetVariation(11, 0, 1);
        mpPros.Tasks.ClearImmediately();
        mpPros.Heading = 356.264f;

        mpPros2 = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 13,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.6239893f, 0.7814329f),
            Position = new Vector3(919.1266f, -3119.027f, 5.900805f),
        };
        mpPros2.SetVariation(3, 2, 2);
        mpPros2.SetVariation(4, 0, 2);
        mpPros2.SetVariation(8, 2, 0);
        mpPros2.SetVariation(9, 2, 0);
        mpPros2.SetVariation(11, 0, 1);
        mpPros2.Tasks.ClearImmediately();
        mpPros2.Heading = 77.21609f;

        mpPros3 = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 11,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.6081227f, 0.793843f),
            Position = new Vector3(919.7299f, -3109.879f, 5.900803f),
        };
        mpPros3.SetVariation(3, 2, 2);
        mpPros3.SetVariation(4, 0, 2);
        mpPros3.SetVariation(8, 2, 0);
        mpPros3.SetVariation(9, 2, 0);
        mpPros3.SetVariation(11, 0, 1);
        mpPros3.Tasks.ClearImmediately();
        mpPros3.Heading = 74.90777f;

        mpPros4 = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 18,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.6868105f, 0.7268366f),
            Position = new Vector3(969.6948f, -3113.865f, 5.900803f),
        };
        mpPros4.SetVariation(0, 1, 0);
        mpPros4.SetVariation(3, 2, 2);
        mpPros4.SetVariation(4, 0, 2);
        mpPros4.SetVariation(5, 0, 1);
        mpPros4.SetVariation(8, 2, 0);
        mpPros4.SetVariation(9, 2, 0);
        mpPros4.SetVariation(11, 0, 1);
        mpPros4.Tasks.ClearImmediately();
        mpPros4.Heading = 273.2437f;

        mpPros5 = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 22,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.6883097f, 0.725417f),
            Position = new Vector3(935.6395f, -3125.215f, 5.900803f),
        };
        mpPros5.SetVariation(3, 1, 0);
        mpPros5.SetVariation(4, 1, 0);
        mpPros5.SetVariation(8, 2, 0);
        mpPros5.SetVariation(9, 2, 0);
        mpPros5.SetVariation(11, 0, 1);
        mpPros5.Tasks.ClearImmediately();
        mpPros5.Heading = 86.99291f;

        mpPros6 = new Ped("MP_G_M_PROS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 16,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.4094613f, 0.9123275f),
            Position = new Vector3(961.3345f, -3131.792f, 5.900803f),
        };
        mpPros6.SetVariation(3, 1, 0);
        mpPros6.SetVariation(4, 1, 0);
        mpPros6.SetVariation(8, 2, 0);
        mpPros6.SetVariation(9, 2, 0);
        mpPros6.SetVariation(11, 0, 1);
        mpPros6.Tasks.ClearImmediately();
        mpPros6.Heading = 311.658f;

        igBankman = new Ped("IG_BANKMAN", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 11,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9995623f, -0.02958586f),
            Position = new Vector3(949.1386f, -3113.797f, 5.900801f),
        };
        igBankman.SetVariation(0, 0, 0);
        igBankman.SetVariation(2, 0, 0);
        igBankman.SetVariation(3, 0, 0);
        igBankman.SetVariation(4, 0, 0);
        igBankman.SetVariation(5, 0, 0);
        igBankman.SetVariation(6, 0, 0);
        igBankman.SetVariation(7, 0, 0);
        igBankman.SetVariation(8, 0, 0);
        igBankman.Tasks.ClearImmediately();
        igBankman.Heading = 183.3908f;

        chemwork = new Ped("G_M_M_CHEMWORK_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 4,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9624053f, -0.2716177f),
            Position = new Vector3(948.4152f, -3121.584f, 5.900801f),
        };
        chemwork.SetVariation(0, 1, 1);
        chemwork.SetVariation(3, 0, 0);
        chemwork.SetVariation(4, 0, 0);
        chemwork.SetVariation(5, 0, 0);
        chemwork.SetVariation(6, 0, 0);
        chemwork.SetVariation(8, 0, 0);
        chemwork.Tasks.ClearImmediately();
        chemwork.Heading = 211.5211f;

        chemwork2 = new Ped("G_M_M_CHEMWORK_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 1,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9860164f, 0.1666483f),
            Position = new Vector3(958.549f, -3120.159f, 5.900803f),
        };
        chemwork2.SetVariation(0, 1, 1);
        chemwork2.SetVariation(3, 0, 0);
        chemwork2.SetVariation(4, 0, 0);
        chemwork2.SetVariation(5, 0, 0);
        chemwork2.SetVariation(6, 0, 0);
        chemwork2.SetVariation(8, 0, 0);
        chemwork2.Tasks.ClearImmediately();
        chemwork2.Heading = 160.814f;

        chemwork3 = new Ped("G_M_M_CHEMWORK_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 21,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9860165f, 0.1666481f),
            Position = new Vector3(941.9926f, -3121.393f, 5.900803f),
        };
        chemwork3.SetVariation(0, 1, 1);
        chemwork3.SetVariation(3, 0, 0);
        chemwork3.SetVariation(4, 0, 0);
        chemwork3.SetVariation(5, 0, 0);
        chemwork3.SetVariation(6, 0, 0);
        chemwork3.SetVariation(8, 0, 0);
        chemwork3.Tasks.ClearImmediately();
        chemwork3.Heading = 160.814f;

        korlieut = new Ped("G_M_Y_KORLIEUT_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 11,
            RelationshipGroup = new RelationshipGroup("NO_RELATIONSHIP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.9437738f, -0.3305919f),
            Position = new Vector3(946.0991f, -3113.62f, 5.900803f),
        };
        korlieut.SetVariation(0, 0, 1);
        korlieut.SetVariation(2, 0, 1);
        korlieut.SetVariation(3, 1, 1);
        korlieut.SetVariation(4, 0, 2);
        korlieut.Tasks.ClearImmediately();
        korlieut.Heading = 218.6094f;
    }
}
