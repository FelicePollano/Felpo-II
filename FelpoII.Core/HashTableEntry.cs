using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FelpoII.Core
{
    struct HashTableEntry
    {
        ulong key;
        public ulong Payload;
        public const ulong KeyMask = 0xFFFFFFFUL;
        //public byte type;
        public const int MoveCountType = 0x80;
        public const int ExactType = 0x40;
        public const int AlphaType = 0x20;
        public const int BetaType = 0x10;

        public ulong Key
        {
            get { return key & ~KeyMask; }
            set { key &= KeyMask; key |= (value & ~KeyMask); }
        }

        public byte Type
        {
            get { return (byte)(key & 0xFFUL); }
            set { key &= ~0xFFUL;key |= value; }
        }
        public byte Depth
        {
            get { return (byte)((key & 0xFF00UL)>>8); }
            set { key &= ~0xFF00UL; key |= ((ulong)value<<8); }
        }

        
    }
}
