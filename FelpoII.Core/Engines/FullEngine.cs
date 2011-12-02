using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using System.Diagnostics;

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
        int totalMillisec = 0;
        RunClock clk;
        long[] kslot;
        protected override int DoSearch(IChessBoard board,TimeSpan timeAvail)
        {
            this.totalMillisec = (int)timeAvail.TotalMilliseconds;
            this.board = board;
            int bestMove = 0;
            int val = 0;
            int alpha;
            bool timeout=false;
            kslot = new long[200];
            using(clk = new RunClock())
            {
                var preOrder = GetPreOrderedMoves();
                for (int curDepth = 1; curDepth <= maxDepth; ++curDepth) // iterative deeping
                {
                    alpha = -INFINITE;
                    var moves = GetBestMoveFirst(bestMove, preOrder);
                    //var moves = GetMoves(bestMove);
                    foreach (var m in moves)
                    {
                        board.Move(m);
                        var cm = MovePackHelper.GetAlgebraicString(m);
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
        IEnumerable<int> GetBestMoveFirst(int bestMove,IEnumerable<int> allMoves)
        {
            if (bestMove != 0)
                yield return bestMove;
            foreach (var i in allMoves)
            {
                if(i!=bestMove)
                    yield return i;
            }
        }
        IEnumerable<int> GetPreOrderedMoves()
        {
            board.TestInCheck(board.ToMove);
            int[] moves = new int[200];
            int all = board.GetMoves(0, moves);
            List<KeyValuePair<int, int>> orderedList = new List<KeyValuePair<int, int>>();
            foreach(var move in moves.Take(all) )
            {
                orderedList.Add(new KeyValuePair<int,int>(move,-AlphaBeta(2,-INFINITE,INFINITE)));
            }
            return orderedList.OrderByDescending(k => k.Value).Select(k=>k.Key);
        }
        static readonly int maxQuiesceDepth = 2;
        int AlphaBeta(int depth, int alpha, int beta)
        {
            int score = 0;
            int ttMove=0;
            int bestMove = 0;
            TTType ttFlag = TTType.Alpha;

            if (clk.GetElapsedMilliseconds() > totalMillisec)
                return 0; // timeout

            if (depth == 0)
            {
                return Quiesce( alpha, beta,0);
            }
            else
            {
                if (ttable.Probe(board.ZKey,depth, alpha, beta, ref ttMove,ref score))
                    return score;
                bestMove = ttMove;
                var moves = GetMoves(bestMove,depth);
                
                if (moves.Count() == 0 )
                {
                    score =board.InCheck(board.ToMove)?-INFINITE:0;
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
                        ttable.Save(board.ZKey,depth, beta, TTType.Beta, bestMove);
                        if(0!=(MovePackHelper.Capture&move))
                            StoreKiller(depth, move);
                        return beta;
                    }
                }
                ttable.Save(board.ZKey,depth, alpha, ttFlag, bestMove);
                return alpha;
            }
        }

        private void StoreKiller(int depth, int move)
        {
            if ((kslot[depth] & 0xFFFFFFFF) == 0)
            {
                kslot[depth] |= (uint)move;
            }
            else
            {
                kslot[depth] |= ((long)(uint)move) << 32;
            }
        }


        int Quiesce(int alpha,int beta,int depth)
        {
            if (depth == maxQuiesceDepth)
                return StaticEval();
            var moves = GetMoves(0,0);
            IEnumerable<int> selected;
            if (board.GetCheckCount(board.ToMove)>0)
            {
                if (moves.Count() == 0)
                    return -INFINITE;//mate
                //depth -= 1;
                selected = moves;
            }
            else
                selected = moves.Where(k => (k & MovePackHelper.Capture) != 0 ||  MovePackHelper.GetPromotion(k) != PromotionTo.None);
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
                if (alpha >= beta) // current player move is better than the other one better, no reason to search further
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

        IEnumerable<int> GetMoves(int best,int depth)
        {
            int[] moves = new int[200];
            board.TestInCheck(board.ToMove);
            var n = board.GetMoves(0, moves);
            int k1, k2;
            k1 = (int)((kslot[depth] & 0x7FFFFFFF00000000) >> 32);
            k2 = (int)(kslot[depth] & 0xFFFFFFFF);

            foreach (int i in moves.Take(n).OrderByDescending(m => GetWeight(m, best,k1,k2)))
            {
                yield return i;
            }
        }

        private int GetWeight(int m,int best,int k1,int k2)
        {
            int val =  (m & MovePackHelper.GoodCapture) != 0 ? 1000 : 0;
            if ((m & MovePackHelper.Capture) != 0 && (m & MovePackHelper.GoodCapture) == 0)
                val -= 1000;
            PromotionTo to;
            if ((to = MovePackHelper.GetPromotion(m)) != PromotionTo.None)
            {
                switch (to)
                {
                    case PromotionTo.Queen:
                        val += 1000;
                        break;
                    case PromotionTo.Knight:
                        val += 500;
                        break;
                    case PromotionTo.Bishop:
                        val -= 1000;
                        break;
                    case PromotionTo.Rook:
                        val -= 1000;
                        break;
                }
            }
            if (m == k1 || m == k2)
                val += 5000;

            if (m == best)
                val += 10000;
            return val;
        }
    }
}
