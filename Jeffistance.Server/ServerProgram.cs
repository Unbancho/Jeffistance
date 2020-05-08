using System;
using Jeffistance.JeffServer.Services.MessageProcessing;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.JeffServer.Models;

namespace Jeffistance.JeffServer
{
    class Program
    {
        static Server Server;

        static void Main(string[] args)
        {
            Server = new Server();
            Server.Run(7700);
            string input;
            while((input = Console.ReadLine().ToLower()) != "stop")
                HandleInput(input);
        }

        static void HandleInput(string input)
        {
            Console.WriteLine($"{input}ed!\n");
        }
    }
}
