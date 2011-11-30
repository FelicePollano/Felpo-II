using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core
{
    public static class  MovePackHelper
    {
        public const int Capture = 0x10000;
        public const int GoodCapture = 0x20000;
        public const int EpFlag = 0x2000000;
        public const int Castling = 0x4000000;

        
        public static bool HasCastling(int move)
        {
            return (move & Castling) != 0;
        }

        public static PromotionTo GetPromotion(int move)
        {
            return (PromotionTo)((move >> 27) & 0xF);
        }
        public static int GetPromotionMerge(PromotionTo promotion)
        { 
            return (int)promotion << 27;
        }

        public static int GetCastlingMerge(int mask)
        {
            return mask << 21;
        }
        public static int GetStartSquare(int move)
        {
            return move & 0xff;
        }
        public static int GetEndSquare(int move)
        {
            return (move >> 8) & 0xff;
        }
        public static bool HasCapture(int move)
        {
            return (move & Capture) != 0;
        }


        public static int Pack(int start, int dest)
        {
            return start | (dest << 8);
        }

        

        public static int Pack(IPiece moving, IPiece capt)
        {
            return moving.HomeSquare | (capt.HomeSquare << 8) | GetCaptureDescriber(moving,capt) | GetCaptureCastlingMask(capt.HomeSquare,moving.Board.CastlingStatus);
        }

        static int GetCaptureCastlingMask(int captureSquare,CastlingAvail status)
        {
            switch (captureSquare)
            {
                case Square.a1:
                    return MovePackHelper.GetCastlingMerge((int)(status & CastlingAvail.QueenSideWhite));
                case Square.h1:
                    return MovePackHelper.GetCastlingMerge((int)(status & CastlingAvail.KingSideWhite));
                case Square.a8:
                    return MovePackHelper.GetCastlingMerge((int)(status & CastlingAvail.QueenSideBlack));
                case Square.h8:
                    return MovePackHelper.GetCastlingMerge((int)(status & CastlingAvail.KingSideBlack));
            }
            return 0;
        }


        public static  int GetCaptureDescriber(IPiece moving, IPiece capt)
        {
            int val = Capture;

            if (moving.CapValue <= capt.CapValue)
            {
                // good capture
                val |= GoodCapture;
                val |= ((capt.CapValue - moving.CapValue) << 18);
            }
            else
            {
                // loosing capture
                val |= ((moving.CapValue - capt.CapValue) << 18);
            }
            
            return val;
        }

        public static int GetCleanedMove(int p)
        {
            return (p & (0xFFFF | (0xF << 27)));
        }

        public static string GetAlgebraicString(int p)
        {
            return string.Format("{0}{1}{2}", GetSquareString(GetStartSquare(p)),GetSquareString( GetEndSquare(p)),GetPromotionString(p));
        }

        private static object GetPromotionString(int p)
        {
            PromotionTo pm = GetPromotion(p);
            switch (pm)
            {
                case PromotionTo.Bishop:
                    return "B";
                case PromotionTo.Knight:
                    return "N";
                case PromotionTo.Queen:
                    return "Q";
                case PromotionTo.Rook:
                    return "R";
                default:
                    return "";
            }
        }

        public static string GetSquareString(int p)
        {
            string ranks = " 12345678";
            string cols = " abcdefgh";
            return string.Format("{0}{1}",cols[Square.Col(p)], ranks[Square.Rank(p)]);
        }

        public static int DecodeAlgebraic(string p)
        {
            if (Regex.Match(p, "[abcdefgh][12345678][abcdefgh][12345678][qQrRnNbB]?").Success)
            {
                return Pack(DecodeSquareInternal(p.Substring(0,2)), DecodeSquareInternal(p.Substring(2)))|DecodePromotion(p);
            }
            else
            {
                throw new Exception("Malformed move:" + p);
            }
        }

        private static int DecodePromotion(string p)
        {
            if (p.Length > 4)
            {
                switch (p.Substring(4).ToLower())
                {
                    case "q":
                        return GetPromotionMerge(PromotionTo.Queen);
                    case "b":
                        return GetPromotionMerge(PromotionTo.Bishop);
                    case "r":
                        return GetPromotionMerge(PromotionTo.Rook);
                    case "n":
                        return GetPromotionMerge(PromotionTo.Knight);
                }
            }
            return 0;
        }
        public static int DecodeSquare(string p)
        {
            if (Regex.Match(p, "[abcdefgh][12345678]").Success)
            {
                return DecodeSquareInternal(p);
            }
            else
            {
                throw new Exception("Malformed square" + p);
            }
        }
        private static int DecodeSquareInternal(string p)
        {
            string ranks = "12345678";
            string cols = "abcdefgh";
            return cols.IndexOf(p[0]) | ranks.IndexOf(p[1]) << 4;
        }

        public static CastlingAvail GetCastleMask(int move)
        {
            return (CastlingAvail)(move >> 21 & 0xf);
        }
    }
}
