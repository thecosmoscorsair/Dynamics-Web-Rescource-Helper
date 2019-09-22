namespace DynamicsJScriptHelper.FileIo
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using static DynamicsJScriptHelper.Common.Logger;

    internal static class Io
    {
        internal static Dictionary<string, string> GetAllJsFiles()
        {
            var outputDictionary = new Dictionary<string, string>();

            var files = Directory.GetFiles(
                    Directory.GetCurrentDirectory(),
                    "*.js",
                    SearchOption.AllDirectories)
                .ToList();

            files.AddRange(Directory.GetFiles(
                    Directory.GetCurrentDirectory(),
                    "*.html",
                    SearchOption.AllDirectories)
                .ToList());

            foreach (var file in files)
            {
                outputDictionary.Add(file, File.ReadAllText(file));
            }

            return outputDictionary;
        }

        internal static void WriteAllCompiledFiles(Dictionary<string, string> files)
        {
            foreach(var file in files)
            {
                var fileName = file.Key
                    .Substring(file.Key.LastIndexOf("\\") + 1);
                fileName = fileName
                    .Substring(0, fileName.LastIndexOf("."));
                File.WriteAllText($"{fileName}_Compiled.js", file.Value);
            }
        }

        internal static void SaveLog()
        {
            File.WriteAllLines(
                "LastRun.txt",
                Logged.ToArray<string>());
        }
    }
}
