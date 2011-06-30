using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FelpoII.Interfaces
{
    public interface IEngine
    {
        void Break();

        void Search();

        Side Side { get; set; }
        Side MovingSide { get; set; }
        string BestMove { get; }
    }
}
