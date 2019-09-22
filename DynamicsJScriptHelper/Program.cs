namespace DynamicsJScriptHelper
{
    using DynamicsJScriptHelper.Closure;
    using DynamicsJScriptHelper.Common;
    using DynamicsJScriptHelper.Dynamics;
    using Settings;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static DynamicsJScriptHelper.Common.Logger;
    using static DynamicsJScriptHelper.FileIo.Io;
    using static Formus.Utils.Connectors.Dynamics.ServiceFactory;

    class Program
    {
        static void Main(string[] args)
        {
            var settings = new RuntimeSettings(args);
            Logger.Settings = settings;
            Log($"Starting JScript helper...", true);
            var timer = new StopWatch();
            timer.Start();

            var files = GetAllJsFiles();

            if(files.Count == 0)
            {
                Log("Found no JS files.", true);
                return;
            }

            if (settings.Compile && (settings.Save || settings.Publish))
            {
                var compiledFiles = new Dictionary<string, string>();

                var compileTimer = new StopWatch();
                compileTimer.Start();
                Log($"Compiling JScript...", false);
                Parallel.ForEach(files, file =>
                {
                    var compilation = new ClosureCompilation(file.Value);
                    compilation.Compile();
                    compiledFiles.Add(
                        file.Key,
                        compilation.ClosureResponse.compiledCode);
                });
                Log($"Compiled in {compileTimer.Stop()} seconds.", false);

                files = compiledFiles;
            }

            if(settings.Save && settings.Compile)
            {
                WriteAllCompiledFiles(files);
            }

            if (settings.Publish)
            {
                var connectionTimer = new StopWatch();
                connectionTimer.Start();
                Log($"Connecting to CRM...", false);
                var crmService = GetCrmClient(settings.Credentials);
                Log($"Connected in {connectionTimer.Stop()} seconds.", false);

                Log($"Connected to \"{settings.Credentials.Uri}\".", false);
                Log($"As user \"{settings.Credentials.User}\".", false);

                var updateTimer = new StopWatch();
                updateTimer.Start();
                Log("Updating web resources...", false);
                Parallel.ForEach(files, file =>
                {
                    var update = new WebResourceUpdate(
                        file.Key,
                        file.Value,
                        crmService);

                    update.UpdateWebResource();
                });
                Log($"Update performed in {updateTimer.Stop()} seconds.", false);

                var publishTimer = new StopWatch();
                publishTimer.Start();
                Log($"Publishing...", false);
                WebResourceUpdate.PublishCustomisations(crmService);
                Log($"Published in {publishTimer.Stop()} seconds.", false);
            }

            Log($"Finished helper in {timer.Stop()} seconds.", true);
            SaveLog();
        }
    }
}
