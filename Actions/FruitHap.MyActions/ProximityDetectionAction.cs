using System;
using FruitHAP.Core.Action;
using FruitHAP.Core;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHap.MyActions.Messages;

namespace FruitHap.MyActions
{
	public class ProximityDetectionAction : IAction
	{
		private readonly ISensorRepository sensoRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqPublisher;


		#region IAction implementation

		public void Initialize ()
		{
			logger.InfoFormat ("Initializing action {0}", this);
			var proximityDetector = sensoRepository.FindDeviceOfTypeByName<IReadOnlySwitch>("ProximityDetector");
			proximityDetector.StateChanged += ProximityDetector_StateChanged;
		}

		void ProximityDetector_StateChanged (object sender, SwitchEventArgs e)
		{

			if (e.NewState == SwitchState.On) {
				var message = new ProximityDetectionMessage () { DetectorName = (sender as ISensor).Name, Timestamp = DateTime.Now};
				try {
					logger.Info ("Send notification");
					mqPublisher.Publish (message, "alerts");
				} 
				catch (Exception ex) 
				{
					logger.Error ("Error sending notification", ex);
				}
			}
		}


		#endregion

		public ProximityDetectionAction (ISensorRepository deviceRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensoRepository = deviceRepository;
			this.logger = logger;
			this.mqPublisher = publisher;
		}

		}
	}


