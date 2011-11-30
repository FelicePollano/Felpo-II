using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using FelpoII.Core;

namespace FelpoII
{
    public class Program
    {
        static void Main(string[] args)
        {
            using(IChessBoard board = new OX88Chessboard())
            {
                board.AllocateHashTable();
                WinBoardAdapter adapter = new WinBoardAdapter(board, null);
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
