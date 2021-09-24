using System.Collections.Generic;

namespace Castor.Interfaces
{
    class ICastorConfig
    {
        public string ArchiveName { get; set; }
        public bool Z2f { get; set; }
        public List<string> IncludeFolders { get; set; }
        public List<string> ExcludeFolders { get; set; }
        public List<string> DevDependencies { get; set; }
    }
}
