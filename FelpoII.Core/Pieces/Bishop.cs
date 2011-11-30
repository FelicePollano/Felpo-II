using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Pieces
{
    public class Bishop:Piece
    {
        
        public Bishop(Side owner, IChessBoard board, int cell)
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
            get { return true; }
        }
        public override bool Straight
        {
            get { return false; }
        }
        public override PieceType Type
        {
            get { return PieceType.Bishop; }
        }
        public override bool AttackSquare(int square)
        {
            int dir = DiagAttackLookup[Square.Ox88Dist(square, HomeSquare)];
            if (dir == 0)
                return false;
            return AttackPathFree(dir,square);
        }
        protected override string GetPieceString()
        {
            if (Owner == Side.White)
                return "B";
            else
                return "b";
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
        protected override int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            int begin = start;
            for (int i = 0; i < rayLen; ++i)
            {
                int square = ray[i];
                int dir = DiagAttackLookup[Square.Ox88Dist(square, HomeSquare)];
                if ((PinStatus == PinStatus.None || PinCompatible(dir)) &&
                    AttackPathFree(dir, square)  )
                {
                    moves[start++] = MovePackHelper.Pack(HomeSquare, square);
                }
            }
            return start-begin;
        }
        protected override int OnGetCaptureMoves(int start, int[] moves, int square)
        {
            int dir = DiagAttackLookup[Square.Ox88Dist(square, HomeSquare)];
            if ((PinStatus == PinStatus.None || PinCompatible(dir)) &&
                AttackPathFree(dir,square) &&
                board.BoardArray[square] != null &&
                IsEnemy(square))
            {
                moves[start++] = MovePackHelper.Pack(this, board.BoardArray[square]);
                return 1;
            }
            return 0;
        }

        
        protected override int OnGetMoves(int start, int[] moves)
        {
            int begin = start;
            int sq;
            //NE
           
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.SWNE)!=0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + NE))
                {
                    sq += NE;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq);
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]);
                        }
                        break;
                    }

                }
            }
            //NW
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.NWSE)!=0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + NW))
                {
                    sq += NW;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq);
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]);
                        }
                        break;
                    }

                }
            }
            //SW
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.SWNE)!=0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + SW))
                {
                    sq += SW;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq);
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]);
                        }
                        break;
                    }

                }
            }
            //SE
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.NWSE) != 0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + SE))
                {
                    sq += SE;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq);
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]);
                        }
                        break;
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
        static Bishop()
        { 
            for( int rank=0;rank<8;rank++ )
                for (int file = 0; file < 8; file++)
                {
                    ZKeysWhite[rank<<4|file] = Random64[GetZKeyIndex(PieceType.Bishop,rank,file,Side.White)];
                    ZKeysBlack[rank << 4 | file] = Random64[GetZKeyIndex(PieceType.Bishop, rank, file, Side.Black)];
                }
        }
    }
}
