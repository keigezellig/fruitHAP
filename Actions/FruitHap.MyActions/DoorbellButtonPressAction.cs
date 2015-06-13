using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core;
using FruitHAP.Core.Sensor.SensorTypes;
using System.Threading.Tasks;

namespace FruitHap.MyActions
{
	public class DoorbellButtonPressAction : ActionBase
	{
		private readonly ISensorRepository sensorRepository;

		public DoorbellButtonPressAction(ISensorRepository deviceRepository, ILogger logger, IMessageQueueProvider mqProvider):base(mqProvider,logger)
		{
			this.sensorRepository = deviceRepository;
		}


		public override void InitializeFunction ()
		{
			mqProvider.SubscribeToRequest<ButtonPressRequest,ButtonPressResponse> (HandleButtonPress);
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
						IButton doorbellButton = sensorRepository.FindDeviceOfTypeByName<IButton>(request.Name);

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

