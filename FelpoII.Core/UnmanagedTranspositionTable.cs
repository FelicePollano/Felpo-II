using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;

namespace FelpoII.Core
{
    public class UnmanagedTranspositionTable:ITranspositionTable,IDisposable
    {

        const int MaskForHash = 0x7FFFFF;

        public UnmanagedTranspositionTable()
        {
            HashTable.Initialize(MaskForHash);    
        }
        #region IDisposable Members

        public void Dispose()
        {
            HashTable.Drop();
        }

        #endregion

        #region ITranspositionTable Members

        public bool ProbeMovesCount(ulong zkey, out ulong moves, int depth)
        {
            return HashTable.ProbeMovesCount(zkey, out moves, depth);
        }

        public void StoreMovesCount(ulong zkey, ulong moves, int depth)
        {
            HashTable.StoreMovesCount(zkey, moves, depth);
        }

        #endregion

        #region ITranspositionTable Members


        public bool Probe(ulong zkey, int depth, int alpha, int beta, ref int ttMove, ref int score)
        {
            return HashTable.Probe( zkey,  depth,  alpha,  beta,  ref ttMove, ref score);
        }

        public void Save(ulong zkey, int depth, int score, TTType tTType, int move)
        {
            byte type = HashTableEntry.AlphaType;
            switch(tTType)
            {
                case TTType.Alpha:
                    type = HashTableEntry.AlphaType;
                    break;
                case TTType.Beta:
                    type = HashTableEntry.BetaType;
                    break;
                case TTType.Exact:
                    type = HashTableEntry.ExactType;
                    break;
            }
            HashTable.Save(zkey, depth, score, type, move);
        }

        #endregion
    }
}
