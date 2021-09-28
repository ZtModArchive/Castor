using System.Collections.Generic;

namespace Castor.Interfaces
{
    class ICastorConfig
    {
        public string ArchiveName { get; set; }
        public string RepoName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string License { get; set; }
        public string Description { get; set; }
        public bool Z2f { get; set; }
        public string ZT2loc { get; set; }
        public List<string> IncludeFolders { get; set; }
        public List<string> ExcludeFolders { get; set; }
        public List<string> Dependencies { get; set; }
        public List<string> DevDependencies { get; set; }
    }
}
