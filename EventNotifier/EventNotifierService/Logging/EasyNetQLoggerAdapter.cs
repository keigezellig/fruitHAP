using Castle.Core.Logging;
using EasyNetQ;
using System;

namespace EventNotifierService.Logging
{
    public class EasyNetQLoggerAdapter : IEasyNetQLogger
    {
        private readonly ILogger logger;

        public EasyNetQLoggerAdapter(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }

        public void DebugWrite(string format, params object[] args)
        {
            logger.DebugFormat(format, args);
        }

        public void ErrorWrite(Exception exception)
        {
            logger.Error("Exception occurred: ", exception);
        }

        public void ErrorWrite(string format, params object[] args)
        {
            logger.ErrorFormat(format, args);
        }

        public void InfoWrite(string format, params object[] args)
        {
            logger.InfoFormat(format, args);
        }
    }
}
