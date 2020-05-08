using System;
using System.Linq;
using Jeffistance.Common.ExtensionMethods;
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
            Server = new Server();
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

        public static void Kick(string username) // TODO: A way to get User by name, maybe a Dictionary
        {
            foreach (var user in Server.UserList)
            {
                if(username == user.Name.ToLower())
                {
                    Server.Kick(user);
                    return;
                }
            }
        }
    }
}
