using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FelpoII.Core.Interfaces
{
    public enum GameEnded
    {
        None,
        WhiteMate,
        BlackMate,
        Draw,
    }
    public interface ISearchResults
    {
        void SearchDone(string bestMove, GameEnded gameEnded);
    }
}
