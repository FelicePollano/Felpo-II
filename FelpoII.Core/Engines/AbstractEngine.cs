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
        protected abstract int DoSearch(IChessBoard board,TimeSpan timeAvailable);

        #region IEngine Members

        public void Break()
        {
            OnBreak();
        }

        public void BeginSearch(string fen, ISearchResults target,TimeSpan timeVailable)
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
                        int bestMove = DoSearch(board,timeVailable);
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

        #region IEngine Members

        protected virtual void ReportMessage(string msg)
        {
            Message(this, new InfoMessageEventArgs(msg));
        }
        public event EventHandler<InfoMessageEventArgs> Message = delegate { };

        #endregion
    }
}
