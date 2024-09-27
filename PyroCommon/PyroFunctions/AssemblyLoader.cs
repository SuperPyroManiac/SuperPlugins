using System;
using System.Reflection;

namespace PyroCommon.PyroFunctions;

public static class AssemblyLoader
{
    static AssemblyLoader()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.Contains("YamlDotNet"))
            {
                var resourceName = "Namespace.Path.To.YamlDotNet.dll";
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                var assemblyData = new byte[stream!.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
            return null;
        };
    }

    public static void Load() { /* Invokes the static constructor */ }
}