using System.Configuration;
using Castle.Windsor;
using EasyNetQ;

namespace EventNotifierService.Startup
{
    public class BusBuilder
    {
        private readonly IWindsorContainer container;

        public BusBuilder(IWindsorContainer container)
        {
            this.container = container;
        }
        public IBus CreateMessageBus()
        {
            var connectionString = ConfigurationManager.AppSettings["mqConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new EventNotifierServiceException("MQ connection string is missing");
            }
            return RabbitHutch.CreateBus(connectionString, x => x.Register<IEasyNetQLogger>(_ => container.Resolve<IEasyNetQLogger>()));
        }
    }
}

