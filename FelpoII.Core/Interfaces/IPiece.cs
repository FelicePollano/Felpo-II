using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Interfaces
{
    public interface IPiece:IComparable
    {
        IChessBoard Board { get; }
        int Value { get;  }
        int CapValue { get; }
        Side Owner { get; }
        int HomeSquare { get;  }
        void Move(int move);
        void UnMove(int move);
        int GetMoves(int start, int[] moves);
        int GetCaptureMoves(int start, int[] moves,int square);
        int GetBlockingMoves(int start, int[] moves, int[] ray,int rayLen);
        void Capture();
        void UnCapture(int square);
        bool AttackSquare(int square);
        bool Diagonal { get; }
        bool Straight { get; }
        PinStatus PinStatus { get; set; }
        PieceType Type { get; }
        void ForceSquare(int Square);
        ulong ZKey{get;}
    }
}
