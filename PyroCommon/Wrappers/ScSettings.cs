using System;
using System.Reflection;
using System.Windows.Forms;

namespace PyroCommon.Wrappers;

internal static class ScSettings
{
    private static bool _animals;
    private static bool _robbery;
    private static bool _carAccident;
    private static bool _hotPursuit;
    private static bool _kidnapping;
    private static bool _truckCrash;
    private static bool _prisonTransport;
    private static bool _hitRun;
    private static bool _stolenCopVehicle;
    private static bool _stolenDumptruck;
    private static bool _ambulanceEscort;
    private static bool _aliens;
    private static bool _openCarry;
    private static bool _fire;
    private static bool _officerShootout;
    private static bool _weirdCar;
    private static bool _manhunt;
    private static bool _impersonator;
    private static bool _toiletPaperBandit;
    private static bool _blockingTraffic;
    private static bool _illegalParking;
    private static bool _knifeAttack;
    private static bool _deadBody;
    private static bool _fakeCall;
    private static bool _trespassing;
    private static bool _vandalizing;
    private static bool _injuredCop;
    private static bool _indecentExposure;
    private static bool _fight;
    private static bool _prisonBreak;
    private static bool _mafia1;
    private static bool _mafia2;
    private static bool _mafia3;
    private static bool _mafia4;
    private static bool _lostMc;
    private static bool _lsgtf;
    private static Keys _interact;
    private static Keys _endCall;
    private static string _emergencyNumber;

    private static readonly Type SettingsType = Assembly.Load("SuperCallouts").GetType("SuperCallouts.Settings");

    internal static void GetSettings()
    {
        if ( !Main.UsingSc ) return;
        _animals = (bool)SettingsType.GetField("Animals", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _robbery = (bool)SettingsType.GetField("Robbery", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _carAccident = (bool)SettingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _hotPursuit = (bool)SettingsType.GetField("HotPursuit", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _kidnapping = (bool)SettingsType.GetField("Kidnapping", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _truckCrash = (bool)SettingsType.GetField("TruckCrash", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _prisonTransport = (bool)SettingsType.GetField("PrisonTransport", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _hitRun = (bool)SettingsType.GetField("HitRun", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _stolenCopVehicle = (bool)SettingsType.GetField("StolenCopVehicle", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _stolenDumptruck = (bool)SettingsType.GetField("StolenDumptruck", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _ambulanceEscort = (bool)SettingsType.GetField("AmbulanceEscort", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _aliens = (bool)SettingsType.GetField("Aliens", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _openCarry = (bool)SettingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _fire = (bool)SettingsType.GetField("Fire", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _officerShootout = (bool)SettingsType.GetField("OfficerShootout", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _weirdCar = (bool)SettingsType.GetField("WeirdCar", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _manhunt = (bool)SettingsType.GetField("Manhunt", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _impersonator = (bool)SettingsType.GetField("Impersonator", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _toiletPaperBandit = (bool)SettingsType.GetField("ToiletPaperBandit", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _blockingTraffic = (bool)SettingsType.GetField("BlockingTraffic", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _illegalParking = (bool)SettingsType.GetField("IllegalParking", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _knifeAttack = (bool)SettingsType.GetField("KnifeAttack", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _deadBody = (bool)SettingsType.GetField("DeadBody", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _fakeCall = (bool)SettingsType.GetField("FakeCall", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _trespassing = (bool)SettingsType.GetField("Trespassing", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _vandalizing = (bool)SettingsType.GetField("Vandalizing", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _injuredCop = (bool)SettingsType.GetField("InjuredCop", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _indecentExposure = (bool)SettingsType.GetField("IndecentExposure", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _fight = (bool)SettingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _prisonBreak = (bool)SettingsType.GetField("PrisonBreak", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _mafia1 = (bool)SettingsType.GetField("Mafia1", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _mafia2 = (bool)SettingsType.GetField("Mafia2", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _mafia3 = (bool)SettingsType.GetField("Mafia3", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _mafia4 = (bool)SettingsType.GetField("Mafia4", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _lostMc = (bool)SettingsType.GetField("LostMc", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _lsgtf = (bool)SettingsType.GetField("Lsgtf", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _interact = (Keys)SettingsType.GetField("Interact", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _endCall = (Keys)SettingsType.GetField("EndCall", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _emergencyNumber = (string)SettingsType.GetField("EmergencyNumber", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if ( !Main.UsingSc ) return;
        SettingsType.GetField("Animals", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _animals);
        SettingsType.GetField("Robbery", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _robbery);
        SettingsType.GetField("CarAccident", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _carAccident);
        SettingsType.GetField("HotPursuit", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _hotPursuit);
        SettingsType.GetField("Kidnapping", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _kidnapping);
        SettingsType.GetField("TruckCrash", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _truckCrash);
        SettingsType.GetField("PrisonTransport", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _prisonTransport);
        SettingsType.GetField("HitRun", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _hitRun);
        SettingsType.GetField("StolenCopVehicle", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _stolenCopVehicle);
        SettingsType.GetField("StolenDumptruck", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _stolenDumptruck);
        SettingsType.GetField("AmbulanceEscort", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _ambulanceEscort);
        SettingsType.GetField("Aliens", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _aliens);
        SettingsType.GetField("OpenCarry", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _openCarry);
        SettingsType.GetField("Fire", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _fire);
        SettingsType.GetField("OfficerShootout", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _officerShootout);
        SettingsType.GetField("WeirdCar", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _weirdCar);
        SettingsType.GetField("Manhunt", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _manhunt);
        SettingsType.GetField("Impersonator", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _impersonator);
        SettingsType.GetField("ToiletPaperBandit", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _toiletPaperBandit);
        SettingsType.GetField("BlockingTraffic", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _blockingTraffic);
        SettingsType.GetField("IllegalParking", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _illegalParking);
        SettingsType.GetField("KnifeAttack", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _knifeAttack);
        SettingsType.GetField("DeadBody", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _deadBody);
        SettingsType.GetField("FakeCall", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _fakeCall);
        SettingsType.GetField("Trespassing", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _trespassing);
        SettingsType.GetField("Vandalizing", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _vandalizing);
        SettingsType.GetField("InjuredCop", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _injuredCop);
        SettingsType.GetField("IndecentExposure", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _indecentExposure);
        SettingsType.GetField("Fight", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _fight);
        SettingsType.GetField("PrisonBreak", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _prisonBreak);
        SettingsType.GetField("Mafia1", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _mafia1);
        SettingsType.GetField("Mafia2", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _mafia2);
        SettingsType.GetField("Mafia3", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _mafia3);
        SettingsType.GetField("Mafia4", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _mafia4);
        SettingsType.GetField("LostMc", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _lostMc);
        SettingsType.GetField("Lsgtf", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _lsgtf);
        SettingsType.GetField("Interact", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _interact);
        SettingsType.GetField("EndCall", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _endCall);
        SettingsType.GetField("EmergencyNumber", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _emergencyNumber);
    }

    internal static void SaveSettings()
    {
        if ( !Main.UsingSc ) return;
        var saveSettingsMethod = SettingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}