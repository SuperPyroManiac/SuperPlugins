using System.Linq;
using System.Reflection;
using Rage;
using Task = System.Threading.Tasks.Task;

namespace PyroCommon.PyroFunctions;

internal static class Log
{
    public static void Error(string message, bool snd = true)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        if ( VersionChecker.OutdatedPyroPlugins.ContainsKey(asmName) ) snd = false;
        var fullMessage = $"{asmName}%{message}";
        Game.Console.Print($"{asmName}: There was a serious issue here! See https://dsc.PyrosFun.com for help.");
        Game.Console.Print("======================ERROR======================");
        Game.Console.Print(message);
        Game.Console.Print("======================ERROR======================");
        if ( snd ) Task.Run(() => PyroFunctions.ProcessMsg(fullMessage));
    }

    public static void Warning(string message)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Game.Console.Print($"{asmName}: There was a minor issue here! See https://dsc.PyrosFun.com for help.");
        Game.Console.Print("======================WARNING======================");
        Game.Console.Print(message);
        Game.Console.Print("======================WARNING======================");
    }

    public static void Info(string message)
    {
        var asmName = Assembly.GetCallingAssembly().FullName.Split(',').First();
        Game.Console.Print($"{asmName}: {message}");
    }
}