using System;
using System.Collections.Generic;
using System.Text;

namespace FelpoII
{
    public class EngineToWinboardEventArgs:EventArgs
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
	
    }
}
