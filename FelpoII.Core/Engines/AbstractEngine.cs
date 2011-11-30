using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Core.Interfaces;
using System.ComponentModel;

namespace FelpoII.Core.Engines
{
    public abstract class AbstractEngine:IEngine
    {
        protected abstract void OnBreak();
        protected abstract IChessBoard CreateBoard(string fen);
        protected abstract int DoSearch(IChessBoard board);
        #region IEngine Members

        public void Break()
        {
            OnBreak();
        }

        public void BeginSearch(string fen, ISearchResults target)
        {
            var board = CreateBoard(fen);
            var eg = EndGameReporter.ReportEndGame(board);
            if (eg != GameEnded.None)
            {
                target.SearchDone(string.Empty, eg);
            }
            else
            {
                BackgroundWorker wrk = new BackgroundWorker();
                wrk.DoWork += (e, a) =>
                    {
                        int bestMove = DoSearch(board);
                        string move = string.Empty;
                        if (bestMove != 0)
                        {
                            move = MovePackHelper.GetAlgebraicString(bestMove);
                        }
                        target.SearchDone(move, EndGameReporter.ReportEndGame(board));
                    };
                wrk.RunWorkerAsync();
            }
        }

        #endregion
    }
}
