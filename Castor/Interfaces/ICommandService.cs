using System.Diagnostics;

namespace Castor.Interfaces
{
    interface ICommandService
    {
        void Build(string[] args);
        void Help();
        void Init(string[] args);
        void Install(string[] args);
        void Version();
        void Serve(string[] args);
        Process ConsoleCommand(string arg);
    }
}
