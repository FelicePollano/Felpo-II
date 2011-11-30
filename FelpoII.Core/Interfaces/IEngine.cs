using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FelpoII.Core.Interfaces
{
    public interface IEngine
    {
        void Break();

        void BeginSearch(string fen,ISearchResults target);

        
       
    }
}
