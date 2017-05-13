using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FruitHAP.Core.Service;

namespace FruitHAP.Startup
{
    class Program
    {
        static void Main(string[] args)
		{
			try
			{
			    var container = new WindsorContainer ().Install (FromAssembly.This ());
                var service = container.Resolve<IFruitHAPService>();
                service.Start();
                ConsoleKeyInfo key = default(ConsoleKeyInfo);
                while (key.KeyChar != 'q')
                {
                    key = Console.ReadKey();
                    if (key.KeyChar == 's')
                    {
                        service.Stop();
                    }

                    if (key.KeyChar == 'r')
                    {
                        service.Start();
                    }

                }

                service.Stop();

                Console.WriteLine ("STOPPED");
                Environment.Exit(0);
			}
			catch (Exception ex) 
			{
				Console.WriteLine ("Exception during main(): {0}", ex);
			}
		}
    }
}
