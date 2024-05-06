namespace PyroCommon.API;

public static class Enums
{
    
    public enum ScAnimationsSet
    {
        Drunk,
        Injured
    }
    
    public enum DrunkState
    {
        Tipsy = 0,
        ModeratelyDrunk = 1,
        VeryDrunk = 2,
        ExtremelyDrunk = 4,
        Sloshed = 8
    }

    public enum Permits
    {
        Drivers,
        Guns,
        Hunting,
        Fishing
    }

    public enum PermitStatus
    {
        None,
        Revoked,
        Expired,
        Valid
    }
    
    public enum ResistanceAction
    {
        /// <summary>The ped will not resist.</summary>
        None,
        /// <summary>The ped will flee.</summary>
        Flee,
        /// <summary>The ped will attack.</summary>
        Attack,
        /// <summary>The ped will refuse to follow orders.</summary>
        Uncooperative,
    }
    
    
    //SEARCH ITEM ENUMS
    
    public enum ItemLocation
    {
        /// <summary>No specific location.</summary>
        /// <remarks>Mostly used for peds.</remarks>
        None = 0,
        /// <summary>Item is anywhere.</summary>
        /// <remarks>
        /// If a vehicle search item has this set as location it will be randomized.
        /// </remarks>
        Anywhere = 1,
        /// <summary>Item is around the driver seat.</summary>
        DriverSeat = 2,
        /// <summary>Item is around the passenger seat.</summary>
        PassengerSeat = 4,
        /// <summary>Item is around the back left seat.</summary>
        BackLeftSeat = 8,
        /// <summary>Item is around the back right seat.</summary>
        BackRightSeat = 16, // 0x00000010
        /// <summary>Item is in the trunk.</summary>
        Trunk = 32, // 0x00000020
    }
    
    public enum DrugType
    {
        /// <summary>No drug.</summary>
        None = 0,
        /// <summary>Opioid</summary>
        Morphine = 1,
        /// <summary>Opioid</summary>
        Heroin = 2,
        /// <summary>Opioid</summary>
        Codeine = 4,
        /// <summary>Opioid</summary>
        Oxycodone = 8,
        /// <summary>Opioid</summary>
        Hydrocodone = 16, // 0x00000010
        /// <summary>Opioid</summary>
        Fentanyl = 32, // 0x00000020
        /// <summary>Hallucinogen</summary>
        PCP = 64, // 0x00000040
        /// <summary>Hallucinogen</summary>
        LSD = 128, // 0x00000080
        /// <summary>Hallucinogen</summary>
        Mescaline = 256, // 0x00000100
        /// <summary>Hallucinogen</summary>
        Psilocybin = 512, // 0x00000200
        /// <summary>Hallucinogen</summary>
        Cannabis = 1024, // 0x00000400
        /// <summary>Amphetamine</summary>
        Adderall = 2048, // 0x00000800
        /// <summary>Amphetamine</summary>
        Concerta = 4096, // 0x00001000
        /// <summary>Amphetamine</summary>
        Ritalin = 8192, // 0x00002000
        /// <summary>Amphetamine</summary>
        Methamphetamine = 16384, // 0x00004000
        /// <summary>Amphetamine</summary>
        Vyvanse = 32768, // 0x00008000
        /// <summary>Amphetamine</summary>
        Cocaine = 65536, // 0x00010000
        /// <summary>Antipsychotics</summary>
        Risperdal = 131072, // 0x00020000
        /// <summary>Antipsychotics</summary>
        Seroquel = 262144, // 0x00040000
        /// <summary>Antipsychotics</summary>
        Abilify = 524288, // 0x00080000
        /// <summary>Antipsychotics</summary>
        Clozapine = 1048576, // 0x00100000
    }
}