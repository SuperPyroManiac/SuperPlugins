#region

using System;
using System.Drawing;
using Rage;

#endregion

namespace SuperCallouts.CustomScenes;

internal static class LsgtfSetup
{
    internal static void ConstructLspdraidScene(out Ped ballaorig2, out Ped ballaorig3, out Ped ballasout,
        out Ped ballasout2, out Ped ballaorig, out Ped ballaeast, out Ped ballaorig4, out Ped ballas,
        out Vehicle police, out Ped mpFibsec, out Ped mpFibsec2)
    {
        ballaorig2 = new Ped("G_M_Y_BALLAORIG_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 10,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0.002047441f, -0.001958636f, -0.7182238f),
            Orientation = new Quaternion(0f, 0f, 0.9777542f, -0.2097541f),
            Position = new Vector3(104.3498f, -1951.051f, 20.84373f)
        };
        ballaorig2.SetVariation(0, 0, 2);
        ballaorig2.SetVariation(3, 1, 0);
        ballaorig2.SetVariation(4, 0, 1);
        ballaorig2.SetVariation(8, 0, 0);
        // Newly spawned Peds will try to face north.
        ballaorig2.Tasks.ClearImmediately();
        ballaorig2.Heading = 204.2159f;

        ballaorig3 = new Ped("G_M_Y_BALLAORIG_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 20,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0.0004746652f, -0.001751979f, -0.2077992f),
            Orientation = new Quaternion(0f, 0f, 0.9912587f, -0.1319322f),
            Position = new Vector3(100.2635f, -1957.756f, 20.7979f)
        };
        ballaorig3.SetVariation(0, 1, 0);
        ballaorig3.SetVariation(3, 0, 2);
        ballaorig3.SetVariation(4, 0, 2);
        ballaorig3.SetVariation(8, 0, 1);
        // Newly spawned Peds will try to face north.
        ballaorig3.Tasks.ClearImmediately();
        ballaorig3.Heading = 195.1625f;

        ballasout = new Ped("G_M_Y_BALLASOUT_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 6,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0.002480251f, 0.001370145f, -0.2805587f),
            Orientation = new Quaternion(0f, 0f, -0.6635113f, 0.7481662f),
            Position = new Vector3(113.1443f, -1926.435f, 20.8231f)
        };
        ballasout.SetVariation(0, 0, 2);
        ballasout.SetVariation(2, 0, 0);
        ballasout.SetVariation(3, 0, 2);
        ballasout.SetVariation(4, 0, 0);
        ballasout.SetVariation(8, 0, 0);
        // Newly spawned Peds will try to face north.
        ballasout.Tasks.ClearImmediately();
        ballasout.Heading = 276.8636f;

        ballasout2 = new Ped("G_M_Y_BALLASOUT_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0.002750732f, 0.000680165f, 0.1327576f),
            Orientation = new Quaternion(0f, 0f, 0.7556524f, -0.6549729f),
            Position = new Vector3(120.7787f, -1940.82f, 20.71693f)
        };
        ballasout2.SetVariation(0, 1, 0);
        ballasout2.SetVariation(2, 0, 1);
        ballasout2.SetVariation(3, 1, 2);
        ballasout2.SetVariation(4, 1, 1);
        ballasout2.SetVariation(8, 0, 1);
        // Newly spawned Peds will try to face north.
        ballasout2.Tasks.ClearImmediately();
        ballasout2.Heading = 261.8352f;

        ballaorig = new Ped("G_M_Y_BALLAORIG_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 23,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0f, 1f),
            Position = new Vector3(105.3017f, -1934.918f, 20.80371f)
        };
        ballaorig.SetVariation(0, 1, 1);
        ballaorig.SetVariation(3, 0, 1);
        ballaorig.SetVariation(4, 0, 0);
        ballaorig.SetVariation(8, 0, 1);
        // Newly spawned Peds will try to face north.
        ballaorig.Tasks.ClearImmediately();
        ballaorig.Heading = 0f;

        ballaeast = new Ped("G_M_Y_BALLAEAST_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 17,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 67.62693f),
            Velocity = new Vector3(-1.149346f, 0.7425567f, 0.2160705f),
            Orientation = new Quaternion(0f, 0f, 0.4955916f, 0.8685558f),
            Position = new Vector3(106.7902f, -1937.902f, 20.7895f)
        };
        ballaeast.SetVariation(0, 0, 0);
        ballaeast.SetVariation(3, 0, 1);
        ballaeast.SetVariation(4, 0, 1);
        ballaeast.Inventory.Weapons.Add(WeaponHash.Pistol).Ammo = -1;
        ballaeast.Inventory.Weapons.Add(WeaponHash.Knife).Ammo = 0;
        // Newly spawned Peds will try to face north.
        ballaeast.Tasks.ClearImmediately();
        ballaeast.Heading = 59.41751f;

        ballaorig4 = new Ped("G_M_Y_BALLAORIG_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xbc066b98u),
            Money = 10,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.283294f, 0.9590331f),
            Position = new Vector3(103.7655f, -1938.615f, 20.79428f)
        };
        ballaorig4.SetVariation(0, 1, 2);
        ballaorig4.SetVariation(3, 0, 0);
        ballaorig4.SetVariation(4, 0, 2);
        ballaorig4.SetVariation(8, 0, 1);
        ballaorig4.Inventory.Weapons.Add(WeaponHash.Pistol).Ammo = -1;
        ballaorig4.Inventory.Weapons.Add(WeaponHash.Knife).Ammo = 0;
        // Newly spawned Peds will try to face north.
        ballaorig4.Tasks.ClearImmediately();
        ballaorig4.Heading = 32.91379f;

