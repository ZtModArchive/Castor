using Castor.Interfaces;
using Castor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace Castor.Services
{
    class CommandService : ICommandService
    {
        private static IZippingService _zippingService;
        public CommandService(IZippingService zippingService)
        {
            _zippingService = zippingService;
        }

        public void Build(string[] args)
        {
            if (!File.Exists("castor.json"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: could not build, castor.json file not found");
                Console.ResetColor();
                Environment.Exit(1);
                return;
            }

            string castorConfigText = File.ReadAllText("castor.json");
            CastorConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

            string archiveName = $"{castorConfig.ArchiveName}.zip";

            Console.WriteLine("Building Zoo Tycoon 2 mod...");
            File.Create(archiveName).Close();
            using (FileStream zipToOpen = new FileStream(archiveName, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (var folder in castorConfig.IncludeFolders)
                    {
                        if (!Directory.Exists(folder))
                        {
                            continue;
                        }

                        DirectoryInfo directory = new DirectoryInfo(folder);
                        _zippingService.Zip(archive, castorConfig, directory);
                    }
                }
            }
            if (castorConfig.Z2f || !Array.Exists(args, element => element == "--zip"))
            {
                File.Delete($"{castorConfig.ArchiveName}.z2f");
                File.Move(archiveName, $"{castorConfig.ArchiveName}.z2f");
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Build succeeded.");
            Console.ResetColor();
        }

        public void Help()
        {
            Console.WriteLine("build - build from castor.json file");
            Console.WriteLine("init - create new castor.json file");
            Console.WriteLine("help - display help message");
            Console.WriteLine("install - install packages");
            Console.WriteLine("version - display version");
        }

        public void Init(string[] args)
        {
            if (File.Exists("castor.json"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: castor.json already exists");
                Console.ResetColor();
                Environment.Exit(1);
            }

            string archiveName;

            if (args.Length > 1)
            {
                archiveName = args[1];
            }
            else
            {
                string[] directoryPath = Directory.GetCurrentDirectory().Split('\\');
                archiveName = directoryPath[directoryPath.Length - 1];
            }

            CastorConfig newConfig = new()
            {
                ArchiveName = archiveName,
                Version = "v0.0.1",
                License = "unspecified",
                Description = "This is a Zoo Tycoon 2 mod",
                Z2f = true,
                IncludeFolders = new List<string> {
                    "ai",
                    "awards",
                    "biomes",
                    "config",
                    "effects",
                    "entities",
                    "lang",
                    "locations",
                    "maps",
                    "materials",
                    "photochall",
                    "puzzles",
                    "scenario",
                    "scripts",
                    "shared",
                    "tourdata",
                    "ui",
                    "world",
                    "xpinfo"
                },
                ExcludeFolders = new List<string> {
                    "modules"
                },
                Dependencies = new List<string>
                {

                },
                DevDependencies = new List<string>
                {

                }
            };

            string newJson = JToken.Parse(JsonSerializer.Serialize(newConfig)).ToString();
            File.WriteAllText("castor.json", newJson);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("initialized a new castor project");
            Console.ResetColor();

            if (!File.Exists(".gitignore"))
            {
                Console.WriteLine("generating gitignore file");
                using (var gitignore = File.Create(".gitignore")){ }

                File.WriteAllText(
                    ".gitignore",
                    "# exclude file types\n*.zip\n*.z2f\n\n# exclude modules\nmodules/"
                );

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("created gitignore file");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("gitignore already exists");
            }

            Console.WriteLine("check out castor.json and configure it as you need. Happy coding! :)");
        }

        public void Install(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Version()
        {
            Console.WriteLine("Castor v3.0.0-beta");
        }
    }
}
