using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Samples
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Running sample 1: simple");
            SimpleExample.Run();
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Running sample 2: stop");
            StopExample.Run();
        }
    }
}
