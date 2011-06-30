using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MersenneTwister;
using System.Diagnostics;

namespace MersenneTwisterGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            RandomMT mt = new RandomMT(0x7449BBFF801FED0B);
            for (int i = 0; i < 781; ++i)
            {
                if (i % 4 == 0)
                {
                    Console.WriteLine();
                    Trace.WriteLine("");
                }
                
                Console.Write("0x{0:X}UL,",mt.RandomInt()|mt.RandomInt()<<32);
                Trace.Write(string.Format("0x{0:X}UL,", mt.RandomInt() | mt.RandomInt() << 32));



            }
          
        }
    }
}
