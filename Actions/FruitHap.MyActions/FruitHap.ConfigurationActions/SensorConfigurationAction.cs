using System;
using FruitHAP.Core.Action;
using FruitHAP.Core;
using Castle.Core.Logging;
using System.Collections.Generic;
using FluentValidation;
using FruitHap.ConfigurationActions.Messages;
using FruitHap.ConfigurationActions.Validators;
using System.Threading.Tasks;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Common.Configuration;
using FruitHAP.Core.SensorConfiguration;

namespace FruitHap.ConfigurationActions
{
	public class SensorConfigurationAction : ActionBase
	{	
		private IValidator<GetSensorListRequest> getSensorListRequestValidator;
		private ISensorConfigurationRepository sensorConfigurationRepository;

		public SensorConfigurationAction (IMessageQueueProvider mqProvider, ISensorConfigurationRepository sensorConfigurationRepository, ILogger logger) : base (mqProvider, logger)
		{
			this.sensorConfigurationRepository = sensorConfigurationRepository;
			getSensorListRequestValidator = new GetSensorListRequestValidator ();
		}

		public override void InitializeFunction ()
		{

			mqProvider.SubscribeToRequest<GetSensorListRequest,ConfigurationResponse> (HandleGetSensorListRequest);
			mqProvider.SubscribeToRequest<GetSensorTypesRequest,ConfigurationResponse> (HandleGetSensorTypesRequest);
		}


		private Task<ConfigurationResponse> HandleGetSensorListRequest (GetSensorListRequest req)
		{
			return HandleConfigurationRequest (req, getSensorListRequestValidator, (request) => sensorConfigurationRepository.GetSensorList ());
		}

		private Task<ConfigurationResponse> HandleGetSensorTypesRequest (GetSensorTypesRequest req)
		{
			return HandleConfigurationRequest (req, null, (request) => sensorConfigurationRepository.GetSensorTypes ());
		}			

		private Task<ConfigurationResponse> HandleConfigurationRequest<TRequest>(TRequest request, IValidator validator, Func<TRequest, object> processFunction)
		{
			Task<ConfigurationResponse> task = 
				new Task<ConfigurationResponse> (() => 
					{
						if (validator != null)
						{
							var validationResult = validator.Validate(request);
							if (!validationResult.IsValid)
							{
								string err = string.Concat(validationResult.Errors.Select(f=> f.ErrorMessage));
								return new ConfigurationResponse () {Result = Result.NotOk, FaultReason = FaultReason.ParameterError, FaultMessage=err};
							}
							else
							{
								var result = processFunction(request);
								return new ConfigurationResponse {Result = Result.Ok,ResultData = result};
							}
						}

						else
						{
							var result = processFunction(request);
							return new ConfigurationResponse {Result = Result.Ok,ResultData = result};


						}
					});
			task.ContinueWith ((originalTask) => 
				{
					//HandleTaskExceptions(originalTask);
					return new ConfigurationResponse {Result = Result.NotOk, FaultReason = FaultReason.InternalError, FaultMessage = "Internal error while handling request"};
				},TaskContinuationOptions.OnlyOnFaulted);

			task.Start ();
			return task;

		}


		private void HandleTaskExceptions(Task originalTask)
		{
			if(originalTask.Exception != null)
			{
				originalTask.Exception.Handle(x =>
					{                   
						logger.Error("Exception occured in handling request :", x);
						return true;
					});
			}
		}		
	}
}

