using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSTEngine.Pieces
{
    public class EnPassant:IPiece
    {
        public bool AttackSquare(int square)
        {
            return false;
        }
        public  PieceType Type
        {
            get { return PieceType.Pawn; }
        }
        public PinStatus PinStatus { get; set; }
        public  bool Diagonal
        {
            get { return false; }
        }
        public  bool Straight
        {
            get { return false; }
        }
        public void Capture()
        {
            HomeSquare = Square.Invalid;
        }
        public void UnCapture(int square)
        {
            HomeSquare = square;
        }
        public EnPassant()
        {
            HomeSquare = TSTEngine.Square.Invalid;
        }
        #region IPiece Members

        public int Value
        {
            get { throw new NotImplementedException(); }
        }
        public void SetSquare(int Square)
        {
            this.HomeSquare = Square; 
        }
        public int CapValue
        {
            get { throw new NotImplementedException(); }
        }

        public Side Owner
        {
            get {
                if (HomeSquare >= 32 && HomeSquare <= 39)
                    return Side.White;
                if (HomeSquare >= 80 && HomeSquare <= 87)
                    return Side.White;
                throw new Exception("Internal error: enPassant on incompatible square");
            }
        }

        public int HomeSquare
        {
            get;
            internal set;
        }

        public void Move(int move)
        {
            
        }

        public void UnMove(int move)
        {
            throw new NotImplementedException();
        }

        public int GetMoves(int start, int[] moves)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
