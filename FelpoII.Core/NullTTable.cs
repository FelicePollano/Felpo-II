using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core
{
    public class NullTTable:ITranspositionTable
    {
        #region ITranspositionTable Members

        public bool ProbeMovesCount(ulong zkey, out ulong moves, int depth)
        {
            moves = 0;
            return false;
        }

        public void StoreMovesCount(ulong zkey, ulong moves, int depth)
        {
            
        }

        public bool Probe(ulong zkey, int depth, int alpha, int beta, ref int ttMove, ref int score)
        {
            return false;
        }

        public void Save(ulong zkey, int depth, int score, TTType tTType, int p)
        {
            
        }

        #endregion
    }
}
