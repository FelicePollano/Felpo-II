using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using FelpoII.Core;
using FelpoII.Core.Engines;

namespace FelpoII
{
    public class Program
    {
        static void Main(string[] args)
        {
            using(IChessBoard board = new OX88Chessboard())
            using(var tt = new UnmanagedTranspositionTable())
            {
                WinBoardAdapter adapter = new WinBoardAdapter(board, new FullEngine(5,tt) , tt);
                adapter.MessageToWinboard += new EventHandler<EngineToWinboardEventArgs>(adapter_MessageToWinboard);
                string line;
                while(null != ( line = Console.ReadLine()) )
                {
                    adapter.Consume(line);
                }
            }
        }

        static void adapter_MessageToWinboard(object sender, EngineToWinboardEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
