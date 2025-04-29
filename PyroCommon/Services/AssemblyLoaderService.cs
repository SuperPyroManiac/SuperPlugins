using System;
using System.Collections.Generic;
using System.Reflection;
using PyroCommon.Utils;

namespace PyroCommon.Services;

public static class AssemblyLoaderService
{
    private static readonly Dictionary<string, Assembly> LoadedAssemblies = new();

    static AssemblyLoaderService()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
    }

    private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;
        var resourceName = assemblyName switch
        {
            "YamlDotNet" => "PyroCommon.Libs.YamlDotNet.dll",
            _ => null,
        };

        if (resourceName == null)
            return null!;

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            LogUtils.Error($"Resource {resourceName} not found.");
            return null!;
        }

        var assemblyData = new byte[stream.Length];
        _ = stream.Read(assemblyData, 0, assemblyData.Length);
        var assembly = Assembly.Load(assemblyData);
        LoadedAssemblies[assemblyName] = assembly; // Strong reference to prevent unloading
        return assembly;
    }

    public static void Load() { }
}
