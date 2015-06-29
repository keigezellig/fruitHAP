using System;
using FruitHap.StandardActions.Messages.Outbound;
using FruitHap.StandardActions.IncomingRequest.RequestHandlers;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;

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

			DataType requestType = DataType.Undefined;
			bool isValidRequestType = DataType.TryParse (request.DataType, out requestType);
			if (!isValidRequestType)
			{
				return new UnknownRequestTypeHandler();
			}

			switch (requestType) 
			{
			case DataType.Command:
				return new CommandHandler (logger, sensorRepository);
			case DataType.GetValue:
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

