using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FelpoII.Core.Interfaces
{
    public enum TTType
    {
        Exact
        ,Alpha
        ,Beta
    }
    public interface ITranspositionTable
    {
        bool ProbeMovesCount(ulong zkey, out ulong moves, int depth);
        void StoreMovesCount(ulong zkey, ulong moves, int depth);
        bool Probe(ulong zkey, int depth, int alpha, int beta, ref int ttMove, ref int score);
        void Save(ulong zkey, int depth, int score, TTType tTType, int p);
    }
}
