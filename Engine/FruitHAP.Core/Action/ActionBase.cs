using System;
using Castle.Core.Logging;

namespace FruitHAP.Core.Action
{
	public abstract class ActionBase : IAction
	{
		protected IMessageQueueProvider mqProvider;
		protected ILogger logger;


		public void Initialize()
		{
			logger.Info ("Initializing action..");
			InitializeFunction ();
			logger.Info ("Done initializing action..");
		}
		public abstract void InitializeFunction ();


		protected ActionBase (IMessageQueueProvider mqProvider, ILogger logger)
		{
			this.logger = logger;
			this.mqProvider = mqProvider;
		}
	}
}

