using System;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;

namespace FruitHAP.Core.Sensor
{
	public interface IAggregatedSensor : ISensor
	{
		void Initialize(List<ISensor> inputs);
		List<ISensor> Inputs {get;}
	}
}

