using System;
using System.Reflection;

namespace SuperEvents.EventFunctions;

public static class AssemblyResolver
{
    public static void Register()
    {
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
        if ( !args.Name.Contains("RageNativeUI") ) return null;
        const string resourceName = "PyroCommon.Libs.RageNativeUI.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if ( stream == null ) return null;
        var assemblyData = new byte[stream.Length];
        stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
    }
}