using System.IO;
using System.IO.Compression;

namespace Castor.Interfaces
{
    interface IZippingService
    {
        public void Zip(ZipArchive archive, ICastorConfig config, DirectoryInfo directoryInfo);
    }
}
