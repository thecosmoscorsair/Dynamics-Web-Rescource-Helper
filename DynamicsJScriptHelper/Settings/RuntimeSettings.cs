namespace DynamicsJScriptHelper.Settings
{
    using DynamicsJScriptHelper.JsonModels;
    using Formus.Utils.Connectors.Dynamics;
    using Newtonsoft.Json;
    using System.IO;
    using System.Linq;

    internal class RuntimeSettings
    {
        internal bool Compile { get; private set; }

        internal bool Save { get; private set; }

        internal bool Publish { get; private set; }

        internal LogLevel LogLevel { get; private set; }
        
        internal DynamicsCredentials Credentials { get; private set; }

        internal RuntimeSettings(string[] args)
        {
            Compile = args.Contains("compile");

            Save = args.Contains("save");

            Publish = args.Contains("publish");

            LogLevel = args.Contains("basic")
                ? LogLevel.Basic
                : LogLevel.Verbose;

            if (Publish)
            {
                var fileContents = File.ReadAllText("credentials.json");

                var model = JsonConvert
                    .DeserializeObject<DynamicsOnlineCredentialsModel>(
                    fileContents);

                Credentials = new DynamicsCredentials(
                    model.Url,
                    model.Username,
                    model.Password,
                    "",
                    DeploymentType.Online);
            }
        }
    }

    internal enum LogLevel
    {
        Basic,
        Verbose
    }
}
