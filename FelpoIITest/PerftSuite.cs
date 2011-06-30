using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FelpoII;

namespace FelpoIITest
{
    class PerftSuite
    {
        Stream suiteStream;
        public PerftSuite()
        {
            suiteStream = GetType().Assembly.GetManifestResourceStream("FelpoIITest.Perft.perftsuite.epd");
        }
        internal void Run()
        {
            using (StreamReader reader = new StreamReader(suiteStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        DoPerftTest(line);
                    }
                }
            }
        }

        private void DoPerftTest(string line)
        {
            string[] chunks = line.Split(';');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Testing position:" + chunks[0]);
            Console.ForegroundColor = ConsoleColor.White;
            OX88Chessboard board = new OX88Chessboard(chunks[0]);
            for (int i = 1; i < chunks.Length; ++i)
            {
                int level; ulong expected;
                string[] items = chunks[i].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                level = int.Parse(items[0].Substring(1));
                expected = ulong.Parse(items[1]);
                Console.Write("Performing perft {0}. Expected:{1}\t",level,expected);
                PerfResults res =  board.Perft(level,true);
                Console.ForegroundColor = res.MovesCount == expected ? ConsoleColor.DarkGreen : ConsoleColor.Red;
                Console.WriteLine("Found:{0}\t\t\t{1}",res.MovesCount,res.MovesCount==expected ? "SUCCESS!":"*ERROR*");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
