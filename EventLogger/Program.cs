using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace EventLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventLogger =  new EventLogger();
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
