using System;
using System.Collections.Generic;
using System.Text;

namespace FelpoII.Core
{
    public class EndGameEventArgs:EventArgs
    {
        private Side winningSide;

        public Side MateSide
        {
            get { return winningSide; }
            set { winningSide = value; }
        }
        private bool mate;

        public bool Mates
        {
            get { return mate; }
            set { mate = value; }
        }

        private bool staleMate;

        public bool StaleMates
        {
            get { return staleMate; }
            set { staleMate = value; }
        }
	
	
	
    }
}
