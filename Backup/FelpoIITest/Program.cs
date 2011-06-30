using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FelpoIITest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing move generator...");
            new PerftSuite().Run();
        }
    }
}
