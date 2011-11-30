using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Pieces
{
    public class Rook:Piece
    {
        int queenSide, kingSide;
        int castlingMaskKing;
        int castlingMaskQueen;
        ulong[] ZKeys;
        public Rook(Side owner, IChessBoard board, int cell)
            :base(owner,board,cell)
        {
            if (owner == Side.White)
            {
                queenSide = Square.a1;
                kingSide = Square.h1;
                castlingMaskKing = (int)(CastlingAvail.KingSideWhite);
                castlingMaskQueen = (int)(CastlingAvail.QueenSideWhite);
                ZKeys = ZKeysWhite;
            }
            else
            {
                queenSide = Square.a8;
                kingSide = Square.h8;
                castlingMaskKing = (int)(CastlingAvail.KingSideBlack);
                castlingMaskQueen = (int)(CastlingAvail.QueenSideBlack);
                ZKeys = ZKeysBlack;
            }
        }

        public override ulong GetZKey(int square)
        {
            return ZKeys[square];
        }

        public override bool AttackSquare(int square)
        {
            int dir = StraightAttackLookup[Square.Ox88Dist(square, HomeSquare)];
            if (dir == 0)
                return false;
            return AttackPathFree(dir, square);
        }
        public override bool Diagonal
        {
            get { return false; }
        }
        public override PieceType Type
        {
            get { return PieceType.Rook; }
        }
        public override bool Straight
        {
            get { return true; }
        }
        protected override string GetPieceString()
        {
            if (Owner == Side.White)
                return "R";
            else
                return "r";
        }
        protected override int GetValue()
        {
            return 500;
        }

        protected override void OnMove(int move)
        {
            if (!MovePackHelper.HasCapture(move))
            {
                board.ZKey ^= ZKeyForCastling[(int)board.CastlingStatus];
                board.CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                board.ZKey ^= ZKeyForCastling[(int)board.CastlingStatus];
            }
        }

        protected override void OnUnMove(int move)
        {
            if (!MovePackHelper.HasCapture(move))
            {
                board.ZKey ^= ZKeyForCastling[(int)board.CastlingStatus];
                board.CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                board.ZKey ^= ZKeyForCastling[(int)board.CastlingStatus];
            }
        }
        protected override int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            int begin = start;
            for (int i = 0; i < rayLen; ++i)
            {
                int square = ray[i];
                int dir = StraightAttackLookup[Square.Ox88Dist(square, HomeSquare)];
                if ((PinStatus == PinStatus.None || PinCompatible(dir)) &&
                    AttackPathFree(dir, square) )
                {
                    int castlingMask = 0;
                    if (HomeSquare == kingSide)
                        castlingMask = MovePackHelper.GetCastlingMerge(castlingMaskKing & (int)board.CastlingStatus);
                    if (HomeSquare == queenSide)
                        castlingMask = MovePackHelper.GetCastlingMerge(castlingMaskQueen & (int)board.CastlingStatus);
                    moves[start++] = MovePackHelper.Pack(HomeSquare,square) | castlingMask;
                }
            }
            return start - begin;
        }
        protected override int OnGetCaptureMoves(int start, int[] moves, int square)
        {
            int dir = StraightAttackLookup[Square.Ox88Dist(square, HomeSquare)];
            if ((PinStatus == PinStatus.None || PinCompatible(dir)) &&
                AttackPathFree(dir, square) &&
                board.BoardArray[square] != null &&
                IsEnemy(square))
            {
                int castlingMask = 0;
                if (HomeSquare == kingSide)
                    castlingMask = MovePackHelper.GetCastlingMerge(castlingMaskKing & (int)board.CastlingStatus);
                if (HomeSquare == queenSide)
                    castlingMask = MovePackHelper.GetCastlingMerge(castlingMaskQueen & (int)board.CastlingStatus);
                moves[start++] = MovePackHelper.Pack(this, board.BoardArray[square]) | castlingMask;
                return 1;
            }
            
            return 0;
        }
        protected override int OnGetMoves(int start, int[] moves)
        {
            int begin = start;
            int sq;
            //NORTH
            int castlingMask=0;
            if (HomeSquare == kingSide)
                castlingMask = MovePackHelper.GetCastlingMerge(castlingMaskKing & (int)board.CastlingStatus);
            if (HomeSquare == queenSide)
                castlingMask =  MovePackHelper.GetCastlingMerge(castlingMaskQueen & (int)board.CastlingStatus);
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.NS) != 0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + NORTH))
                {
                    sq += NORTH;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq) | castlingMask;
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]) | castlingMask;
                        }
                        break;
                    }

                }
            }
            //WEST
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.WE) != 0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + WEST))
                {
                    sq += WEST;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq) | castlingMask;
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]) | castlingMask;
                        }
                        break;
                    }

                }
            }
            //SOUTH
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.NS) != 0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + SOUTH))
                {
                    sq += SOUTH;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq) | castlingMask;
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]) | castlingMask;
                        }
                        break;
                    }

                }
            }
            //EAST
            if (PinStatus == PinStatus.None || (PinStatus & PinStatus.WE) != 0)
            {
                sq = HomeSquare;
                while (Square.SquareValid(sq + EAST))
                {
                    sq += EAST;
                    if (board.BoardArray[sq] == null)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, sq) | castlingMask;
                    }
                    else
                    {
                        if (IsEnemy(sq))
                        {
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[sq]) | castlingMask;
                        }
                        break;
                    }

                }
            }
            return start - begin;
        }

        protected override int GetCapValue()
        {
            return 2;
        }
        static ulong[] ZKeysBlack = new ulong[128];
        static ulong[] ZKeysWhite = new ulong[128];
        static ulong[] ZKeyForCastling;
        static Rook()
        { 
            for( int rank=0;rank<8;rank++ )
                for (int file = 0; file < 8; file++)
                {
                    ZKeysWhite[rank<<4|file] = Random64[GetZKeyIndex(PieceType.Rook,rank,file,Side.White)];
                    ZKeysBlack[rank << 4 | file] = Random64[GetZKeyIndex(PieceType.Rook, rank, file, Side.Black)];
                }
            ZKeyForCastling = new ulong[16];
            int offset = 768;
            for (int i = 0; i < 16; ++i)
            {
                if (0 != (i & (int)CastlingAvail.KingSideBlack))
                {
                    ZKeyForCastling[i] ^= Piece.Random64[offset + 2];
                }
                if (0 != (i & (int)CastlingAvail.KingSideWhite))
                {
                    ZKeyForCastling[i] ^= Piece.Random64[offset + 0];
                }
                if (0 != (i & (int)CastlingAvail.QueenSideBlack))
                {
                    ZKeyForCastling[i] ^= Piece.Random64[offset + 3];
                }
                if (0 != (i & (int)CastlingAvail.QueenSideWhite))
                {
                    ZKeyForCastling[i] ^= Piece.Random64[offset + 1];
                }
            }
        }
    }
}
