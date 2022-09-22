namespace Castor.Interfaces
{
    interface ICommandService
    {
        void Build(string[] args);
        void Init(string[] args);
        void Help();
        void Install(string[] args);
        void Serve(string[] args);
        void Version();
    }
}
