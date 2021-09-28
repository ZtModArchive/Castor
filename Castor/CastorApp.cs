using Castor.Interfaces;
using System;

namespace Castor
{
    internal class CastorApp
    {
        private static ICommandService _commandService;
        public CastorApp(ICommandService commandService)
        {
            _commandService = commandService;
        }

        internal static void Run(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "build":
                    case "b":
                        _commandService.Build(args);
                        break;
                    case "init":
                        _commandService.Init(args);
                        break;
                    case "install":
                    case "i":
                        _commandService.Install(args);
                        break;
                    case "serve":
                    case "s":
                        _commandService.Serve(args);
                        break;
                    case "version":
                    case "v":
                        _commandService.Version();
                        break;
                    case "help":
                    case "h":
                    default:
                        _commandService.Help();
                        break;
                }
            }
            else
            {
                _commandService.Help();
            }
            Environment.Exit(0);
        }
    }
}