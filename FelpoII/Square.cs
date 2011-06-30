using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FelpoII
{
    public static class Square
    {
        public static int Ox88Dist(int sq1, int sq2)
        {
            return 0x77 + sq1 - sq2;
        }
        public static int Rank(int square)
        {
            return 1 + (square >> 4);
        }
        public static int Col(int square)
        {
            return 1 + (square & 0xF);
        }
        public static bool SquareValid(int square)
        {
            return (square & 0x88) == 0;
        }
        public const int Invalid = 0x88;
        public const int a1=   0;
        public const int  b1=   1;
        public const int  c1=   2;
        public const int d1=   3;
        public const int e1=   4;
        public const int f1=   5;
        public const int g1=   6;
        public const int h1=   7;
         
        public const int a2=   16;
        public const int b2=   17;
        public const int c2=   18;
        public const int d2=   19;
        public const int e2=   20;
        public const int f2=   21;
        public const int g2=   22;
        public const int h2=   23;

        public const int a3=    32;
        public const int b3=    33;
        public const int c3=    34;
        public const int d3=    35;
        public const int e3=    36;
        public const int f3=    37;
        public const int g3=    38;
        public const int h3=    39;
        
        public const int a4=    48;
        public const int b4=    49;
        public const int c4=    50;
        public const int d4=    51;
        public const int e4=    52;
        public const int f4=    53;
        public const int g4=    54;
        public const int h4=    55;
        
        public const int a5=    64;
        public const int b5=    65;
        public const int c5=    66;
        public const int d5=    67;
        public const int e5=    68;
        public const int f5=    69;
        public const int g5=    70;
        public const int h5=    71;
        
        public const int a6=    80;
        public const int b6=    81;
        public const int c6=    82;
        public const int d6=    83;
        public const int e6=    84;
        public const int f6=    85;
        public const int g6=    86;
        public const int h6=    87;
        
        public const int a7=    96;
        public const int b7=    97;
        public const int c7=    98;
        public const int d7=    99;
        public const int e7=    100;
        public const int f7=    101;
        public const int g7=    102;
        public const int h7=    103;

        public const int a8= 112;
        public const int b8= 113;
        public const int c8= 114;
        public const int d8= 115;
        public const int e8= 116;
        public const int f8= 117;
        public const int g8= 118;
        public const int h8= 119;
    }
}
