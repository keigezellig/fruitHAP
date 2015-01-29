﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Topshelf;

namespace FruitHAP.Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer().Install(FromAssembly.This());
            var serviceHostConfigurator = new ServiceHostConfigurator(container);
            HostFactory.Run(serviceHostConfigurator.Configure);
        }
    }
}
