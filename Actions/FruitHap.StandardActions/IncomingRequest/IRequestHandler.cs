using System;

using FruitHAP.Core.Action;

namespace FruitHap.StandardActions
{
	public interface IRequestHandler
	{
		SensorMessage HandleRequest(SensorMessage request);
	}
}

