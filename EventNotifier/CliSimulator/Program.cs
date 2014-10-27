using System;
using SimulatorCommon;

namespace DoorPi.CliSimulator
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: {0} connection_string ", Environment.GetCommandLineArgs()[0]);
                Environment.Exit(1);
            }

            string connectionString = args[0];

            try
            {
                SimulatorLogic.PublishRingMessage(connectionString,@"C:\Users\Public\Pictures\Sample Pictures\desert.jpg");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex);
            }


        }
    }
}
