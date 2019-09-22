namespace DynamicsJScriptHelper.Common
{
    using DynamicsJScriptHelper.Settings;
    using System.Collections.Generic;
    using static System.Console;
    using static System.DateTime;

    internal class Logger
    {
        internal static RuntimeSettings Settings { get; set; }

        internal static List<string> Logged = new List<string>();

        internal static void Log(string message, bool basic)
        {
            Logged.Add($"{UtcNow.ToShortTimeString()}: {message}\n");

            if(!basic && Settings.LogLevel != LogLevel.Verbose)
            {
                return;
            }

            WriteLine($"{UtcNow.ToShortTimeString()}: {message}\n");
        }
    }
}
