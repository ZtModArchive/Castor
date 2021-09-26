namespace Castor.Interfaces
{
    interface IInstallerService
    {
        void InstallModule(string packageName, bool addToConfig = false);
    }
}
