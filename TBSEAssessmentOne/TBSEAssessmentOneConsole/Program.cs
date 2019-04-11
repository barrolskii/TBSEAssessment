using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace TBSEAssessmentOneConsole
{
    class Program
    {
        public static int fileCount = 10400;
        public static int currentCount = 0;

        static void Main(string[] args)
        {
            StoreAnalyser SA = new StoreAnalyser();

            SA.Start();
            SA.MainLoop();
        }
    }
}