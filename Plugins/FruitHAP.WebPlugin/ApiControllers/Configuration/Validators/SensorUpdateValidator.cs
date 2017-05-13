using System;
using FluentValidation;
using FruitHAP.Plugins.Web.ApiControllers.Configuration;
using FluentValidation.Results;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration.Validators
{

    public class SensorUpdateValidator: AbstractValidator<SensorUpdateDTO> 
    {
        public SensorUpdateValidator() 
        {
            RuleFor(sensor => sensor.Type).NotEmpty().WithMessage("Sensor type cannot be empty");
            RuleFor(sensor => sensor.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(sensor => sensor.DisplayName).NotEmpty().WithMessage("Display name cannot be empty");
            RuleFor(sensor => sensor.Category).NotNull().WithMessage("Category cannot be null");
            RuleFor(sensor => sensor.Description).NotNull().WithMessage("Description cannot be null");
            RuleFor(sensor => sensor.Parameters).SetCollectionValidator(new SensorParameterDTOValidator());
        }
    }

    public class SensorParameterDTOValidator : AbstractValidator<SensorParameterDTO> 
    {
        public SensorParameterDTOValidator()
        {
            RuleFor(param => param.Name).NotEmpty().WithMessage("Parameter cannot be empty");
            RuleFor(param => param.Value).NotEmpty().WithMessage("Value cannot be empty");
            Custom(param =>
                {
                    //For now assume everything is an int when its not a string;
                    if (param.Type != "String")
                    {
                        int result;
                        if (!(param.Value.StartsWith("0x")) && (!Int32.TryParse(param.Value, out result)))
                        {
                            return new ValidationFailure("Value","Illegal value for an integer");
                        }
                            
                    }
                    return null;
                });
        }

    }

    
}