        ballas = new Ped("G_F_Y_BALLAS_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xe4df46d5u),
            Money = 21,
            RelationshipGroup = new RelationshipGroup("BADGANG"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(-2.545526E-10f, -1.231681E-10f, 30.11949f),
            Velocity = new Vector3(-1.080602f, -0.4208824f, 0.3328252f),
            Orientation = new Quaternion(1.627156E-10f, -4.610567E-09f, 0.8254462f, 0.5644808f),
            Position = new Vector3(107.8378f, -1934.318f, 20.73237f)
        };
        ballas.SetVariation(0, 1, 2);
        ballas.SetVariation(2, 0, 1);
        ballas.SetVariation(3, 0, 0);
        ballas.SetVariation(4, 0, 0);
        ballas.SetVariation(8, 2, 0);
        ballas.SetVariation(10, 1, 0);
        ballas.SetVariation(11, 2, 0);
        // Newly spawned Peds will try to face north.
        ballas.Tasks.ClearImmediately();
        ballas.Heading = 111.2675f;

        police = new Vehicle("POLICE3", Vector3.Zero, 0f)
        {
            DesiredVerticalFlightPhase = 8.267661E-44f,
            LicensePlateStyle = LicensePlateStyle.BlueOnWhite3,
            AlarmTimeLeft = TimeSpan.FromSeconds(0d),
            DriveForce = 0.3f,
            RimColor = Color.FromArgb(255, 65, 67, 71),
            PearlescentColor = Color.FromArgb(255, 8, 8, 8),
            SecondaryColor = Color.FromArgb(255, 0, 0, 0),
            PrimaryColor = Color.FromArgb(255, 0, 0, 0),
            ConvertibleRoofState = VehicleConvertibleRoofState.Raised,
            LockStatus = (VehicleLockStatus)5,
            DirtLevel = 4f,
            LicensePlate = "64HTK085",
            CollisionIgnoredEntity = null,
            IsGravityDisabled = false,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.09906425f, 0.9950811f),
            Position = new Vector3(20.23883f, -1773.036f, 28.7713f)
        };

        mpFibsec = new Ped("MP_M_FIBSEC_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            Money = 6,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, 0.999358f, 0.03582577f),
            Position = new Vector3(18.11886f, -1772.186f, 29.30607f)
        };
        mpFibsec.SetVariation(0, 0, 0);
        mpFibsec.SetVariation(2, 0, 0);
        mpFibsec.SetVariation(3, 0, 0);
        mpFibsec.SetVariation(4, 0, 0);
        mpFibsec.SetVariation(10, 0, 0);
        // Newly spawned Peds will try to face north.
        mpFibsec.Tasks.ClearImmediately();
        mpFibsec.Heading = 175.8938f;

        mpFibsec2 = new Ped("MP_M_FIBSEC_01", Vector3.Zero, 0f)
        {
            DecisionMaker = new DecisionMaker(0xa49e591cu),
            CanRagdoll = false,
            Money = 9,
            RelationshipGroup = new RelationshipGroup("COP"),
            CollisionIgnoredEntity = null,
            AngularVelocity = new Rotator(0f, 0f, 0f),
            Velocity = new Vector3(0f, 0f, 0f),
            Orientation = new Quaternion(0f, 0f, -0.5948692f, 0.8038225f),
            Position = new Vector3(18.8285f, -1773.461f, 29.31185f)
        };
        mpFibsec2.SetVariation(0, 1, 2);
        mpFibsec2.SetVariation(2, 0, 0);
        mpFibsec2.SetVariation(3, 1, 1);
        mpFibsec2.SetVariation(4, 0, 0);
        mpFibsec2.SetVariation(10, 1, 0);
        // Newly spawned Peds will try to face north.
        mpFibsec2.Tasks.ClearImmediately();
        mpFibsec2.Heading = 286.9934f;
    }
}