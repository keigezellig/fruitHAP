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

		public override void Initialize ()
		{
			mqProvider.SubscribeToRequest<GetSensorListRequest,ConfigurationResponse> (HandleGetSensorListRequest);
			mqProvider.SubscribeToRequest<GetSensorTypesRequest,ConfigurationResponse> (HandleGetSensorTypesRequest);
		}


		private Task<ConfigurationResponse> HandleGetSensorListRequest (GetSensorListRequest req)
		{
			Task<ConfigurationResponse> task = 
				new Task<ConfigurationResponse> (() => 
					{
						var validationResult = getSensorListRequestValidator.Validate(req);
						if (!validationResult.IsValid)
						{
							string err = string.Concat(validationResult.Errors.Select(f=> f.ErrorMessage));
							return new ConfigurationResponse () {Result = Result.NotOk, FaultReason = FaultReason.ParameterError, FaultMessage=err};
						}
						else
						{
							var sensorList = sensorConfigurationRepository.GetSensorList();
							return new ConfigurationResponse {Result = Result.Ok,ResultData = sensorList};

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

		private Task<ConfigurationResponse> HandleGetSensorTypesRequest (GetSensorTypesRequest req)
		{
			Task<ConfigurationResponse> task = 
				new Task<ConfigurationResponse> (() => 
					{
						{
							var sensorTypes = sensorConfigurationRepository.GetSensorTypes();
							return new ConfigurationResponse {Result = Result.Ok,ResultData = sensorTypes};

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

