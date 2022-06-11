using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CnsLogTestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var i = 0;
            while (true)
            {
                i++;
                Console.WriteLine($"Hello world! : {i}");
                Debug.WriteLine($"Hello world! : {i}");
                Thread.Sleep(2000);
            }
        }
    }
}
