#ifndef ICHESSBOARD_H
#define ICHESSBOARD_H

/*
    public interface IChessBoard:IDisposable
    {
        int AllocateHashTable();
        ulong CalcZKey();
        ulong ZKey { get; set; }
        int[] CheckRay { get; }
        int CheckRayLength { get; }
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

        bool InAttack(int square,Side owner);
        bool CheckIfSafeEpCapture(Side owner,int capturer);
        IPiece GetChecker(Side inMove);

        void Dump(System.IO.StringWriter stringWriter);

        void Move(string p);

        event EventHandler DividePartialResult;

        DivideResults Divide(int depth);

        PerfResults Perft(int depth,bool useHash);

        PerfResults Perf(out PerfResults results, int p);

        string CurrentDivideMove { get; set; }
        long CurrentDivideNodeCount { get; set; }

    }
*/

class IChessBoard
{
public:
	virtual int AllocateHashTable() = 0;
	virtual ulong CalcZKey()=0;
    virtual ulong GetZKey()=0;
	virtual int[] GetCheckRay() = 0;

};

#endif