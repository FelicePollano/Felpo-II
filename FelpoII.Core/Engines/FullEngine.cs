using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Engines
{
    public class FullEngine:AbstractEngine
    {
        int maxDepth;
        ITranspositionTable ttable;
        public FullEngine(int maxdepth,ITranspositionTable tt)
        {
            this.maxDepth = maxdepth;
            ttable = tt;
        }
        protected override void OnBreak()
        {
            throw new NotImplementedException();
        }

        protected override IChessBoard CreateBoard(string fen)
        {
            return new OX88Chessboard(fen);
        }
        IChessBoard board;
        const int INFINITE = 10000000;
        protected override int DoSearch(IChessBoard board,TimeSpan timeAvail)
        {
            
            this.board = board;
            int bestMove = 0;
            int val = 0;
            int alpha;
            bool timeout=false;
            using(var clk = new RunClock())
            {
                for (int curDepth = 1; curDepth <= maxDepth; ++curDepth) // iterative deeping
                {
                    alpha = -INFINITE;
                    var moves =  GetMoves(bestMove);
                    
                    foreach (var m in moves)
                    {
                        board.Move(m);
                        if (m == bestMove || -AlphaBeta(curDepth - 1, -alpha - 1, -alpha) > alpha)
                            val = -AlphaBeta(curDepth - 1, -INFINITE, -alpha);
                        board.UndoMove();
                        if (val > alpha)
                        {
                            alpha = val;
                            bestMove = m;
                            ttable.Save(board.ZKey,curDepth,alpha, TTType.Alpha, m);
                        }
                        if (clk.GetElapsedMilliseconds() > timeAvail.TotalMilliseconds / .5)
                        {
                            timeout = true;
                            break;
                        }
                    }
                    if (timeout)
                        break;
                    ttable.Save(board.ZKey,curDepth, alpha, TTType.Alpha, bestMove); 
                }
            }
            return bestMove;
        }
        static readonly int maxQuiesceDepth = 2;
        int AlphaBeta(int depth, int alpha, int beta)
        {
            int score = 0;
            int ttMove=0;
            int bestMove = 0;
            TTType ttFlag = TTType.Alpha;
            if (depth == 0)
            {
                return Quiesce( alpha, beta,0);
            }
            else
            {
                if (ttable.Probe(board.ZKey,depth, alpha, beta, ref ttMove,ref score))
                    return score;
                bestMove = ttMove;
                var moves = GetMoves(bestMove);
                
                if (moves.Count() == 0 )
                {
                    score =board.GetCheckCount(board.ToMove)>0?-INFINITE:0;
                    ttable.Save(board.ZKey,depth, score, TTType.Exact, 0);
                    return score;
                }
                
                foreach (var move in moves)
                {
                    board.Move(move);
                    score = -AlphaBeta(  depth - 1,-beta, -alpha);
                    board.UndoMove();
                    if (score > alpha)
                    {
                        bestMove = move;
                        alpha = score;
                        ttFlag = TTType.Exact;
                    }
                    if (alpha >= beta) // current plaier move is better than the other one better, no reason to search further
                    {
                        //beta cutoff !!!
                        ttable.Save(board.ZKey,depth, alpha, TTType.Beta, bestMove);
                        return alpha;
                    }
                }
                ttable.Save(board.ZKey,depth, alpha, ttFlag, bestMove);
                return alpha;
            }
        }


        int Quiesce(int alpha,int beta,int depth)
        {
            if (depth == maxQuiesceDepth)
                return StaticEval();
            var moves = GetMoves(0);
            IEnumerable<int> selected;
            if (board.GetCheckCount(board.ToMove) > 0)
            {
                depth -= 1;
                selected = moves;
            }
            else
                selected = moves.Where(k => (k & MovePackHelper.Capture) != 0);
            if (selected.Count() == 0)
                return StaticEval();

            foreach (var move in selected)
            {
                board.Move(move);
                var score = -Quiesce( -beta, -alpha,depth+1);
                board.UndoMove();
                if (score > alpha)
                {
                    alpha = score;
                }
                if (alpha >= beta) // current plaier move is better than the other one better, no reason to search further
                {
                    //beta cutoff !!!
                    break;
                }
            }
            return alpha;
            
        }

        private int StaticEval()
        {
            int val = 0;


            foreach (var v in board.BoardArray)
            {
                if (null != v)
                {
                    if (v.Owner == board.ToMove)
                        val += v.Value;
                    else
                        val -= v.Value;
                }
            }


            return val;
        }

        IEnumerable<int> GetMoves(int best)
        {
            int[] moves = new int[200];
            var n = board.GetMoves(0, moves);
            foreach (int i in moves.Take(n).OrderByDescending(m => GetWeight(m, best)))
            {
                yield return i;
            }
            
        }

        private int GetWeight(int m,int best)
        {
            int val =  (m & MovePackHelper.GoodCapture) != 0 ? 1000 : 0;
            if (m == best)
                val += 10000;
            return val;
        }
    }
}
