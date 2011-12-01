using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using FelpoII.Core.Search;
using FelpoII.Core;

namespace FelpoII.Core.Engines
{
    public class SimpleVanillaEngine:AbstractEngine
    {
        int maxdepth;
        public SimpleVanillaEngine(int depth)
        {
            this.maxdepth = depth;
        }
        protected override void OnBreak()
        {
            throw new NotImplementedException();
        }

        protected override IChessBoard CreateBoard(string fen)
        {
            return new OX88Chessboard(fen);
        }

        protected override int DoSearch(IChessBoard board,TimeSpan timeAvail)
        {
            int currentBestMove = 0;
            int bestDepth = 0;
            Side side = board.ToMove;

            PlainAlphaBeta algo = new PlainAlphaBeta();

            algo.getMoves = (currentBoard) =>
            {
                int[] moves = new int[100];
                var n = currentBoard.GetMoves(0, moves);
                return moves.OrderByDescending(m => m & MovePackHelper.GoodCapture).Take(n);
            };
            algo.foundMove = (currentBoard, move, depth, score) =>
            {
                if (depth >= bestDepth && side == currentBoard.ToMove)
                {
                    currentBestMove = move;
                    bestDepth = depth;

                }
            };

            algo.eval = (currentBoard) =>
            {
                int val = 0;
                foreach (var v in currentBoard.BoardArray)
                {
                    if (null != v)
                    {
                        if (v.Owner == currentBoard.ToMove)
                            val += v.Value;
                        else
                            val -= v.Value;
                    }
                }
                return val;
            };

            int cutoffCount = 0;
            algo.betaCutoff = (currentBoard, move, depth, score) =>
            {
                cutoffCount += 1;
            };


            algo.Search(board, PlainAlphaBeta.MINEVAL, PlainAlphaBeta.MAXEVAL, maxdepth);
            return currentBestMove;
        }
    }
}
