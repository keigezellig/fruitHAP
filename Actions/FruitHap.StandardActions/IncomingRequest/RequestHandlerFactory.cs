using System;
using FruitHap.StandardActions.IncomingRequest.RequestHandlers;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHap.Core.Action;

namespace FruitHap.StandardActions.IncomingRequest
{
	public class RequestHandlerFactory : IRequestHandlerFactory
	{
		private ILogger logger;
		private ISensorRepository sensorRepository;

		#region IRequestHandlerFactory implementation

		public IRequestHandler GetRequestHandler (SensorMessage request)
		{
			if (request == null || string.IsNullOrEmpty (request.SensorName)) 
			{
				return new EmptyRequestHandler ();
			}

			RequestDataType requestType = RequestDataType.Undefined;
			bool isValidRequestType = RequestDataType.TryParse (request.EventType, out requestType);
			if (!isValidRequestType)
			{
				return new UnknownRequestTypeHandler();
			}

			switch (requestType) 
			{
			case RequestDataType.Command:
				return new CommandHandler (logger, sensorRepository);
			case RequestDataType.GetValue:
				return new GetValueHandler (logger, sensorRepository);
			default:
				return new UnknownRequestTypeHandler();
			}
				
		}

		#endregion

		
		public RequestHandlerFactory (ILogger logger, ISensorRepository repository)
		{
			this.sensorRepository = repository;
			this.logger = logger;
		}
	}
}

