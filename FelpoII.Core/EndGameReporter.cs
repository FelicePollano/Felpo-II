using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core
{
    public static class EndGameReporter
    {
        public static GameEnded ReportEndGame(IChessBoard board)
        {
            int[] Moves = new int[500];
            int cnt = board.GetMoves(0, Moves);
            if (cnt == 0)
            {
                if (board.TestInCheck(Side.Black))
                {
                    return GameEnded.WhiteMate;
                }
                if (board.TestInCheck(Side.White))
                {
                    return GameEnded.BlackMate;
                }
                return GameEnded.Draw;

            }
            return GameEnded.None;
        }

    }
}
