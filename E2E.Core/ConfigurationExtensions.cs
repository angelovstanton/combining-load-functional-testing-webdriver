using System;
using System.IO;
using System.Reflection;

namespace E2E.Core
{
    public static class ConfigurationExtensions
    {
        public static string NormalizeAppPath(this string appPath)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                return appPath;
            }
            else if (appPath.StartsWith("AssemblyFolder", StringComparison.Ordinal))
            {
                var executionFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                appPath = appPath.Replace("AssemblyFolder", executionFolder);
            }

            return appPath;
        }
    }
}
