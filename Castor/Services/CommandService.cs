using Castor.Interfaces;
using Castor.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace Castor.Services
{
    class CommandService : ICommandService
    {
        private static IFileService _fileService;
        private static IInstallerService _installerService;
        private static IZippingService _zippingService;
        public CommandService(IFileService fileService, IInstallerService installerService, IZippingService zippingService)
        {
            _fileService = fileService;
            _installerService = installerService;
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
                    _zippingService.Zip(archive, castorConfig, directory);
                }
            }

            if (Array.Exists(args, element => element =="--ztroot"))
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

            CastorConfig newConfig = _fileService.NewConfig(archiveName);
            _fileService.CreateConfigFile(newConfig);

            if (!File.Exists(".gitignore"))
            {
                _fileService.CreateGitignoreFile();
            }
            else
            {
                Console.WriteLine("gitignore already exists");
            }

            Console.WriteLine("check out castor.json and configure it as you need. Happy coding! :)");
        }

        public void Install(string[] args)
        {
            if (args.Length > 1)
            {
                _installerService.InstallModule(args[1], true);
            }
            else
            {
                string castorConfigText = File.ReadAllText("castor.json");
                CastorConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

                if (castorConfig.DevDependencies.Count == 0)
                    Environment.Exit(1);

                foreach (var package in castorConfig.DevDependencies)
                {
                    _installerService.InstallModule(package);
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
            CastorConfig castorConfig = JsonSerializer.Deserialize<CastorConfig>(castorConfigText);

            string[] buildArgs = new string[1];
            buildArgs[0] = "--ztroot";
            Build(buildArgs);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine("preparing serve");
            Console.WriteLine("loading Tiny Test Map");
            string ZTprogram = $"\"{castorConfig.ZT2loc}\\zt.exe\"";
            string ZTarg = $"{baseDirectory}castor-serve-save.z2s";

            Console.WriteLine("watching Zoo Tycoon 2...");
            ConsoleCommand(ZTprogram, ZTarg);
        }

        public void Version()
        {
            Console.WriteLine("Castor v3.0.0-beta");
        }

        public void ConsoleCommand(string program, string arg)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = arg;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                using (StreamWriter writer = new StreamWriter("castorlog.txt"))
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine();
                        Console.WriteLine($"{line}");
                        writer.WriteLine($"{line}");
                    }
                }
                
                process.WaitForExit();
            }
        }

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now}] {e.Data}");
        }
    }
}
