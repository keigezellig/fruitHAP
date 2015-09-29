using System;
using FruitHap.Core.Action;

namespace FruitHap.StandardActions
{
	public interface IRequestHandler
	{
		SensorMessage HandleRequest(SensorMessage request);
	}
}

