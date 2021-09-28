using Castor.Interfaces;
using System;
using System.IO;
using System.IO.Compression;

namespace Castor.Services
{
    class ZippingService : IZippingService
    {
        public void Zip(ZipArchive archive, ICastorConfig config, DirectoryInfo directoryInfo)
        {
            string[] directoryPath = Directory.GetCurrentDirectory().Split('\\');
            string rootDirectory = directoryPath[^1];
            string formattedPath = directoryInfo.FullName.Split(rootDirectory)[1].Remove(0, 1);

            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                Console.WriteLine($"compressing {formattedPath}\\{file.Name}");
                if (config.Type == "module" || config.Type == "package")
                {
                    archive.CreateEntryFromFile(file.FullName, $"modules\\{config.RepoName}\\{config.Version.Replace('.', '-')}\\{formattedPath}\\{file.Name}");
                }
                archive.CreateEntryFromFile(file.FullName, $"{formattedPath}\\{file.Name}");
            }
            var directories = directoryInfo.GetDirectories();
            bool exclude;
            string subDirectoryPath;
            foreach (var directory in directories)
            {
                exclude = false;
                subDirectoryPath = directory.FullName.Split(rootDirectory)[1].Remove(0, 1);
                foreach (var excludedPath in config.ExcludeFolders)
                {
                    if (subDirectoryPath == excludedPath || subDirectoryPath == excludedPath.Replace('/', '\\'))
                        exclude = true;
                }

                if (!exclude)
                    Zip(archive, config, directory);
            }
        }
    }
}
