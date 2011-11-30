using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Pieces
{
    public class King:Piece
    {
        int castlingMask;
        int castlingKing;
        int castlingQueen;
        int[] kingSidePath;
        int[] queenSidePath;
        public int CheckCount { get; set; }
        public IPiece Checker { get; set; }
        public int StartSquare { get; set; }
        public override ulong GetZKey(int square)
        {
            return ZKeys[square];
        }
        ulong[] ZKeys;
        public King(Side owner, IChessBoard board,int cell)
            :base(owner,board,cell)
        {

            if (owner == Side.White)
            {
                castlingMask = (int)(CastlingAvail.KingSideWhite | CastlingAvail.QueenSideWhite);
                castlingKing = (int)(CastlingAvail.KingSideWhite);
                castlingQueen = (int)(CastlingAvail.QueenSideWhite);
                StartSquare = Square.e1;
                kingSidePath = new int[] { Square.f1, Square.g1 };
                queenSidePath = new int[] { Square.c1, Square.d1 };
                ZKeys = ZKeysWhite;
            }
            else
            {
                castlingMask = (int)(CastlingAvail.KingSideBlack | CastlingAvail.QueenSideBlack);
                castlingKing = (int)(CastlingAvail.KingSideBlack);
                castlingQueen = (int)(CastlingAvail.QueenSideBlack);
                StartSquare = Square.e8;
                kingSidePath = new int[] { Square.f8, Square.g8 };
                queenSidePath = new int[] { Square.c8, Square.d8 };
                ZKeys = ZKeysBlack;
            }
        }

        private bool KingSideUnattacked()
        {
            foreach (int sq in kingSidePath)
            {
                if (board.InAttack(sq,Owner))
                    return false;
            }
            return true;
        }
        private bool QueenSideUnattacked()
        {
            foreach (int sq in queenSidePath)
            {
                if (board.InAttack(sq, Owner))
                    return false;
            }
            return true;
        }

        public override PieceType Type
        {
            get { return PieceType.King; }
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
            return KingAttackLookup[Square.Ox88Dist(square,HomeSquare)];
        }
        protected override int GetValue()
        {
            return Piece.Infinite;
        }
         
        protected override void OnMove(int move)
        {
            if (!MovePackHelper.HasCapture(move))
            {
                board.ZKey ^= ZKeyForCastling[(int)(board.CastlingStatus)];
                board.CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                board.ZKey ^= ZKeyForCastling[(int)(board.CastlingStatus)];
            }
        }

        protected override void OnUnMove(int move)
        {
            if (!MovePackHelper.HasCapture(move))
            {
                board.ZKey ^= ZKeyForCastling[(int)(board.CastlingStatus)];
                board.CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                board.ZKey ^= ZKeyForCastling[(int)(board.CastlingStatus)];
            }
        }

        

        static int[] rose = new int[] { NORTH,SOUTH,EAST,WEST,NE,NW,SE,SW };
        protected override int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            return 0;
        }


        protected override int OnGetCaptureMoves(int start, int[] moves, int square)
        {
            int rd = Square.Rank(square) - Square.Rank(HomeSquare);
            int cd = Square.Col(square) - Square.Col(HomeSquare);
            if (rd == -1) rd *= -1;
            if (cd == -1) cd *= -1;
            if (rd <= 1 && cd <= 1 && IsEnemy(square))
            {
                int  castling = MovePackHelper.GetCastlingMerge(castlingMask & (int)board.CastlingStatus);
                if (!board.InAttack(square, Owner))
                {
                    moves[start++] = MovePackHelper.Pack(this, board.BoardArray[square]) | castling;
                    return 1;
                }
            }
            return 0;
        }
        public bool CastlingQueenSideFree { get; set; }
        public bool CastlingKingSideFree { get; set; }
        protected override int OnGetMoves(int start, int[] moves)
        {
            int castling = 0;
            
            castling = MovePackHelper.GetCastlingMerge(castlingMask & (int)board.CastlingStatus);
            int begin = start;
            foreach (int dest in rose)
            {
                if (Square.SquareValid(HomeSquare + dest) && (PinEscape(dest) || dest+HomeSquare==Checker.HomeSquare ) && !board.InAttack(HomeSquare + dest,Owner) )
                {
                    if (board.BoardArray[HomeSquare + dest] == null)
                        moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + dest)|castling;
                    else
                        if (IsEnemy(HomeSquare + dest))
                            moves[start++] = MovePackHelper.Pack(this, board.BoardArray[HomeSquare + dest]) | castling;
                }
            }

            if (CheckCount == 0 )
            {
                // castlings
                if (0 != ((int)board.CastlingStatus & castlingKing) && CastlingKingSideFree&&  KingSideUnattacked())
                {
                    moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + EAST+EAST) | castling| MovePackHelper.Castling;
                }
                if (0 != ((int)board.CastlingStatus & castlingQueen) && CastlingQueenSideFree && QueenSideUnattacked())
                {
                    moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + WEST + WEST) | castling | MovePackHelper.Castling;
                }
            }

            return start - begin;
        }

        private bool PinEscape(int dest)
        {
            if (PinStatus == PinStatus.None)
                return true;
            if (0 != (PinStatus & PinStatus.NS))
            {
                if (dest == NORTH || dest == SOUTH)
                    return false;
            }
            if (0 != (PinStatus & PinStatus.SWNE))
            {
                if (dest == SW || dest == NE)
                    return false;
            }
            if (0 != (PinStatus & PinStatus.NWSE))
            {
                if (dest == NW || dest == SE)
                    return false;
            }
            if (0 != (PinStatus & PinStatus.WE))
            {
                if (dest == WEST || dest == EAST)
                    return false;
            }
            return true;
        }
        protected override string GetPieceString()
        {
            if (Owner == Side.White)
                return "K";
            else
                return "k";
        }

        protected override int GetCapValue()
        {
            return 0;// invalid
        }

        static ulong[] ZKeysBlack = new ulong[128];
        static ulong[] ZKeysWhite = new ulong[128];

        static ulong[] ZKeyForCastling;

        static King()
        { 
            for( int rank=0;rank<8;rank++ )
                for (int file = 0; file < 8; file++)
                {
                    ZKeysWhite[rank<<4|file] = Random64[GetZKeyIndex(PieceType.King,rank,file,Side.White)];
                    ZKeysBlack[rank << 4 | file] = Random64[GetZKeyIndex(PieceType.King, rank, file, Side.Black)];
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
