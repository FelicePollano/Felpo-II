using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EngineTests
{
    class EnumTests
    {
        static public IEnumerable<TestGame> GetGames()
        {
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("EngineTests.Suites.TestSuite.epd")))
            {
                string line = string.Empty;
                while(null != (line= reader.ReadLine()))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var tg = new TestGame();
                        var split1 = Regex.Split(line, "bm");
                        tg.Board = split1[0];
                        tg.BestMove = split1[1].Split(';')[0];
                        yield return tg;
                    }
                }
            }
        }
    }
}
