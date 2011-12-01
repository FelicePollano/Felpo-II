using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using FelpoII.Core.Interfaces;
using FelpoII.Core;

namespace FelpoII
{
    
    public class WinBoardAdapter
    {
        IEngine engine;
        IChessBoard board;
        ITranspositionTable hashTable;
        bool force;
        bool edit;
        Side movingSide = Side.White;
        private int ply = 5;

        public int Ply
        {
            get { return ply; }
            set { ply = value; }
        }
       
        public WinBoardAdapter(IChessBoard board,IEngine engine,ITranspositionTable hashTable)
        {
            this.board = board;
            this.engine = engine;
        }
        public void Break()
        {
            if (null != engine)
            {
                engine.Break();
            }
        }
        public void Consume(string input)
        {
            
            string[] tokens = input.Split(' ');
            string rPart="";
            for (int i = 1; i < tokens.Length; ++i)
            {
                rPart += tokens[i];
                rPart += " ";
            }
            rPart = rPart.Trim();
            if (!edit)
            {
                switch (tokens[0])
                {
                    //Ignore unhandled commands
                    case "level":
                    case "otim":
                    case "time":
                    case "random":
                    case "hard":
                    case "easy":
                    case "sd":
                    case "st":
                    case "strong":
                    case "post":
                    case "result":
                    case "computer":
                    case "?":
                        break;
                    case "savepos":
                        OnMessageToWinboard(board.SavePos());
                        break;
                    case "divide":
                        int depth = int.Parse(rPart);
                        board.DividePartialResult+=new EventHandler(div_DividePartialResult);
                        DivideResults div = board.Divide(depth);
                        OnMessageToWinboard(string.Format("Total Nodes: {0}", div.NodesCount));
                        OnMessageToWinboard(string.Format("Moves Count: {0}",div.MovesCount));
                        board.DividePartialResult -= new EventHandler(div_DividePartialResult);
                        break;
                    case "qperft":
                        OnMessageToWinboard("Performing perft with hash table");
                        depth = int.Parse(rPart);
                        PerfResults res2 = board.Perft(depth, true,hashTable);
                        OnMessageToWinboard(string.Format("Depth: {3} {0} moves {1:0.00} seconds. {2:0.000} Move/s. Hash Hit={4}", res2.MovesCount, (double)res2.Elapsed / 1000.0, (double)res2.MovesCount / (res2.Elapsed / 1000.0), depth,res2.HashHit));
                        break;
                    case "perft":
                        depth = int.Parse(rPart);
                        PerfResults res = board.Perft(depth,false,null);
                        OnMessageToWinboard(string.Format("Depth: {3} {0} moves {1:0.00} seconds. {2:0.000} Move/s", res.MovesCount, (double)res.Elapsed / 1000.0, (double)res.MovesCount / (res.Elapsed / 1000.0), depth));
                        break;
                    
                    case "perf":
                        PerfResults undoRedo;
                        PerfResults mgen = board.Perf(out undoRedo, 400000);
                        OnMessageToWinboard(string.Format("Generated {0} moves in {1:0.00} seconds. {2:0.00} Move/s", mgen.MovesCount, (double)mgen.Elapsed / 1000.0, (double)mgen.MovesCount / (mgen.Elapsed / 1000.0)));
                        OnMessageToWinboard(string.Format("{0} moves Done/Undone in {1:0.00} seconds. {2:0.00} Move/s", undoRedo.MovesCount, (double)undoRedo.Elapsed / 1000.0, (double)undoRedo.MovesCount / (undoRedo.Elapsed / 1000.0)));
                        break;
                    case "setboard":
                        board.SetBoard(rPart);
                        break;
                    case "remove":
                        Break();
                        board.UndoMove();
                        board.UndoMove();
                        break;
                    case "undo":
                        board.UndoMove();
                        break;
                    case "new":
                        Break();
                        force = false;
                        board.SetBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                        InitializeEngine();
                        break;

                    case "dump":
                        StringBuilder sb = new StringBuilder();
                        board.Dump(new StringWriter(sb));
                        OnMessageToWinboard(sb.ToString());
                        break;
                    case "go":
                        force = false;
                        AsyncStartPlay();
                        break;
                    case "force":
                        Break();
                        force = true;
                        break;
                    case "black":
                        Break();
                        movingSide = Side.Black;
                        throw new NotImplementedException();
                    case "white":
                        Break();
                        movingSide = Side.White;
                        throw new NotImplementedException();
                    case "edit":
                        Break();
                        edit = true;
                        break;
                    default:
                        try
                        {
                            board.Move(tokens[0]);
                        }
                        catch (Exception e)
                        {
                            string s = string.Format("Error ({0}):", e.Message);
                            OnMessageToWinboard(s);
                        }
                        break;
                }
            }
            else
            {
                switch (tokens[0])
                {
                    case "#":
                        board.SetBoard("/8/8/8/8/8/8/8/8 w KQkq - 0 1");
                        //engine.Board = board;
                        editingSide = Side.White;
                        break;
                    case ".":
                        throw new NotImplementedException();
                        //edit = false;
                        //engine.OpeningBook = null; // remove opening book
                        break;
                    case "c":
                        editingSide = Side.Black;
                        break;
                    default:
                        
                        break;
                }
            }
        }

        void div_DividePartialResult(object sender, EventArgs e)
        {
            IChessBoard b =  sender as IChessBoard;
            OnMessageToWinboard(string.Format("{0}: {1}",b.CurrentDivideMove,b.CurrentDivideNodeCount));
        }

       

       
        private Side editingSide;
        delegate void AsyncMoveDelegate(string s);
        private void DoMove(string p)
        {
            if (!force)
            {
                board.Move(p);
                engine.BeginSearch(board.SavePos(),null,TimeSpan.FromHours(1));
            }
            else
            {
                
                board.Move(p);
            }
        }
        
        private void AsyncStartPlay()
        {
            engine.BeginSearch(board.SavePos(),null,TimeSpan.FromHours(1));
        }
        
       
       
       
        public IEngine Engine
        {
            get { return engine; }
        }
        public event EventHandler EngineCreated;
        private void InitializeEngine()
        {
           
        }

        void engine_Message(object sender, InfoMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        void engine_EndGame(object sender, EndGameEventArgs e)
        {
            string s = null;
            if (e.StaleMates)
                s = "1/2-1/2 {Stalemate}";
            else
            {
                if (e.MateSide == Side.White)
                    s = "0-1 {Black Mates}";
                else
                    s = "1-0 {White Mates}";
            }
            if( !string.IsNullOrEmpty( s) )
                OnMessageToWinboard(s);
        }

        void engine_WaitOpponentForMove(object sender, EventArgs e)
        {
            movingSide = movingSide == Side.White ? Side.Black : Side.White;
        }

        void engine_EngineMove(object sender, EventArgs e)
        {
            //OnMessageToWinboard("move "+((sender) as IEngine).BestMove);
        }
        protected virtual void OnMessageToWinboard(string s)
        {
            if( null != MessageToWinboard )
            {
                EngineToWinboardEventArgs e = new EngineToWinboardEventArgs();
                e.Message = s;
                MessageToWinboard(this,e);
            }
        }
	
        public event EventHandler<EngineToWinboardEventArgs> MessageToWinboard;
    }
}
