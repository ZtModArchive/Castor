using Castor.Models;

namespace Castor.Interfaces
{
    interface IFileService
    {
        CastorConfig NewConfig(string archiveName);
        void CreateConfigFile(CastorConfig config);
        void CreateGitignoreFile();
    }
}
