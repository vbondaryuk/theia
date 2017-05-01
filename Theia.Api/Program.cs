using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace Theia.Api
{
    class Program
    {
        private const string BaseAddress = "http://*:6535";
        static void Main(string[] args)
        {
            DisplayIPAddresses();
            try
            {
                using (WebApp.Start<Startup>(url: BaseAddress))
                {
                    Console.WriteLine("Application started...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public static void DisplayIPAddresses()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface network in networkInterfaces)
                {
                    // Read the IP configuration for each network 
                    IPInterfaceProperties properties = network.GetIPProperties();

                    // Each network interface may have multiple IP addresses 
                    foreach (IPAddressInformation address in properties.UnicastAddresses)
                    {
                        // We're only interested in IPv4 addresses for now 
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        // Ignore loopback addresses (e.g., 127.0.0.1) 
                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        sb.AppendLine(address.Address.ToString() + " (" + network.Name + ")");
                    }
                }

                Console.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get Ip adress exception: {ex.Message}" );
            }
            
        }
    }
}