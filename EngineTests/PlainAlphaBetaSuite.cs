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

        //"00:01:41.0562645"
        //
        [TestMethod]
        public void TestQg6()
        {
            var engine = new SynchronEngineAdapter(new SimpleVanillaEngine(), "2rr3k/pp3pp1/1nnqbN1p/3pN3/2pP4/2P3Q1/PPB4P/R4RK1 w - - 1 1");
            Assert.AreEqual("g3g6", engine.Search());
        }
        [TestMethod]
        public void TestQxf3()
        {
            var engine = new SynchronEngineAdapter(new SimpleVanillaEngine(), "4r1k1/p1pb1ppp/Qbp1r3/8/1P6/2Pq1B2/R2P1PPP/2B2RK1 b - - 1 1");
            Assert.AreEqual("f2f3", engine.Search());
        }
    }
}
