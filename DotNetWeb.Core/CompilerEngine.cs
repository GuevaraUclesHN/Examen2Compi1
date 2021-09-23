using System;
using System.Collections.Generic;
using System.Text;
using DotNetWeb.Core.Interfaces;

namespace DotNetWeb.Core
{
    public class CompilerEngine
    {
        private readonly IParser parser;

        public CompilerEngine(IParser parser)
        {
            this.parser = parser;
        }

        public void Run()
        {
            var intermediateCode = this.parser.Parse();
        }
    }
}