using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using Theia.Common.Utils;

namespace Theia.Api
{
    class Program
    {
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        static void Main()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                ExitEvent.Set();
            };

            try
            {
                var address = $"{StaticVariables.TheiaHost}:{StaticVariables.TheiaPort}";
                Console.WriteLine("Starting web Server...");
                WebApp.Start<Startup>(address);
                Console.WriteLine("Server running at {0}", address);
                
                ExitEvent.WaitOne();
                Console.WriteLine("Server stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}