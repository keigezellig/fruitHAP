using System;
using SimulatorCommon;

namespace DoorPi.CliSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string imagepath = "";
            
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: {0} connection_string ", Environment.GetCommandLineArgs()[0]);
                Environment.Exit(1);
            }

            string connectionString = args[0];

            if (args.Length == 2)
            {
                imagepath = args[1];
            }

            try
            {
                SimulatorLogic.PublishRingMessage(connectionString, imagepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex);
            }


        }
    }
}
