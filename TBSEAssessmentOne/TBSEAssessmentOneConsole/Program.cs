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
        static void Main(string[] args)
        {
            StoreAnalyser SA = new StoreAnalyser();

            SA.ReadAllFiles();
            SA.MainLoop();
        }
    }
}