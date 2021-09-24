using Castor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.Services
{
    class CommandService : ICommandService
    {
        private static IZippingService _zippingService;
        public CommandService(IZippingService zippingService)
        {
            _zippingService = zippingService;
        }

        public void Build(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Help()
        {
            Console.WriteLine("sample help messgae");
        }

        public void Init(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Install(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Version()
        {
            Console.WriteLine("Castor v3.0.0-beta");
        }
    }
}
