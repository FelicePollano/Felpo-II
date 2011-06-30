using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Interfaces;

namespace FelpoII
{
    static class Extension
    {
        public static void Add(this IPiece[] orig,IPiece piece)
        {
            for (int index = 0; index < orig.Length; ++index)
            {
                if (orig[index] == null)
                {
                    orig[index] = piece;
                    break;
                }
            }
        }
    }
}
