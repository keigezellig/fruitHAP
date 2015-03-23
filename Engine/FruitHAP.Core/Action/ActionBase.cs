using System;
using Castle.Core.Logging;

namespace FruitHAP.Core.Action
{
	public abstract class ActionBase : IAction
	{
		protected IMessageQueueProvider mqProvider;
		protected ILogger logger;

		public abstract void Initialize ();


		protected ActionBase (IMessageQueueProvider mqProvider, ILogger logger)
		{
			this.logger = logger;
			this.mqProvider = mqProvider;
		}
	}
}

