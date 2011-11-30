using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using System.Threading;

namespace FelpoII.Core.Engines
{
    public class SynchronEngineAdapter
    {
       class ResultCollector : ISearchResults
       {
           ManualResetEvent barrier;
            public ResultCollector()
            {
                barrier = new ManualResetEvent(false);
            }
            public string BestMove { get; set; }
           #region ISearchResults Members

           public void SearchDone(string bestMove, GameEnded gameEnded)
           {
               BestMove = bestMove;
               barrier.Set();
           }

           #endregion
           public void Wait()
           {
               barrier.WaitOne();
           }
       }
        IEngine engine;
        string fen;
        public SynchronEngineAdapter(IEngine engine,string fen)
        {
            this.engine = engine;
            this.fen = fen;
        }
        public string Search()
        {
            var rc = new ResultCollector();
            engine.BeginSearch(fen, rc);
            rc.Wait();
            return rc.BestMove;
        }
    }
}
