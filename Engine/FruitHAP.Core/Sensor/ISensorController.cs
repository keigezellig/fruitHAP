using System;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorController : IDisposable
    {
        string Name { get; }
        void Start();
        void Stop();
		bool IsStarted {get;}
		void SendData (byte[] data);

		event EventHandler<ControllerDataEventArgs> ControllerDataReceived;

    }

	public class ControllerDataEventArgs : EventArgs
	{
		public byte[] Data { get; set;}
	}
}
