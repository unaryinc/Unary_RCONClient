using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using LiteNetLib;
using System.Threading;

namespace Unary.RCONClient
{
    public static class Program
    {
        public static bool Running = true;

        static void Main(string[] args)
        {
            Config NewConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));
            Client NewClient = new Client();
            NewClient.Start(NewConfig.IP, NewConfig.Port, NewConfig.Password);

            Thread PollThread = new Thread(() =>
            {
                while(Running)
                {
                    NewClient.Poll();
                    Thread.Sleep(15);
                }
            });

            PollThread.Start();

            while (Running)
            {
                string Input = Console.ReadLine();

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                if (Input == "!quit")
                {
                    Running = false;
                }
                else
                {
                    NewClient.Send(Input);
                }
            }

            PollThread.Join();

            NewClient.Stop();
        }
    }
}