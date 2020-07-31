using System;
using System.Linq;
using Jeffistance.Common.ExtensionMethods;
using Jeffistance.Common.Models;
using System.Reflection;
using ModusOperandi.Messaging;
using Jeffistance.JeffServer.Models;


namespace Jeffistance.JeffServer
{
    class Program
    {
        static Server Server;

        static void Main(string[] args)
        {
            Server = new Server(dedicated: true);
            Server.Run(7700);
            string input;
            while((input = Console.ReadLine().ToLower()) != "stop")
                HandleInput(input);
        }

        static void HandleInput(string input)
        {
            string[] command = input.Split(' ');
            string commandName = command[0];
            string[] args = command.Skip(1).ToArray();
            var methods = typeof(Program).GetMethods();
            MethodInfo methodToInvoke = typeof(Program).GetMethod(commandName.Capitalized(), bindingAttr:BindingFlags.Static | BindingFlags.Public);
            methodToInvoke?.Invoke(null, args);
        }

        public static void Say(string message)
        {
            Server.Broadcast(new Message(message));
        }

        public static void Kick(string username)
        {
            User userToKick = Server.GetUser(username);
            if(userToKick != null)
                Server.Kick(userToKick);
        }
    }
}
