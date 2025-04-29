namespace PyroCommon.Models;

public static class Enums
{
    public enum UpdateState
    {
        Failed,
        Update,
        Current,
    }

    public enum ScAnimationsSet
    {
        Drunk,
        Injured,
    }

    public enum DrunkState
    {
        Tipsy = 0,
        ModeratelyDrunk = 1,
        VeryDrunk = 2,
        ExtremelyDrunk = 4,
        Sloshed = 8,
    }

    public enum Permits
    {
        Drivers,
        Guns,
        Hunting,
        Fishing,
    }

    public enum PermitStatus
    {
        None,
        Revoked,
        Expired,
        Valid,
    }

    public enum MarkerType
    {
        Cone = 0,
        Cylinder = 1,
        Arrow = 2,
        ArrowFlat = 3,
        Flag = 4,
        RingFlag = 5,
        Ring = 6,
        Plane = 7,
        BikeLogo1 = 8,
        BikeLogo2 = 9,
        Num0 = 10,
        Num1 = 11,
        Num2 = 12,
        Num3 = 13,
        Num4 = 14,
        Num5 = 15,
        Num6 = 16,
        Num7 = 17,
        Num8 = 18,
        Num9 = 19,
        Chevron1 = 20,
        Chevron2 = 21,
        Chevron3 = 22,
        RingFlat = 23,
        Lap = 24,
        Halo = 25,
        HaloPoint = 26,
        HaloRotate = 27,
        Sphere = 28,
        Money = 29,
        Lines = 30,
        Beast = 31,
        QuestionMark = 32,
        TransformPlane = 33,
        TransformHelicopter = 34,
        TransformBoat = 35,
        TransformCar = 36,
        TransformBike = 37,
        TransformPushBike = 38,
        TransformTruck = 39,
        TransformParachute = 40,
        TransformThruster = 41,
        Warp = 42,
        Boxes = 43,
        PitLane = 44,
    }

    public enum ResistanceAction
    {
        None,
        Flee,
        Attack,
        Uncooperative,
    }

    public enum ItemLocation
    {
        None = 0,
        Anywhere = 1,
        DriverSeat = 2,
        PassengerSeat = 4,
        BackLeftSeat = 8,
        BackRightSeat = 16,
        Trunk = 32,
    }

    public enum DrugType
    {
        None = 0,
        Morphine = 1,
        Heroin = 2,
        Codeine = 4,
        Oxycodone = 8,
        Hydrocodone = 16,
        Fentanyl = 32,
        PCP = 64,
        LSD = 128,
        Mescaline = 256,
        Psilocybin = 512,
        Cannabis = 1024,
        Adderall = 2048,
        Concerta = 4096,
        Ritalin = 8192,
        Methamphetamine = 16384,
        Vyvanse = 32768,
        Cocaine = 65536,
        Risperdal = 131072,
        Seroquel = 262144,
        Abilify = 524288,
        Clozapine = 1048576,
    }

    public enum BackupType
    {
        Code2 = 0,
        Code3 = 1,
        Swat = 2,
        Noose = 3,
        Fire = 4,
        Medical = 5,
        Pursuit = 6,
    }
}
