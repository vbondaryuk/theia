using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Microsoft.Owin.Hosting;
using Theia.Common.Utilits;

namespace Theia.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayIpAddresses();
            try
            {
                var address = $"{StaticVariables.TheiaHost}:{StaticVariables.TheiaPort}";
                using (WebApp.Start<Startup>(address))
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

        public static void DisplayIpAddresses()
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
                    foreach (var address in properties.UnicastAddresses)
                    {
                        // We're only interested in IPv4 addresses for now 
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        // Ignore loopback addresses (e.g., 127.0.0.1) 
                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        sb.AppendLine(address.Address + " (" + network.Name + ")");
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