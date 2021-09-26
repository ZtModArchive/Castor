using Castor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.Interfaces
{
    interface IFileService
    {
        CastorConfig NewConfig(string archiveName);
        void CreateConfigFile(CastorConfig config);
        void CreateGitignoreFile();
    }
}
