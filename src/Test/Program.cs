using Microsoft.Owin.Hosting;
using System;

namespace Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }
    }
}
