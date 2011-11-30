using System;
using System.Collections.Generic;
using System.Text;

namespace FelpoII.Core
{
    public enum CastlingAvail
    {
        KingSideWhite=1,QueenSideWhite=2,KingSideBlack=4,QueenSideBlack=8,None = 0
        ,All = KingSideWhite|KingSideBlack|QueenSideWhite|QueenSideBlack
    }
}
