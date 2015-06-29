using System;
using FruitHap.StandardActions.Messages.Outbound;

namespace FruitHap.StandardActions
{
	public interface IRequestHandler
	{
		SensorMessage HandleRequest(SensorMessage request);
	}
}

