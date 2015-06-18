using System;
using FruitHAP.Core.Sensor.Controllers;

namespace FruitHAP.Controller.Rfx
{
	public class NewRfxController : IACController, IDefaultController
	{
		public event EventHandler<ControllerDataEventArgs> ControllerDataReceived;

		public void SendData (byte[] data)
		{
			throw new NotImplementedException ();
		}

		#region IACController implementation

		public event EventHandler<FruitHAP.Sensor.Protocols.ACProtocol.ACProtocolEventArgs> ACDataReceived;

		public void SendACData (FruitHAP.Sensor.Protocols.ACProtocol.ACProtocolData data)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region ISensorController implementation

		public void Start ()
		{
			throw new NotImplementedException ();
		}

		public void Stop ()
		{
			throw new NotImplementedException ();
		}

		public string Name {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsStarted {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}

		#endregion

		public NewRfxController ()
		{
		}
	}
}

