using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core;
using FruitHAP.Core.Sensor.SensorTypes;
using System.Threading.Tasks;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHap.MyActions.Messages;

namespace FruitHap.MyActions
{
	public class DoorbellButtonPressAction 
	{

		private readonly ISensorRepository sensoRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqPublisher;

		public DoorbellButtonPressAction(ISensorRepository deviceRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensoRepository = deviceRepository;
			this.logger = logger;
			this.mqPublisher = publisher;
		}


		public void Initialize ()
		{
			mqPublisher.SubscribeToRequest<ButtonPressRequest,ButtonPressResponse> (HandleButtonPress);
		}

		public Task<ButtonPressResponse> HandleButtonPress(ButtonPressRequest request)
		{
			Task<ButtonPressResponse> task = 
				new Task<ButtonPressResponse> (() => 
					{
						logger.DebugFormat("Handling Button press. Request = {0}",request);
						if (request == null)
						{
							return new ButtonPressResponse() {Result = false, Message = "Invalid request"};
						}

						logger.InfoFormat("Looking for button {0}",request.Name);
						IButton doorbellButton = sensoRepository.FindSensorOfTypeByName<IButton>(request.Name);

						if (doorbellButton == null)
						{
							logger.ErrorFormat("Button not found");
							return new ButtonPressResponse() {Result = false, Message = string.Format("Button with name {0} is not defined",request.Name)};
						}

						logger.InfoFormat("Found button: {0}",doorbellButton);
						logger.InfoFormat("Push the button!");
						doorbellButton.PressButton();
						return new ButtonPressResponse() {Result = true};

					});
			task.Start ();
			return task;
			                                
		}



	}
}

