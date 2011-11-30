using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Search
{
    public class PlainAlphaBeta
    {
        public static readonly int MAXEVAL = int.MaxValue;
        public static readonly int MINEVAL = int.MinValue;

        public Func<IChessBoard, int> eval = (b)=>0; // null eval function
        public Func<IChessBoard,int,int, int> quiesce;
        public Func<IChessBoard, IEnumerable<int>> getMoves;
        public Action<IChessBoard, int, int, int> foundMove;
        public Action<IChessBoard, int, int, int> betaCutoff;

        public Func<int, ulong, int> probeBestMove;

        public Func<int> zeroMoveEval;
       
        public PlainAlphaBeta()
        {
            eval = (b) => 0; // null eval function
            quiesce = (b,alpha,beta) => eval(b);
            // when no more moves evaluate to loose.
            // this is true for chess, but can change for other games..
            zeroMoveEval = () => MINEVAL;
            foundMove =  delegate { };
            betaCutoff = delegate { };      
            probeBestMove=(depth,key)=>0;              
        }
        public int Search(IChessBoard board, int alpha, int beta,int depth)
        {
            if (depth == 0)
            {
                return quiesce(board, alpha, beta);
            }
            else
            {
                var moves = getMoves(board);
                if (moves.Count() == 0)
                {
                    return zeroMoveEval();
                }

                var pv = probeBestMove(depth, board.ZKey);
                if (pv != 0)
                {
                    board.Move(pv);
                    var score = -Search(board, -beta, -alpha, depth - 1);
                    board.UndoMove();
                    if (score > alpha)
                    {
                        foundMove(board, pv, depth, score);
                        alpha = score;
                    }
                    if (alpha >= beta) // current plaier move is better than the other one better, no reason to search further
                    {
                        //beta cutoff !!!
                        betaCutoff(board, pv, depth, score);
                        return alpha;
                    }
                }

                foreach (var move in moves)
                {
                    board.Move(move);
                    var score = -Search(board, -beta, -alpha, depth - 1);
                    board.UndoMove();
                    if (score > alpha)
                    {
                        foundMove(board, move, depth, score);
                        alpha = score;
                    }
                    if (alpha >= beta) // current plaier move is better than the other one better, no reason to search further
                    { 
                        //beta cutoff !!!
                        betaCutoff(board, move, depth, score);
                        break;
                    }
                }
                return alpha;
            }

        }
    }
}
