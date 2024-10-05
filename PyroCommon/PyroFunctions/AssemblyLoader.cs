using System;
using System.Collections.Generic;
using System.Reflection;

namespace PyroCommon.PyroFunctions;

public static class AssemblyLoader
{
    private static readonly Dictionary<string, Assembly> LoadedAssemblies = new();

    static AssemblyLoader()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyName = new AssemblyName(args.Name).Name;
            var resourceName = assemblyName switch
            {
                "YamlDotNet" => "PyroCommon.Libs.YamlDotNet.dll",
                "RageNativeUI" => "PyroCommon.Libs.RageNativeUI.dll",
                _ => null
            };
            if (resourceName == null) return null;
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                var assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                var assembly = Assembly.Load(assemblyData);
                LoadedAssemblies[assemblyName] = assembly; // Strong reference to prevent unloading
                return assembly;
            }
            API.Log.Error($"Resource {resourceName} not found.");
            return null;
        };
    }
    public static void Load(){}
}