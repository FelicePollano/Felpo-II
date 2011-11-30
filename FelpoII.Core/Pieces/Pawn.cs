using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core.Pieces
{
    public class Pawn:Piece
    {
        int epSquare;
        int oneStep, twoStep;
        int diag1, diag2;
        int rankForEp;
        int rankForEpCapture;
        int lastRank;
        public Pawn(Side owner, IChessBoard board, int cell)
            :base(owner,board,cell)
        {
            if (owner == Side.White)
            {
                oneStep = 16;
                twoStep = 32;
                rankForEp = 2;
                rankForEpCapture = 5;
                diag1 = 17;
                diag2 = 15;
                lastRank = 8;
                ZKeys = ZKeysWhite;
            }
            else
            {
                oneStep = -16;
                twoStep = -32;
                rankForEp = 7;
                rankForEpCapture = 4;
                diag1 = -17;
                diag2 = -15;
                lastRank = 1;
                ZKeys = ZKeysBlack;
            }
            MovePackHelper.Pack(HomeSquare, HomeSquare + twoStep);
            epSquare = HomeSquare + oneStep;
        }
        ulong[] ZKeys;
        public override ulong GetZKey(int square)
        {
            return ZKeys[square];
        }
        public override PieceType Type
        {
            get { return PieceType.Pawn; }
        }

        public override bool Diagonal
        {
            get { return false; }
        }
        public  override bool Straight 
        {
            get { return false; }
        }

        public override bool AttackSquare(int square)
        {
            if (HomeSquare + diag1 == square || HomeSquare + diag2 == square)
                return true;
            return false;
        }
        protected override string GetPieceString()
        {
            if (Owner == Side.White)
                return "P";
            else
                return "p";
        }
        protected override int GetValue()
        {
            return 100;
        }

        protected override void OnMove(int move)
        {
            if (Square.Rank(MovePackHelper.GetStartSquare(move)) == rankForEp && MovePackHelper.GetEndSquare(move) - MovePackHelper.GetStartSquare(move) == twoStep)
            {
                board.PlaceEnpassant(epSquare);
            }
            if ((move & MovePackHelper.EpFlag) != 0)
            {
                board.Capture(board.BoardArray[MovePackHelper.GetEndSquare(move)-oneStep]);
            }
        }

        protected override void OnUnMove(int move)
        {
            //if (MovePackHelper.GetStartSquare(move)==epSquare && MovePackHelper.GetEndSquare(move)==epSquare+twoStep)
            if (Square.Rank(MovePackHelper.GetStartSquare(move)) == rankForEp && MovePackHelper.GetEndSquare(move) - MovePackHelper.GetStartSquare(move) == twoStep)
            {
                board.UnplaceEnPassant();
            }
            if ((move & MovePackHelper.EpFlag) != 0)
            {
                board.UnCapture(MovePackHelper.GetEndSquare(move) - oneStep);
            }
        }
        protected override int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            int begin = start;
            if ((PinStatus == PinStatus.None || (PinStatus & PinStatus.NS) != 0))
            {
                for (int i = 0; i < rayLen; ++i)
                {
                    int square = ray[i];
                    if (square - HomeSquare == oneStep)
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, square);
                        if (Square.Rank(square) == lastRank)
                        {
                            int prom = moves[start-1];
                            moves[start - 1] |= MovePackHelper.GetPromotionMerge(PromotionTo.Queen);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Bishop);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Rook);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Knight);
                        }
                    }
                    if (Square.Rank(HomeSquare) == rankForEp && board.BoardArray[HomeSquare + oneStep] == null && square - HomeSquare == twoStep)
                    {
                        moves[start] = MovePackHelper.Pack(HomeSquare, square);
                        MovePackHelper.GetCleanedMove(moves[start]);
                        start++;
                    }
                }
            }
            return start - begin;
        }
        protected override int OnGetCaptureMoves(int start, int[] moves, int square)
        {
            int begin = start;
            if ((square - HomeSquare == diag1 || square - HomeSquare == diag2) &&
                IsEnemy(square) &&PinCompatible(square-HomeSquare)
                )
            {
                moves[start++] = MovePackHelper.Pack(this, board.BoardArray[square]);
                
                if (Square.Rank(square) == lastRank)
                {
                    int prom = moves[start - 1];
                    moves[start - 1] |= MovePackHelper.GetPromotionMerge(PromotionTo.Queen);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Bishop);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Rook);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Knight);
                }
            }
            // ep.
            if (Square.Rank(HomeSquare) == rankForEpCapture )
            {
                if (HomeSquare + diag2 - oneStep == square ) // THE CAPTURED ep is the target square
                {
                    if (Square.SquareValid(HomeSquare + diag2) && HomeSquare + diag2 == board.EnPassant && PinCompatible(diag2) && board.CheckIfSafeEpCapture(Owner, HomeSquare))
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + diag2) | MovePackHelper.EpFlag;
                    }
                }
                if (HomeSquare + diag1 - oneStep == square) // THE CAPTURED ep is the target square
                {
                    if (Square.SquareValid(HomeSquare + diag1) && HomeSquare + diag1 == board.EnPassant && PinCompatible(diag1) && board.CheckIfSafeEpCapture(Owner, HomeSquare))
                    {
                        moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + diag1) | MovePackHelper.EpFlag;
                    }
                }
            }
            return start-begin;
        }
        protected override int OnGetMoves(int start, int[] moves)
        {
            int begin = start;
            if (Square.Rank(HomeSquare) != lastRank && board.BoardArray[HomeSquare + oneStep] == null && (PinStatus==PinStatus.None || (PinStatus&PinStatus.NS)!=0))
            {
                moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + oneStep);
                if (Square.Rank(HomeSquare + oneStep) == lastRank)
                {
                    int prom = moves[start - 1];
                    moves[start - 1] |= MovePackHelper.GetPromotionMerge(PromotionTo.Queen);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Bishop);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Rook);
                    moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Knight);
                }
                if (Square.Rank(HomeSquare) == rankForEp && board.BoardArray[HomeSquare + twoStep] == null)
                {
                    moves[start] = MovePackHelper.Pack(HomeSquare, HomeSquare + twoStep);
                    MovePackHelper.GetCleanedMove(moves[start]);
                    start++;
                }
            }
            if (Square.Rank(HomeSquare) != lastRank)
            {
                if (PinStatus == PinStatus.None || (PinStatus & PinStatus.NS) == 0)
                {
                    if (Square.SquareValid(HomeSquare + diag1) && IsEnemy(HomeSquare + diag1) && PinCompatible(diag1))
                    {
                        moves[start++] = MovePackHelper.Pack(this, board.BoardArray[HomeSquare + diag1]);
                        if (Square.Rank(HomeSquare + diag1) == lastRank)
                        {
                            int prom = moves[start - 1];
                            moves[start - 1] |= MovePackHelper.GetPromotionMerge(PromotionTo.Queen);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Bishop);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Rook);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Knight);
                        }
                        
                    }
                    if (Square.SquareValid(HomeSquare + diag2) && IsEnemy(HomeSquare + diag2) &&  PinCompatible(diag2))
                    {
                        moves[start++] = MovePackHelper.Pack(this, board.BoardArray[HomeSquare + diag2]);
                        if (Square.Rank(HomeSquare + diag2) == lastRank)
                        {
                            int prom = moves[start - 1];
                            moves[start - 1] |= MovePackHelper.GetPromotionMerge(PromotionTo.Queen);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Bishop);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Rook);
                            moves[start++] = prom | MovePackHelper.GetPromotionMerge(PromotionTo.Knight);
                        }
                    }
                    // ep.
                    if (Square.Rank(HomeSquare) == rankForEpCapture)
                    {
                        if (Square.SquareValid(HomeSquare + diag2) && HomeSquare + diag2 == board.EnPassant &&  PinCompatible(diag2) && board.CheckIfSafeEpCapture(Owner,HomeSquare))
                        {
                              moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + diag2) | MovePackHelper.EpFlag;
                        }
                        if (Square.SquareValid(HomeSquare + diag1) && HomeSquare + diag1 == board.EnPassant && PinCompatible(diag1) && board.CheckIfSafeEpCapture(Owner,HomeSquare))
                        {
                              moves[start++] = MovePackHelper.Pack(HomeSquare, HomeSquare + diag1) | MovePackHelper.EpFlag;
                        }
                    }

                }
            }

            return start-begin;
        }

        protected override int GetCapValue()
        {
            return 0;
        }


        static ulong[] ZKeysBlack = new ulong[128];
        static ulong[] ZKeysWhite = new ulong[128];
        static Pawn()
        { 
            for( int rank=0;rank<8;rank++ )
                for (int file = 0; file < 8; file++)
                {
                    ZKeysWhite[rank<<4|file] = Random64[GetZKeyIndex(PieceType.Pawn,rank,file,Side.White)];
                    ZKeysBlack[rank << 4 | file] = Random64[GetZKeyIndex(PieceType.Pawn, rank, file, Side.Black)];
                }
        }
    }
}
