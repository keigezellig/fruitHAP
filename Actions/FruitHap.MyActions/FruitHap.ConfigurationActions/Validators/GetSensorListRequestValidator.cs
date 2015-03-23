using System;
using FluentValidation;
using FruitHap.ConfigurationActions.Messages;
using System.Collections.Generic;

namespace FruitHap.ConfigurationActions.Validators
{
	public class GetSensorListRequestValidator : AbstractValidator<GetSensorListRequest> 
	{
		public GetSensorListRequestValidator ()
		{
			RuleFor (request => request.SensorType).NotEmpty ().WithMessage ("SensorType cannot be empty. Either specify a sensor type or 'All' if you want all sensors");
		}

	}
}

