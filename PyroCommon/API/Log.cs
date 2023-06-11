using System.Linq;
using System.Reflection;
using Rage;

namespace PyroCommon.API;

internal static class Log
{
    public static void Error(string message)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
        Game.Console.Print($"{asmName}: Error Report Start");
        Game.Console.Print("======================================================");
        Game.Console.Print(message);
        Game.Console.Print("======================================================");
        Game.Console.Print($"{asmName}: Error Report End");
    }
    
    public static void Warning(string message)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Game.Console.Print($"{asmName}: Warning: There was an issue here. See https://dsc.gg/ulss for help.");
        Game.Console.Print($"{asmName}: Warning Report Start");
        Game.Console.Print("======================================================");
        Game.Console.Print(message);
        Game.Console.Print("======================================================");
        Game.Console.Print($"{asmName}: Warning Report End");
    }

    public static void Info(string message)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Game.Console.Print($"{asmName}: {message}");
    }
}