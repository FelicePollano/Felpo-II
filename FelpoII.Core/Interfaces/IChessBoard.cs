using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Pieces;


namespace FelpoII.Core.Interfaces
{
    public interface IChessBoard:IDisposable
    {
        void Flip();
        ulong CalcZKey();
        ulong ZKey { get; set; }
        int[] CheckRay { get; }
        int CheckRayLength { get; }
        int GetCheckCount(Side side);
        bool TestInCheck(Side side);
        IPiece[] BoardArray { get; }
        int GetMoves(int start, int[] moves);
        Side ToMove { get; }
        string SavePos();
        void PlaceEnpassant(int square);
        void UnplaceEnPassant();
        void SetEnPassant(int square);
        void SetBoard(string fen);
        CastlingAvail CastlingStatus { get; set; }
        void UndoMove();
        int EnPassant { get; }

        void Capture(IPiece p);
        void UnCapture(int square);
        bool InCheck(Side owner);
        bool InAttack(int square,Side owner);
        bool CheckIfSafeEpCapture(Side owner,int capturer);
        IPiece GetChecker(Side inMove);

        void Dump(System.IO.StringWriter stringWriter);

        void Move(string p);
        void Move(int move);

        event EventHandler DividePartialResult;

        DivideResults Divide(int depth);

        PerfResults Perft(int depth,bool useHash,ITranspositionTable hash);

        PerfResults Perf(out PerfResults results, int p);

        string CurrentDivideMove { get; set; }
        long CurrentDivideNodeCount { get; set; }

    }
}
