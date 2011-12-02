using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FelpoII.Core;
using FelpoII.Core.Search;
using System.Diagnostics;
using FelpoII.Core.Engines;

namespace EngineTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FullEngineSuite
    {
        public FullEngineSuite()
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

        //"00:01:41.0562645"
        //
        [TestMethod]
        public void TestWac001()
        {
            using (var tt = new UnmanagedTranspositionTable())
            {
                using (var rc = new RunClock())
                {
                    var iengine = new FullEngine(6, tt);
                    iengine.Message += (s, e) =>
                        {
                            Trace.WriteLine(e.Message);
                        };
                    var engine = new SynchronEngineAdapter(iengine
                        , "2rr3k/pp3pp1/1nnqbN1p/3pN3/2pP4/2P3Q1/PPB4P/R4RK1 w - - 1 1"
                        , TimeSpan.FromSeconds(6)
                        );
                    Assert.AreEqual("g3g6", engine.Search());
                    Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
                }
            }
        }
        
        [TestMethod]
        public void TestWac002()
        {
            using (var rc = new RunClock())
            using (var tt = new UnmanagedTranspositionTable())
            {
                var iengine = new FullEngine(8, tt);
                iengine.Message += (s, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                var engine = new SynchronEngineAdapter(iengine
                    , "8/7p/5k2/5p2/p1p2P2/Pr1pPK2/1P1R3P/8 b - - 1 1"
                    , TimeSpan.FromSeconds(10)
                    );
                Assert.AreEqual("b3b2", engine.Search());
                Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
            }
           
        }
        [TestMethod]
        public void TestWac003()
        {
            using (var rc = new RunClock())
            using (var tt = new UnmanagedTranspositionTable())
            {
                var iengine = new FullEngine(5, tt);
                iengine.Message += (s, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                var engine = new SynchronEngineAdapter(iengine
                    , "5rk1/1ppb3p/p1pb4/6q1/3P1p1r/2P1R2P/PP1BQ1P1/5RKN w - - 1 1"
                    , TimeSpan.FromSeconds(10)
                    );
                Assert.AreEqual("e3g3", engine.Search());
                Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
            }

        }
        [TestMethod]
        public void TestWac004()
        {
            using (var rc = new RunClock())
            using (var tt = new UnmanagedTranspositionTable())
            {
                var iengine = new FullEngine(5, tt);
                iengine.Message += (s, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                var engine = new SynchronEngineAdapter(iengine
                    , "r1bq2rk/pp3pbp/2p1p1pQ/7P/3P4/2PB1N2/PP3PPR/2KR4 w - - 1 1"
                    , TimeSpan.FromSeconds(10)
                    );
                Assert.AreEqual("h6h7", engine.Search());
                Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
            }

        }
        [TestMethod]
        public void TestWac005()
        {
            using (var rc = new RunClock())
            using (var tt = new UnmanagedTranspositionTable())
            {
                var iengine = new FullEngine(5, tt);
                iengine.Message += (s, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                var engine = new SynchronEngineAdapter(iengine
                    , "5k2/6pp/p1qN4/1p1p4/3P4/2PKP2Q/PP3r2/3R4 b - - 1 1"
                    , TimeSpan.FromSeconds(10)
                    );
                Assert.AreEqual("c6c4", engine.Search());
                Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
            }

        }
        [TestMethod]
        public void TestWac006()
        {
            using (var rc = new RunClock())
            using (var tt = new UnmanagedTranspositionTable())
            {
                var iengine = new FullEngine(5, tt);
                iengine.Message += (s, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                var engine = new SynchronEngineAdapter(iengine
                    , "7k/p7/1R5K/6r1/6p1/6P1/8/8 w - - 1 1"
                    , TimeSpan.FromSeconds(10)
                    );
                Assert.AreEqual("b6b7", engine.Search());
                Console.WriteLine("Elapsed milliseconds:" + rc.GetElapsedMilliseconds());
            }

        }

    }
}
