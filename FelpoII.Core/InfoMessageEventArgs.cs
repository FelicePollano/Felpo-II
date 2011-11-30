using System;
using System.Collections.Generic;
using System.Text;

namespace FelpoII.Core
{
    public class InfoMessageEventArgs:EventArgs
    {
        public InfoMessageEventArgs(string message)
        {
            this.message = message;
        }
        string message;
        public string Message
        {
            get { return message; }
        }
    }
}
