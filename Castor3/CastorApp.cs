using Castor3.Interfaces;
using System;

namespace Castor3
{
    internal class CastorApp
    {
        private static ICommandService _commandService;
        public CastorApp(ICommandService commandService)
        {
            _commandService = commandService;
        }

        internal void Run(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "build":
                    case "b":
                        Build();
                        break;
                    case "init":
                        Init();
                        break;
                    case "install":
                    case "i":
                        Install();
                        break;
                    case "version":
                    case "v":
                        Version();
                        break;
                    case "help":
                    case "h":
                    default:
                        Help();
                        break;
                }
            }
            else
            {
                Help();
            }
            Environment.Exit(0);
        }

        void Build()
        {

        }

        void Init()
        {

        }

        void Install()
        {

        }

        void Version()
        {

        }

        void Help()
        {
            _commandService.ShowHelp();
        }
    }
}