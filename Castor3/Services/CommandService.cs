using Castor3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor3.Services
{
    class CommandService : ICommandService
    {
        public void ShowHelp()
        {
            Console.WriteLine("Help message");
        }
    }
}
