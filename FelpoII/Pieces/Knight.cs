using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Interfaces;

namespace FelpoII.Pieces
{
    public class Knight:Piece
    {
        public Knight(Side owner, IChessBoard board, int cell)
            :base(owner,board,cell)
        {
            if (owner == Side.White)
                ZKeys = ZKeysWhite;
            else
                ZKeys = ZKeysBlack;
        }
        ulong[] ZKeys;
        public override ulong GetZKey(int square)
        {
            return ZKeys[square];
        }
        public override bool Diagonal
        {
            get { return false; }
        }
        public override bool Straight
        {
            get { return false; }
        }
        public override bool AttackSquare(int square)
        {
            return LeaperAttackLookup[Square.Ox88Dist(square, HomeSquare)];
        }
        protected override string GetPieceString()
        {
            if (Owner == Side.White)
                return "N";
            else
                return "n";
        }
        protected override int GetValue()
        {
            return 300;
        }

        protected override void OnMove(int move)
        {
           
        }

        protected override void OnUnMove(int move)
        {
            
        }
        public override PieceType Type
        {
            get { return PieceType.Knight; }
        }
        static int[] rose = new int[] { 18, 33, 31, 14, -18, -33, -31, -14 };
        protected override int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            int begin = start;
            if (PinStatus == PinStatus.None)
            {
                for (int i = 0; i < rayLen; ++i)
                {
                    int square = ray[i];
                    int rd = Square.Rank(square) - Square.Rank(HomeSquare);
                    int cd = Square.Col(square) - Square.Col(HomeSquare);
                    if (rd < 0) rd *= -1;
                    if (cd < 0) cd *= -1;
                    if ((cd == 1 && rd == 2) || (cd == 2 && rd == 1) )
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, square);
                    }
                }
            }
            return start-begin;
        }
        protected override int OnGetCaptureMoves(int start, int[] moves, int square)
        {
            if (PinStatus == PinStatus.None)
            {
                int rd = Square.Rank(square) - Square.Rank(HomeSquare);
                int cd = Square.Col(square) - Square.Col(HomeSquare);
                if (rd < 0) rd *= -1;
                if (cd < 0) cd *= -1;
                if (((cd == 1 && rd == 2) || (cd == 2 && rd == 1)) && IsEnemy(square))
                {
                    moves[start++] = MovePackHelper.Pack(this, board.BoardArray[square]);
                    return 1;
                }
            }
            return 0;
        }
        protected override int OnGetMoves(int start, int[] moves)
        {
            int begin = start;
            if (PinStatus == PinStatus.None)
            {
                foreach (int dest in rose)
                {
                    if (Square.SquareValid(HomeSquare + dest))
                    {
                        if (board.BoardArray[HomeSquare + dest] == null)
                            moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + dest);
                        else
                            if (IsEnemy(HomeSquare + dest))
                                moves[start++] = MovePackHelper.Pack(this, board.BoardArray[HomeSquare + dest]);
                    }
                }
            }
            return start-begin;
        }

        protected override int GetCapValue()
        {
            return 1;
        }
        static ulong[] ZKeysBlack = new ulong[128];
        static ulong[] ZKeysWhite = new ulong[128];
        static Knight()
        { 
            for( int rank=0;rank<8;rank++ )
                for (int file = 0; file < 8; file++)
                {
                    ZKeysWhite[rank<<4|file] = Random64[GetZKeyIndex(PieceType.Knight,rank,file,Side.White)];
                    ZKeysBlack[rank << 4 | file] = Random64[GetZKeyIndex(PieceType.Knight, rank, file, Side.Black)];
                }
        }
    }
}
