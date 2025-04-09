﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace PyroCommon.PyroFunctions;

public static class AssemblyLoader
{
    private static readonly Dictionary<string, Assembly> LoadedAssemblies = new();

    static AssemblyLoader()
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
            Log.Error($"Resource {resourceName} not found.");
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
