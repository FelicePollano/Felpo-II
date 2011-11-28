using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FelpoII;
using FelpoII.Search;

namespace EngineTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PlainAlphaBetaSuite
    {
        public PlainAlphaBetaSuite()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            
            var game = EnumTests.GetGames().First();

            int currentBestMove=0;
            int bestDepth = 0;

            OX88Chessboard board = new OX88Chessboard(game.Board);
            
            Side side = board.ToMove;

            PlainAlphaBeta algo = new PlainAlphaBeta();
           
            algo.getMoves = (currentBoard) =>
                {
                    int[] moves = new int[100];
                    var n = currentBoard.GetMoves(0,moves);
                    return moves.Take(n);
                };
            algo.foundMove = (currentBoard, move, depth, score) =>
            {
                if (depth >= bestDepth && side == currentBoard.ToMove)
                {
                    currentBestMove = move;
                    bestDepth = depth;
                    
                }
            };

            algo.eval = (currentBoard) =>
            {
                int val = 0;
                foreach (var v in currentBoard.BoardArray)
                {
                    if (null != v)
                    {
                        if (v.Owner == currentBoard.ToMove)
                            val += v.Value;
                        else
                            val -= v.Value;
                    }
                }
                return val;
            };
            algo.quiesce = (chessBoard, alpha, beta) =>
            {
                return algo.eval(chessBoard);
            };

            algo.Search(board, PlainAlphaBeta.MINEVAL, PlainAlphaBeta.MAXEVAL, 5);

            
            var best = MovePackHelper.GetAlgebraicString(currentBestMove);
            Assert.AreEqual(game.BestMove, best);
        }
    }
}
