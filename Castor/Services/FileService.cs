using Castor.Interfaces;
using Castor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Castor.Services
{
    class FileService : IFileService
    {
        public void CreateConfigFile(CastorConfig config)
        {
            string newJson = JToken.Parse(JsonSerializer.Serialize(config)).ToString();
            File.WriteAllText("castor.json", newJson);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("initialized a new castor project");
            Console.ResetColor();
        }

        public void CreateGitignoreFile()
        {
            Console.WriteLine("generating gitignore file");
            using (var gitignore = File.Create(".gitignore")) { }

            File.WriteAllText(
                ".gitignore",
                "# exclude file types\n*.zip\n*.z2f\n\n# exclude modules\nmodules/"
            );

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("created gitignore file");
            Console.ResetColor();
        }

        public CastorConfig NewConfig(string archiveName)
        {
            return new CastorConfig()
            {
                ArchiveName = archiveName,
                Version = "v0.0.1",
                Author = "",
                License = "",
                Description = "This is a Zoo Tycoon 2 mod",
                Z2f = true,
                ZT2loc = @"C:\Program Files (x86)\Microsoft Games\Zoo Tycoon 2",
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
        }
    }
}
