using Castor.Interfaces;
using Castor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.Json;

namespace Castor.Services
{
    class InstallerService : IInstallerService
    {
        public void InstallModule(string packageName, bool addToConfig = false)
        {
            if (addToConfig)
            {
                string castorConfigText = File.ReadAllText("castor.json");
                CastorConfig config = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

                if (!config.DevDependencies.Contains(packageName))
                {
                    config.DevDependencies.Add(packageName);
                    string newJson = JToken.Parse(JsonSerializer.Serialize(config)).ToString();
                    File.WriteAllText("castor.json", newJson);
                    Console.WriteLine($"added {packageName} to castor.json");
                }
            }

            using (var client = new WebClient())
            {
                Guid guid = Guid.NewGuid();
                Console.WriteLine($"generated temporary GUID {guid} for package {packageName}");
                string[] packageArgs = packageName.Split('/');
                Console.WriteLine($"downloading package {packageName}");
                client.DownloadFile($"https://github.com/{packageArgs[0]}/{packageArgs[1]}/archive/refs/tags/{packageArgs[2]}.zip", $"{guid}.zip");
                Console.WriteLine($"extracting {packageName} to folder {guid}");
                ZipFile.ExtractToDirectory($"{guid}.zip", $"{guid}");

                // remove the package's modules folder if the dev didn't exclude it
                if (Directory.Exists($"{guid}/modules"))
                    Directory.Delete($"{guid}/modules");

                Console.WriteLine($"installing package {packageName}");

                string[] subDirectories = Directory.GetDirectories($"{guid}");
                string firstSubDir = "";
                if (subDirectories.Length > 0)
                {
                    firstSubDir = subDirectories[0];
                }

                CopyFilesRecursively($"{guid}/{firstSubDir.Split('\\')[1]}", $"modules/{packageArgs[0]}/{packageArgs[1]}/{packageArgs[2].Replace('.', '-')}");

                Console.WriteLine($"preparing cleanup for {guid}");
                var directory = new DirectoryInfo($"{guid}") { Attributes = FileAttributes.Normal };
                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                Console.WriteLine($"cleaning temporary files: {guid}");
                Directory.Delete($"{guid}", true);
                File.Delete($"{guid}.zip");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"installed package {packageName}");
                Console.ResetColor();
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
