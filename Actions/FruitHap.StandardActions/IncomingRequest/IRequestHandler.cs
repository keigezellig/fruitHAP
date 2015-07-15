using System;
using FruitHap.StandardActions.Messages;

namespace FruitHap.StandardActions
{
	public interface IRequestHandler
	{
		SensorMessage HandleRequest(SensorMessage request);
	}
}

