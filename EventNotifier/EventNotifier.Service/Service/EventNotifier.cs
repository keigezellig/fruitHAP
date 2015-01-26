using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using EasyNetQ;
using EventNotifierService.Common.Plugin;
using FruitHAP.Messages;

namespace EventNotifierService.Service
{
    public class EventNotifier : IEventNotifier
    {
        private readonly IBus bus;
        private readonly IList<IPlugin> handlers;
        private IDisposable consumer;
        private ILogger logger = NullLogger.Instance;        
        
        public EventNotifier(IBus bus, IList<IPlugin> handlers)
        {
            this.bus = bus;
            this.handlers = handlers;
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }


        public void Start()
        {
            logger.Info("Starting");
            consumer = bus.Subscribe<DoorMessage>(string.Empty,
                message =>
                {
                    logger.InfoFormat("Message received! {0}", message);
                    
                    foreach (var handler in handlers)
                    {
                        handler.HandleMessage(message);
                    }

                });
        }

        public void Stop()
        {
            if (consumer != null)
            {
                consumer.Dispose();
            }
        }

    }
}

