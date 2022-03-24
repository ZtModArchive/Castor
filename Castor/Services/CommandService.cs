using Castor.Interfaces;
using Castoreum.Config.Models;
using Castoreum.Interface.Service.Compression;
using Castoreum.Interface.Service.Config;
using Castoreum.Interface.Service.Installation;
using Castoreum.Interface.Service.Watch;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace Castor.Services
{
    class CommandService : ICommandService
    {
        private static ICompressionManager _compressionManager;
        private static IConfigManager _configManager;
        private static IInstallationManager _installationManager;
        private static IProcessWatcher _processWatcher;
        public CommandService(
            ICompressionManager compressionManager,
            IConfigManager configManager,
            IInstallationManager installationManager,
            IProcessWatcher processWatcher
        )
        {
            _compressionManager = compressionManager;
            _configManager = configManager;
            _installationManager = installationManager;
            _processWatcher = processWatcher;
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
            IConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

            string archiveName = $"{castorConfig.ArchiveName}.zip";

            Console.WriteLine("Building Zoo Tycoon 2 mod...");
            File.Create(archiveName).Close();
            using (FileStream zipToOpen = new(archiveName, FileMode.Open))
            {
                using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);
                foreach (var folder in castorConfig.IncludeFolders)
                {
                    if (!Directory.Exists(folder))
                    {
                        continue;
                    }

                    DirectoryInfo directory = new(folder);
                    _compressionManager.BuildMod(archive, castorConfig, directory);
                }
            }

            if (Array.Exists(args, element => element == "--ztroot"))
            {
                File.Delete($"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.zip");
                File.Move(archiveName, $"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.zip");
                if (castorConfig.Z2f && !Array.Exists(args, element => element == "--zip"))
                {
                    File.Delete($"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.z2f");
                    File.Move($"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.zip", $"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.z2f");
                    File.Delete($"{castorConfig.ZT2loc}\\{castorConfig.ArchiveName}.zip");
                }
            }
            else
            {
                if (castorConfig.Z2f && !Array.Exists(args, element => element == "--zip"))
                {
                    File.Delete($"{castorConfig.ArchiveName}.z2f");
                    File.Move(archiveName, $"{castorConfig.ArchiveName}.z2f");
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Build succeeded.");
            Console.ResetColor();
        }

        public void Help()
        {
            Console.WriteLine("build - build from castor.json file");
            Console.WriteLine("serve - build and serve from castor.json file");
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
                archiveName = directoryPath[^1];
            }

            IConfig newConfig = _configManager.CreateConfigFile(archiveName);
            _configManager.PlaceConfigFile(newConfig, "castor.json");

            if (!File.Exists(".gitignore"))
            {
                _configManager.PlaceGitIgnore("");
            }
            else
            {
                Console.WriteLine("gitignore already exists");
            }

            Console.WriteLine("check out castor.json and configure it as you need. Happy coding! :)");
        }

        public void Install(string[] args)
        {
            string castorConfigText = File.ReadAllText("castor.json");
            IConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);
            if (args.Length > 1)
            {
                if (args.Length > 2)
                {
                    if (args[2] == "--dev")
                        castorConfig = _installationManager.InstallDevDependency(castorConfig, args[1]);
                }
                else
                {
                    castorConfig = _installationManager.InstallDependency(castorConfig, args[1]);
                }

                _configManager.PlaceConfigFile(castorConfig, "castor.json");
            }
            else
            {
                foreach (var package in castorConfig.Dependencies)
                {
                    Console.WriteLine($"installing package {package}");
                    _installationManager.InstallPackage(package);
                }

                foreach (var package in castorConfig.DevDependencies)
                {
                    Console.WriteLine($"installing package {package}");
                    _installationManager.InstallPackage(package);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("installed packages");
                Console.ResetColor();
            }
        }

        public void Serve(string[] args)
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
            IConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

            string[] buildArgs = new string[1];
            buildArgs[0] = "--ztroot";
            Build(buildArgs);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine("preparing serve");
            Console.WriteLine("loading Tiny Test Map");
            string ZTprogram = $"\"{castorConfig.ZT2loc}\\zt.exe\"";
            string ZTarg = $"{baseDirectory}castor-serve-save.z2s";

            Console.WriteLine("watching Zoo Tycoon 2...");
            _processWatcher.Watch(ZTprogram, ZTarg);
        }

        public void Version()
        {
            Console.WriteLine($"Castor v{Assembly.GetExecutingAssembly().GetName().Version}");
        }
    }
}
