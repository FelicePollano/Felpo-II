using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Interfaces;
using System.Reflection;
using FelpoII.Pieces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FelpoII
{
    public class OX88Chessboard:IChessBoard
    {
        public OX88Chessboard()
            :this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            
        }

        static OX88Chessboard()
        {
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
            ZKeyForEnPassant = new ulong[8];
            for (int i = 0; i < 8; ++i)
            {
                ZKeyForEnPassant[i] = Piece.Random64[772 + i];
            }
        }
        static ulong ZKeyWhiteToMove = Piece.Random64[780];

        static ulong[] ZKeyForCastling;
        static ulong[] ZKeyForEnPassant;


        public ulong CalcZKey()
        {
            ulong ZKey = 0;
            foreach (Piece p in whitePieceList)
            {
                if (p.HomeSquare != 0x88)
                    ZKey ^= p.ZKey;
            }
            foreach (Piece p in blackPieceList)
            {
                if (p.HomeSquare != 0x88)
                    ZKey ^= p.ZKey;
            }
            if( 0x88 != enPassant )
                ZKey ^= ZKeyForEnPassant[Square.Col(enPassant)-1];

            ZKey ^= ZKeyForCastling[(int)CastlingStatus];
            
            if (ToMove == Side.White)
                ZKey ^= ZKeyWhiteToMove;
            return ZKey;
        }
        
        
        public void Dispose()
        {
            HashTable.Drop();
        }

        
       
        const int MaskForHash = 0x7FFFFF;
        public int AllocateHashTable()
        {
            return HashTable.Initialize(MaskForHash);
        }

        public OX88Chessboard(string fen)
        {
            whiteKing = blackKing = null;
            enPassant = Square.Invalid;
            epStack = new Stack<int>();
            halfMovesStack = new Stack<int>();
            movesCount = 0;
            SetBoard(fen);
        }
        public int CheckRayLength { get; internal set; }
        public int[] _checkRay = new int[6];
        public int[] CheckRay
        {
            get 
            {
                return _checkRay;
            }
        }
        public bool InAttack(int square, Side owner)
        {
            if (owner == Side.White)
            {
                foreach (IPiece p in blackPieceList)
                {
                    if (p!= null &&  Square.SquareValid(p.HomeSquare)  && p.AttackSquare(square))
                        return true;
                }
            }
            else
            {
                foreach (IPiece p in whitePieceList)
                {
                    if (p != null && Square.SquareValid(p.HomeSquare)  && p.AttackSquare(square))
                        return true;
                }
            }
            return false;
        }
       
        public void SetEnPassant(int square)
        {
            enPassant=square;
        }
        Stack<int> epStack;
        bool epTouched;
        public void PlaceEnpassant(int square)
        {
            epTouched = true;
            epStack.Push(enPassant);
            if (enPassant != 0x88)
                ZKey ^= ZKeyForEnPassant[Square.Col(enPassant) - 1];
            enPassant=square;
            if (enPassant != 0x88)
                ZKey ^= ZKeyForEnPassant[Square.Col(square)-1];
        }
        public void UnplaceEnPassant()
        {
            epTouched = true;
            if (enPassant != 0x88)
                ZKey ^= ZKeyForEnPassant[Square.Col(enPassant) - 1];
            enPassant = epStack.Pop();
            if (enPassant != 0x88)
                ZKey ^= ZKeyForEnPassant[Square.Col(enPassant) - 1];
        }
        public IPiece[] BoardArray
        {
            get
            {
                return board;
            }
        }
        private int GenerateAllMoves(int start, int[] moves)
        {
            int begin = start;
            if (ToMove == Side.White)
            {
                foreach (IPiece p in whitePieceList)
                {
                    if ((p.HomeSquare & 0x88) == 0)
                    {
                        start += p.GetMoves(start, moves);
                    }
                }
            }
            else
            {
                foreach (IPiece p in blackPieceList)
                {
                    if ((p.HomeSquare & 0x88) == 0)
                    {
                        start += p.GetMoves(start, moves);
                    }
                }
            }
            return start - begin;
        }
        #region IChessBoard Members

        public int GetMoves(int start, int[] moves)
        {
            if (ToMove == Side.White)
            {
                if (whiteKing.CheckCount == 0)
                {
                    return GenerateAllMoves(start, moves);
                }
                else if (whiteKing.CheckCount == 1)
                {
                    return GenerateEvasions(start,moves);
                }
                else if (whiteKing.CheckCount >= 2)
                {
                    return whiteKing.GetMoves(start, moves);
                }
            }
            else
            {
                if (blackKing.CheckCount == 0)
                {
                    return GenerateAllMoves(start, moves);
                }
                else if (blackKing.CheckCount == 1)
                {
                    return GenerateEvasions(start, moves);
                }
                else if (blackKing.CheckCount >= 2)
                {
                    return blackKing.GetMoves(start, moves);
                }
            }
            return 0;
        }

        private int GenerateEvasions(int start, int[] moves)
        {
            int begin = start;
            if (ToMove == Side.White)
            {
                foreach (IPiece p in whitePieceList)
                {
                    if ((p.HomeSquare & 0x88) == 0 && p.Type!=PieceType.King)
                    {
                        start += p.GetCaptureMoves(start, moves,whiteKing.Checker.HomeSquare);
                        start += p.GetBlockingMoves(start, moves, CheckRay, CheckRayLength);
                    }
                }

                start += whiteKing.GetMoves(start, moves);
            }
            else
            {
                foreach (IPiece p in blackPieceList)
                {
                    if ((p.HomeSquare & 0x88) == 0 && p.Type != PieceType.King )
                    {
                        start += p.GetCaptureMoves(start, moves, blackKing.Checker.HomeSquare);
                        start += p.GetBlockingMoves(start, moves, CheckRay, CheckRayLength);
                    }
                }
                start += blackKing.GetMoves(start, moves);
            }
            return start-begin;
        }

        public CastlingAvail CastlingStatus { get;  set; }
        public string SavePos()
        {
            StringBuilder sb = new StringBuilder();
            SaveParts(sb);
            sb.Append(" ");
            SaveToMove(sb);
            sb.Append(" ");
            SaveCastling(sb);
            sb.Append(" ");
            SaveEnPassant(sb);
            sb.Append(" ");
            sb.Append(halfMoves.ToString());
            sb.Append(" ");
            sb.Append(movesCount.ToString());
            return sb.ToString();
        }
        Stack<int> halfMovesStack = new Stack<int>();
        private void SaveEnPassant(StringBuilder sb)
        {
            sb.Append(FromCellToLiteral(EnPassant));
        }

        private string FromCellToLiteral(int cell)
        {
            if (cell == Square.Invalid)
                return "-";
            string files = "abcdefgh";
            int idx8x8 = FromCellTo8x8(cell);
            if (idx8x8 == -1)
                return "-";
            return files.Substring(idx8x8 % 8, 1) + ((idx8x8 / 8)+1).ToString();
        
        }
        private  int FromCellTo8x8(int cell)
        {
            if ( 0!=(cell & Square.Invalid))
                return -1;
            else
            {
                int idx = (int)cell;
                return (idx >> 4) * 8 + (idx & 0xF);
            }
        }
        private void SaveCastling(StringBuilder sb)
        {
            if (CastlingStatus == CastlingAvail.None)
                sb.Append("-");
            else
            {
                if (0 != (CastlingStatus & CastlingAvail.KingSideWhite))
                {
                    sb.Append("K");
                }
                if (0 != (CastlingStatus & CastlingAvail.QueenSideWhite))
                {
                    sb.Append("Q");
                }
                if (0 != (CastlingStatus & CastlingAvail.KingSideBlack))
                {
                    sb.Append("k");
                }
                if (0 != (CastlingStatus & CastlingAvail.QueenSideBlack))
                {
                    sb.Append("q");
                }
            }
        }

        private void SaveToMove(StringBuilder sb)
        {
            if (ToMove == Side.White)
                sb.Append("w");
            else
                sb.Append("b");
        }

        private void SaveParts(StringBuilder sb)
        {
            int emptyCount = 0;
            for (int i = 112; i >= 0; i -= 16)
            {
                for (int ii = i; ii < i + 9; ++ii)
                {
                    if (0 != (ii & 0x88))
                    {
                        if (emptyCount > 0)
                            sb.Append(emptyCount);
                        if (i > 0)
                            sb.Append("/");
                        emptyCount = 0;
                        break;
                    }
                    if (board[ii] == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            sb.Append(emptyCount);
                            emptyCount = 0;
                        }
                        sb.Append(board[ii].ToString());
                    }
                }
            }
        }


        int  halfMoves,movesCount;
        int whiteMaterialValue, blackMaterialValue;
        IPiece[] board;
        IPiece[] whitePieceList;
        IPiece[] blackPieceList;
        Stack<int> moveStack;
        Stack<IPiece> captured;
        public void SetBoard(string fen)
        {
            ZKey = 0;
            if (fen == null)
                throw new ArgumentNullException("fen");
            InitPieceList();
            ClearBoard();

            
            string[] fenParts = fen.Trim().Split(' ');
            
            whiteMaterialValue = 0;
            blackMaterialValue = 0;
            
            movesCount = 0;
            CastlingStatus = CastlingAvail.None;
            
            if (fenParts.Length != 6)
                throw new Exception("Invalid fen:" + fen);
            ParseParts(fenParts[0]);
            ParseToMove(fenParts[1]);
            ParseCastling(fenParts[2]);
            ParseEnPassant(fenParts[3]);
            
            if (false == int.TryParse(fenParts[4], out halfMoves))
                throw new Exception("Half moves count cannot be parsed:" + fenParts[4]);
       
            if (false == int.TryParse(fenParts[5], out movesCount))
                throw new Exception("Moves count cannot be parsed:" + fenParts[5]);
            if (movesCount < 1)
                throw new Exception("Moves count less than 1:" + fenParts[5]);
        }
        Stack<IPiece> promoted;
        Stack<IPiece> availQueensBlack;
        Stack<IPiece> availBishopsBlack;
        Stack<IPiece> availKnightsBlack;
        Stack<IPiece> availRooksBlack;
        Stack<IPiece> availQueensWhite;
        Stack<IPiece> availBishopsWhite;
        Stack<IPiece> availKnightsWhite;
        Stack<IPiece> availRooksWhite;
        private void ClearBoard()
        {
            board = new IPiece[128];
            moveStack = new Stack<int>();
            captured = new Stack<IPiece>();
            epStack = new Stack<int>();
            promoted = new Stack<IPiece>();
            availBishopsBlack = new Stack<IPiece>();
            availKnightsBlack = new Stack<IPiece>();
            availQueensBlack = new Stack<IPiece>();
            availRooksBlack = new Stack<IPiece>();
            availBishopsWhite = new Stack<IPiece>();
            availKnightsWhite = new Stack<IPiece>();
            availQueensWhite = new Stack<IPiece>();
            availRooksWhite = new Stack<IPiece>();
            for (int i = 0; i < 8; ++i)
            {
                availBishopsBlack.Push(new Bishop(Side.Black, this, Square.Invalid));
                availRooksBlack.Push(new Rook(Side.Black, this, Square.Invalid));
                availQueensBlack.Push(new Queen(Side.Black, this, Square.Invalid));
                availKnightsBlack.Push(new Knight(Side.Black, this, Square.Invalid));
                availBishopsWhite.Push(new Bishop(Side.White, this, Square.Invalid));
                availRooksWhite.Push(new Rook(Side.White, this, Square.Invalid));
                availQueensWhite.Push(new Queen(Side.White, this, Square.Invalid));
                availKnightsWhite.Push(new Knight(Side.White, this, Square.Invalid));
            }
        }

        private void InitPieceList()
        {
            whitePieceList = new IPiece[16];
            blackPieceList = new IPiece[16];
        }

        public void UndoMove()
        {
            UndoMove(moveStack.Pop());
        }
        public ulong ZKey { get; set; }
        public void Dump(System.IO.StringWriter stringWriter)
        {
            for( int rank=7;rank>=0;rank-- )
            {
                stringWriter.Write(string.Format("{0} ",rank+1));
                for (int col = 0; col < 8; ++col)
                {
                    stringWriter.Write(BoardArray[rank << 4 | col] == null ? "." : BoardArray[rank << 4 | col].ToString());
                }
                stringWriter.WriteLine();
            }
            stringWriter.WriteLine("  ABCDEFGH");
            stringWriter.WriteLine(string.Format("Zobrist Key:\t0x{0:X}", ZKey));
            stringWriter.WriteLine(string.Format("Fen:\t\t{0}", SavePos()));
        }

        public void Move(string p)
        {
            int[] probeMoves = new int[256];
            TestInCheck(ToMove);
            int count = GetMoves(0, probeMoves);
            int decoded = MovePackHelper.DecodeAlgebraic(p);
            for (int i = 0; i < count; ++i)
            {
                if (MovePackHelper.GetCleanedMove(probeMoves[i]) == decoded)
                {
                    Move(probeMoves[i]);
                    return;
                }
            }

            throw new Exception("Invalid move:" + p);
        }

        public event EventHandler DividePartialResult;

        public DivideResults Divide(int depth)
        {
            int[] divideMoves = new int[1000];
            bool inCheck = TestInCheck(ToMove);
            
            int nodes = GetMoves(0, divideMoves);
            ulong movescount = 0;
            for( int i=0;i<nodes;++i )
            {
                Move(divideMoves[i]);
                PerfResults res = Perft(Math.Max(0,depth-1),false);
                if (null != DividePartialResult)
                {
                    this.CurrentDivideMove = MovePackHelper.GetAlgebraicString(divideMoves[i]);
                    this.CurrentDivideNodeCount =(long) res.MovesCount;
                    movescount += res.MovesCount;

                    DividePartialResult(this, EventArgs.Empty);
                }
                UndoMove(divideMoves[i]);
            }
            return new DivideResults() { NodesCount=(ulong)nodes, MovesCount = movescount  };
        }
        int[] prftMoves;
       
        public PerfResults Perft(int depth,bool useHash)
        {
            if (useHash == false)
            {
                prftMoves = new int[1000];
                Stopwatch sw = new Stopwatch();
                PerfResults perft = new PerfResults();
                sw.Start();
                perft.MovesCount = InternalPerft(depth, 0);
                sw.Stop();
                perft.Elapsed = (ulong)sw.ElapsedMilliseconds;
                return perft;
            }
            else
            {
                prftMoves = new int[1000];
                Stopwatch sw = new Stopwatch();
                PerfResults perft = new PerfResults();
                sw.Start();
                perft.MovesCount = InternalPerftWithHash(depth, 0,perft);
                sw.Stop();
                perft.Elapsed = (ulong)sw.ElapsedMilliseconds;
                return perft;
            }
        }

        public IPiece GetChecker(Side owner)
        {
            if (owner == Side.White)
            {
                return whiteKing.Checker;
            }
            else
            {
                return blackKing.Checker;
            }
        }
        private ulong InternalPerftWithHash(int depth, int p,PerfResults res)
        {
            ulong count = 0;

            if (HashTable.ProbeMovesCount(ZKey, out count, depth))
            {
                res.HashHit++;
                return count;
            }

            bool inCheck = TestInCheck(ToMove);

            int moveCount = GetMoves(p, prftMoves);

            if (depth == 1)
            {
                HashTable.StoreMovesCount(ZKey, (ulong)moveCount, 1);
                return (ulong)moveCount;
            }
            if (depth == 0)
            {
                return 1;
            }

            for (int i = p; i < p + moveCount; ++i)
            {
                Side moving = ToMove;
                Move(prftMoves[i]);
                
                count += InternalPerftWithHash(depth - 1, p + moveCount,res);
                UndoMove();
                
            }
            HashTable.StoreMovesCount(ZKey, count, depth);
            return count;
        }

        private ulong InternalPerft(int depth, int p)
        {
            ulong count = 0;
            bool inCheck = TestInCheck(ToMove);
            int moveCount = GetMoves(p,prftMoves);
            if (depth == 1)
            {
                return (ulong)moveCount;
            }
            if (depth == 0)
            {
                return 1;
            }
            
            for (int i = p; i < p + moveCount; ++i)
            {
                Side moving = ToMove;
                Move(prftMoves[i]);
                count += InternalPerft(depth - 1, p + moveCount);
                UndoMove();
            }
            return count;
        }

       
        static int[] knightRose = new int[] { 18, 33, 31, 14, -18, -33, -31, -14 };
        King whiteKing, blackKing;
        private bool AttackByPinMode(IPiece p, PinStatus pin)
        {
            if (pin == PinStatus.NS || pin == PinStatus.WE)
            {
                return p.Straight;
            }
            if (pin == PinStatus.NWSE || pin == PinStatus.SWNE)
            {
                return p.Diagonal;
            }
            return false;
        }
        private bool AttackModeByDir(IPiece p, int dir)
        {
            if (dir == Piece.NORTH || dir == Piece.SOUTH || dir == Piece.WEST || dir == Piece.EAST)
            {
                return p.Straight;
            }
            if (dir == Piece.SW || dir == Piece.SE || dir == Piece.NE || dir == Piece.NW)
            {
                return p.Diagonal;
            }
            return false;
        }
        public bool CheckIfSafeEpCapture(Side owner,int capturerHome)
        {
            int block = 0;
            int dir = 0;
            Side enemy = owner == Side.White ? Side.Black : Side.White;
            int home = 0;
            if (owner == Side.Black)
            {
                block = enPassant + Piece.NORTH;
                dir = DirectionFromTo(blackKing.HomeSquare, block);
                home = blackKing.HomeSquare;
            }
            else
            {
                block = enPassant + Piece.SOUTH;
                dir = DirectionFromTo(whiteKing.HomeSquare, block);
                home = whiteKing.HomeSquare;
            }
            for (int i = home + dir; Square.SquareValid(i); i += dir)
            {
                if (i == block || i == capturerHome)
                    continue;
                if (i == enPassant)
                    return true;
                if (BoardArray[i] != null)
                {
                    if (BoardArray[i].Owner == enemy)
                    {
                        if (AttackModeByDir(BoardArray[i], dir))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                        return true;
                }
            }
            return true;
        }

        private int DirectionFromTo(int from, int to)
        {
            int rfrom = Square.Rank(from);
            int rto = Square.Rank(to);
            int cfrom = Square.Col(from);
            int cto = Square.Col(to);
            if (rto > rfrom)
            {
                if (cto - cfrom == 0)
                    return Piece.NORTH;
                if (cto < cfrom)
                    return Piece.NW;
                else
                    return Piece.NE;
            }
            else
            if (rto < rfrom)
            {
                if (cto - cfrom == 0)
                    return Piece.SOUTH;
                if (cto < cfrom)
                    return Piece.SW;
                else
                    return Piece.SE;
            }
            else
                if (rto == rfrom)
                {
                    if (cto - cfrom == 0)
                        return 0;
                    if (cto < cfrom)
                        return Piece.WEST;
                    else
                        return Piece.EAST;
                }
            return 0;
        }
        int[] checkRay = new int[7];
        private void SearchForCheckAndPin(int start,int dir,King k,PinStatus pin)
        {
            
            int checkP = 0;
            IPiece potentialPinned=null;
            for (int i = start + dir; (i & 0x88) == 0; i += dir)
            {
                checkRay[checkP++] = i;
                
                if (BoardArray[i] != null)
                {
                    if (BoardArray[i].Owner == k.Owner)
                    {
                        // friend piece
                        if (potentialPinned == null)
                            potentialPinned = BoardArray[i];
                        else
                            break;
                    }
                    else
                    {
                        //enemy
                        if (potentialPinned == null) // no shield...
                        {
                            if (AttackByPinMode(BoardArray[i], pin))
                            {
                                k.CheckCount++;
                                k.Checker = BoardArray[i];
                                CheckRayLength = --checkP;
                                k.PinStatus |= pin;
                                Array.Copy(checkRay, 0, _checkRay, 0, CheckRayLength);
                            }
                            
                        }
                        else
                        {
                            // the shield is pinned...
                            if (AttackByPinMode(BoardArray[i],pin))
                            {
                                potentialPinned.PinStatus |= pin;
                            }
                        }
                        break;
                    }
                }
            }
        }


        public bool TestInCheck(Side moving)
        {
            CheckRayLength = 0;
            King k;
            if (moving == Side.Black)
            {
                k = blackKing;
                foreach (IPiece p in blackPieceList)
                    p.PinStatus = PinStatus.None;
                if (blackKing.HomeSquare == Square.e8)
                {
                    blackKing.CastlingKingSideFree = BoardArray[Square.f8] == null && BoardArray[Square.g8] == null;
                    blackKing.CastlingQueenSideFree = BoardArray[Square.b8] == null && BoardArray[Square.c8] == null && BoardArray[Square.d8] == null;
                }
                
            }
            else
            {
                k = whiteKing;
                foreach (IPiece p in whitePieceList)
                        p.PinStatus = PinStatus.None;
                if (whiteKing.HomeSquare == Square.e1)
                {
                    whiteKing.CastlingKingSideFree = BoardArray[Square.f1] == null && BoardArray[Square.g1] == null;
                    whiteKing.CastlingQueenSideFree = BoardArray[Square.b1] == null && BoardArray[Square.c1] == null && BoardArray[Square.d1] == null;
                }
            }
            int start = k.HomeSquare;
            k.CheckCount=0;
            k.Checker = null;
            
            SearchForCheckAndPin(start, Piece.NORTH, k,PinStatus.NS);
            SearchForCheckAndPin(start, Piece.SOUTH, k, PinStatus.NS);
            SearchForCheckAndPin(start, Piece.WEST, k, PinStatus.WE);
            SearchForCheckAndPin(start, Piece.EAST, k, PinStatus.WE);
            SearchForCheckAndPin(start, Piece.NE, k, PinStatus.SWNE);
            SearchForCheckAndPin(start, Piece.SW, k, PinStatus.SWNE);
            SearchForCheckAndPin(start, Piece.NW, k, PinStatus.NWSE);
            SearchForCheckAndPin(start, Piece.SE, k, PinStatus.NWSE);
            // contact checks
            // with knights
            foreach (int i in knightRose)
            {
                if (((i + start) & 0x88) == 0)
                {
                    if (BoardArray[i + start] != null && BoardArray[i + start].Owner != moving && BoardArray[i + start].Type == PieceType.Knight)
                    {
                        k.CheckCount++;
                        k.Checker = BoardArray[i + start];
                        return true;
                    }
                }
            }
            if (moving == Side.White)
            {
                if (Square.SquareValid(start + Piece.NE))
                {
                    if (BoardArray[start + Piece.NE] != null && BoardArray[start + Piece.NE].Owner != moving && BoardArray[start + Piece.NE].Type == PieceType.Pawn)
                    {
                        k.CheckCount++;
                        k.Checker = BoardArray[start + Piece.NE];
                        return true;
                    }
                }
                if (Square.SquareValid(start + Piece.NW))
                {
                    if (BoardArray[start + Piece.NW] != null && BoardArray[start + Piece.NW].Owner != moving && BoardArray[start + Piece.NW].Type == PieceType.Pawn)
                    {
                        k.CheckCount++;
                        k.Checker = BoardArray[start + Piece.NW];
                        return true;
                    }
                }
            }
            else
            {
                if (Square.SquareValid(start + Piece.SE))
                {
                    if (BoardArray[start + Piece.SE] != null && BoardArray[start + Piece.SE].Owner != moving && BoardArray[start + Piece.SE].Type == PieceType.Pawn)
                    {
                        k.CheckCount++;
                        k.Checker = BoardArray[start + Piece.SE];
                        return true;
                    }
                }
                if (Square.SquareValid(start + Piece.SW))
                {
                    if (BoardArray[start + Piece.SW] != null && BoardArray[start + Piece.SW].Owner != moving && BoardArray[start + Piece.SW].Type == PieceType.Pawn)
                    {
                        k.CheckCount++;
                        k.Checker = BoardArray[start + Piece.SW];
                        return true;
                    }
                }
            }
            return k.CheckCount != 0;
        }

        public void Capture(IPiece p)
        {
            BoardArray[p.HomeSquare] = null;
            captured.Push(p);
            p.Capture();
        }

        public void UnCapture(int square)
        {
            IPiece resumed = captured.Pop();
            resumed.UnCapture(square);
            board[square] = resumed;
        }

        private void DoCastling(int move)
        {
            // queen side black castling
            if (MovePackHelper.GetEndSquare(move) == Square.c8)
            {
                ZKey ^= BoardArray[Square.a8].ZKey;
                BoardArray[Square.d8] = BoardArray[Square.a8];
                BoardArray[Square.a8] = null;
                BoardArray[Square.d8].ForceSquare(Square.d8);
                ZKey ^= BoardArray[Square.d8].ZKey;
            }
            else
                // king side black castling
                if (MovePackHelper.GetEndSquare(move) == Square.g8)
                {
                    ZKey ^= BoardArray[Square.h8].ZKey;
                    BoardArray[Square.f8] = BoardArray[Square.h8];
                    BoardArray[Square.h8] = null;
                    BoardArray[Square.f8].ForceSquare(Square.f8);
                    ZKey ^= BoardArray[Square.f8].ZKey;
                }
                else
                    // king side white castling
                    if (MovePackHelper.GetEndSquare(move) == Square.g1)
                    {
                        ZKey ^= BoardArray[Square.h1].ZKey;
                        BoardArray[Square.f1] = BoardArray[Square.h1];
                        BoardArray[Square.h1] = null;
                        BoardArray[Square.f1].ForceSquare(Square.f1);
                        ZKey ^= BoardArray[Square.f1].ZKey;
                    }
                    else
                        // queen side white castling
                        if (MovePackHelper.GetEndSquare(move) == Square.c1)
                        {
                            ZKey ^= BoardArray[Square.a1].ZKey;
                            BoardArray[Square.d1] = BoardArray[Square.a1];
                            BoardArray[Square.a1] = null;
                            BoardArray[Square.d1].ForceSquare(Square.d1);
                            ZKey ^= BoardArray[Square.d1].ZKey;
                        }
        }

        private void UndoCastling(int move)
        {
            // queen side black castling
            if (MovePackHelper.GetEndSquare(move) == Square.c8)
            {
                ZKey ^= BoardArray[Square.d8].ZKey;
                BoardArray[Square.a8] = BoardArray[Square.d8];
                BoardArray[Square.d8] = null;
                BoardArray[Square.a8].ForceSquare(Square.a8);
                ZKey ^= BoardArray[Square.a8].ZKey;
            }
            else
                // king side black castling
                if (MovePackHelper.GetEndSquare(move) == Square.g8)
                {
                    ZKey ^= BoardArray[Square.f8].ZKey;
                    BoardArray[Square.h8] = BoardArray[Square.f8];
                    BoardArray[Square.f8] = null;
                    BoardArray[Square.h8].ForceSquare(Square.h8);
                    ZKey ^= BoardArray[Square.h8].ZKey;
                }
                else
                    // king side white castling
                    if (MovePackHelper.GetEndSquare(move) == Square.g1)
                    {
                        ZKey ^= BoardArray[Square.f1].ZKey;
                        BoardArray[Square.h1] = BoardArray[Square.f1];
                        BoardArray[Square.f1] = null;
                        BoardArray[Square.h1].ForceSquare(Square.h1);
                        ZKey ^= BoardArray[Square.h1].ZKey;
                    }
                    else
                        // queen side white castling
                        if (MovePackHelper.GetEndSquare(move) == Square.c1)
                        {
                            ZKey ^= BoardArray[Square.d1].ZKey;
                            BoardArray[Square.a1] = BoardArray[Square.d1];
                            BoardArray[Square.d1] = null;
                            BoardArray[Square.a1].ForceSquare(Square.a1);
                            ZKey ^= BoardArray[Square.a1].ZKey;
                        }
        }
        
       
        public void Move(int move)
        {
            
            halfMovesStack.Push(halfMoves);
            IPiece p = board[MovePackHelper.GetStartSquare(move)];
            if (MovePackHelper.HasCapture(move))
            {
                captured.Push(board[MovePackHelper.GetEndSquare(move)]);
                board[MovePackHelper.GetEndSquare(move)].Capture();
                ZKey ^= ZKeyForCastling[(int) CastlingStatus];
                CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                ZKey ^= ZKeyForCastling[(int)CastlingStatus];
                halfMoves = 0;
            }
            else
            {
                if (p.Type != PieceType.Pawn)
                    halfMoves++;
            }
            p.Move(move);
            if (MovePackHelper.HasCastling(move))
            {
                DoCastling(move);
            }
            PromotionTo to = MovePackHelper.GetPromotion(move);
            if (to != PromotionTo.None)
            {
                DoPromotion(p, move,to);
            }
            moveStack.Push(move);
            if (!epTouched)
                PlaceEnpassant(Square.Invalid);
            epTouched = false;
            if (ToMove == Side.Black)
                movesCount++;
            Flip();
            Debug.Assert(ZKey == CalcZKey());
        }

        private void Flip()
        {
            if (ToMove == Side.White)
            {
                ToMove = Side.Black;
            }
            else
                ToMove = Side.White;
            ZKey ^= ZKeyWhiteToMove;
        }
        
        private void UndoMove(int move)
        {
            IPiece p = board[MovePackHelper.GetEndSquare(move)];
            p.UnMove(move);
            if (MovePackHelper.HasCapture(move))
            {
                IPiece resumed = captured.Pop();
                resumed.UnCapture(MovePackHelper.GetEndSquare(move));
                board[resumed.HomeSquare] = resumed;
                ZKey ^= ZKeyForCastling[(int)CastlingStatus];
                CastlingStatus ^= MovePackHelper.GetCastleMask(move);
                ZKey ^= ZKeyForCastling[(int)CastlingStatus];
                
            }
            if (MovePackHelper.HasCastling(move))
            {
                UndoCastling(move);
            }
            PromotionTo to = MovePackHelper.GetPromotion(move);
            if (to != PromotionTo.None)
            {
                UndoPromotion(p, move,to);
            }
            if (!epTouched)
                UnplaceEnPassant();
            epTouched = false;
            if (ToMove == Side.White)
                movesCount--;
            halfMoves = halfMovesStack.Pop();
            Flip();
            Debug.Assert(ZKey == CalcZKey());
        }

        private void DoPromotion(IPiece pawn, int move, PromotionTo to)
        {
            BoardArray[pawn.HomeSquare] = GetPromoted(to);
            ZKey ^= pawn.ZKey;
            ReplacePawn(pawn,BoardArray[pawn.HomeSquare]); 
        }

        private void ReplacePawn(IPiece pawn, IPiece iPiece)
        {
            if (ToMove == Side.White)
            {
                for (int i = 0; i < whitePieceList.Length; ++i)
                {
                    if (whitePieceList[i] == pawn)
                    {
                        iPiece.ForceSquare(pawn.HomeSquare);
                        ZKey ^= iPiece.ZKey;
                        whitePieceList[i] = iPiece;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < blackPieceList.Length; ++i)
                {
                    if (blackPieceList[i] == pawn)
                    {
                        iPiece.ForceSquare(pawn.HomeSquare);
                        ZKey ^= iPiece.ZKey;
                        blackPieceList[i] = iPiece;
                        break;
                    }
                }
            }

            promoted.Push(pawn);
        }

        private IPiece GetPromoted(PromotionTo to)
        {
            if (ToMove == Side.White)
            {
                switch (to)
                {
                    case PromotionTo.Bishop:
                        return this.availBishopsWhite.Pop();
                    case PromotionTo.Knight:
                        return this.availKnightsWhite.Pop();
                    case PromotionTo.Queen:
                        return this.availQueensWhite.Pop();
                    case PromotionTo.Rook:
                        return this.availRooksWhite.Pop();
                }
            }
            else
            {
                switch (to)
                {
                    case PromotionTo.Bishop:
                        return this.availBishopsBlack.Pop();
                    case PromotionTo.Knight:
                        return this.availKnightsBlack.Pop();
                    case PromotionTo.Queen:
                        return this.availQueensBlack.Pop();
                    case PromotionTo.Rook:
                        return this.availRooksBlack.Pop();
                }
            }
            throw new Exception("Internal error.");
        }
        private void UndoPromotion(IPiece piece, int move, PromotionTo to)
        {
            UngetPromoted(piece,to);
            ZKey ^= piece.ZKey;
            BoardArray[piece.HomeSquare] = promoted.Pop();
            UnReplacePawn(piece, BoardArray[piece.HomeSquare]);     
        }

        private void UnReplacePawn(IPiece piece, IPiece pawn)
        {
            if (ToMove == Side.Black)
            {
                for (int i = 0; i < whitePieceList.Length; ++i)
                {
                    if (whitePieceList[i] == piece)
                    {
                        whitePieceList[i] = pawn;
                        pawn.ForceSquare(piece.HomeSquare);
                        ZKey ^= pawn.ZKey;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < blackPieceList.Length; ++i)
                {
                    if (blackPieceList[i] == piece)
                    {
                        blackPieceList[i] = pawn;
                        pawn.ForceSquare(piece.HomeSquare);
                        ZKey ^= pawn.ZKey;
                        break;
                    }
                }
            }

        }

        private void UngetPromoted(IPiece piece, PromotionTo to)
        {
            if (ToMove == Side.Black)
            {
                switch (to)
                {
                    case PromotionTo.Bishop:
                        this.availBishopsWhite.Push(piece);
                        break;
                    case PromotionTo.Knight:
                        this.availKnightsWhite.Push(piece);
                        break;
                    case PromotionTo.Queen:
                         this.availQueensWhite.Push(piece);
                         break;
                    case PromotionTo.Rook:
                         this.availRooksWhite.Push(piece);
                         break;
                }
            }
            else
            {
                switch (to)
                {
                    case PromotionTo.Bishop:
                        this.availBishopsBlack.Push(piece);
                        break;
                    case PromotionTo.Knight:
                         this.availKnightsBlack.Push(piece);
                         break;
                    case PromotionTo.Queen:
                         this.availQueensBlack.Push(piece);
                         break;
                    case PromotionTo.Rook:
                         this.availRooksBlack.Push(piece);
                         break;
                }
            }
            
        }


        public PerfResults Perf(out PerfResults undoRedo, int p)
        {
            throw new NotImplementedException();
        }

        public string CurrentDivideMove
        {
            get;set;
           
        }

        public long CurrentDivideNodeCount
        {
            get;
            set;
        }

        public Side ToMove { get; internal set; }
        
        int enPassant;
        public int EnPassant
        {
            get { return enPassant; }
        }    

        #endregion
        #region fen parsing privates
        private void ParseEnPassant(string p)
        {
            if (p == "-")
                return;
            FieldInfo fi = typeof(Square).GetField(p);
            if (fi == null)
                throw new Exception("Invalid Fen cell en passant specification:" + p);
            enPassant=(int)fi.GetRawConstantValue();
            int col = Square.Col(enPassant);
            ZKey ^= ZKeyForEnPassant[col-1];
        }

        private void ParseCastling(string p)
        {
            foreach (char c in p)
            {
                switch (c)
                {
                    case 'k':
                        CastlingStatus |= CastlingAvail.KingSideBlack;
                        break;
                    case 'K':
                        CastlingStatus |= CastlingAvail.KingSideWhite;
                        break;
                    case 'q':
                        CastlingStatus |= CastlingAvail.QueenSideBlack;
                        break;
                    case 'Q':
                        CastlingStatus |= CastlingAvail.QueenSideWhite;
                        break;
                }
            }
            if( null != ZKeyForCastling )
                ZKey ^= ZKeyForCastling[(int)CastlingStatus];
        }

        private void ParseToMove(string p)
        {
            if (p == "w")
            {
                ToMove = Side.White;
                ZKey ^= ZKeyWhiteToMove;
            }
            else
                ToMove = Side.Black;
        }
        
        private void ParseParts(string parts)
        {
            int rank = 7;
            int file = 1;
            IPiece piece;
            List<IPiece> whiteList = new List<IPiece>();
            List<IPiece> blackList = new List<IPiece>();
            foreach (char c in parts)
            {
                if (char.IsLetter(c))
                {
                    Side side = char.IsLower(c) ? Side.Black : Side.White;
                    int partValue = 0;
                    char p = char.ToLower(c);
                    int f = file - 1;
                    IList<IPiece> current;
                    if (side == Side.Black)
                        current = blackList;
                    else
                        current = whiteList;
                    switch (p)
                    {
                        case 'p':
                            piece = new Pawn(side, this, rank * 16 + f);
                            board[piece.HomeSquare] = piece;
                            current.Add(piece);
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                        case 'n':
                            piece = new Knight(side, this, rank * 16 + f);
                            current.Add(piece);
                            board[piece.HomeSquare] = piece;
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                        case 'b':
                            piece = new Bishop(side, this, rank * 16 + f);
                            current.Add(piece);
                            board[piece.HomeSquare] = piece;
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                        case 'r':
                            piece = new Rook(side, this, rank * 16 + f);
                            current.Add(piece);
                            board[piece.HomeSquare] = piece;
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                        case 'q':
                            piece = new Queen(side, this, rank * 16 + f);
                            current.Add(piece);
                            board[piece.HomeSquare] = piece;
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                        case 'k':
                            if( side == Side.White )
                                piece = whiteKing = new King(side, this, rank * 16 + f);
                            else
                                piece = blackKing = new King(side, this, rank * 16 + f);
                            current.Add(piece);
                            board[piece.HomeSquare] = piece;
                            partValue += piece.Value;
                            ZKey ^= piece.ZKey;
                            break;
                    }
                    
                    if (side == Side.Black)
                    {
                        blackMaterialValue += partValue;
                    }
                    else
                    {
                        whiteMaterialValue += partValue;
                    }
                    file++;
                }
                if (char.IsDigit(c))
                    file += int.Parse(new string(c, 1));
                else
                    if (c == '/')
                    {
                        file = 1;
                        rank--;
                    }
            }
            whitePieceList = whiteList.ToArray();
            blackPieceList = blackList.ToArray();
        }
        #endregion

        
    }
}
