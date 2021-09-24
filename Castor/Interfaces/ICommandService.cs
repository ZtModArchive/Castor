﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.Interfaces
{
    interface ICommandService
    {
        void Build(string[] args);
        void Help();
        void Init(string[] args);
        void Install(string[] args);
        void Version();
    }
}
