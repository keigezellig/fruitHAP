using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Common.PhysicalInterfaces.SerialPortInterface
{
    public sealed class SerialPortPhysicalInterface : IPhysicalInterface
    {
        private readonly ISerialPort serialPort;
        private readonly ILogger log;        
        private CancellationTokenSource ctSource;
        private Task readerTask;

        public SerialPortPhysicalInterface(ISerialPort serialPort, ILogger log)
        {
            this.serialPort = serialPort;
            this.log = log;
        }

        public event EventHandler<ExternalDataReceivedEventArgs> DataReceived;

        public void StartReading()
        {
            SetupTask();
            readerTask.Start();
            log.Info("Read task started");
        }

        public void StopReading()
        {
            try
            {
                if(ctSource != null)
                {
                    ctSource.Cancel();
                    readerTask.Wait();
                }
            }
            catch(AggregateException ex)
            {
                ex.Handle(x =>
                {
                    if(x is TaskCanceledException)
                    {
                        log.Info("Reading from serial port has been stopped");
                        return true;
                    }                   
                    return false;
                });
            }
        }

        public void Open()
        {
            if(IsOpen()) return;

            try
            {
                log.Info(string.Format("Opening serial port {0} ", serialPort.PortName));
                serialPort.Open();
            }
            catch (IOException ex)
            {
                log.Error("Exception occured when opening the serial port", ex);
                throw;
            }
            
        }

        public void Close()
        {
            StopReading();
        }

        public void Write(byte[] data)
        {
            log.Info(string.Format("Written data to serial port: {0}",PrettyPrint(data)));
            serialPort.Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            log.Info("Dispose called");

            Close();

            if(serialPort != null)
            {
                serialPort.Dispose();
            }
            if(ctSource != null)
            {
                ctSource.Dispose();    
            }
            if (readerTask != null)
            {
                readerTask.Dispose();
            }
        }

        private void SetupTask()
        {
            ctSource = new CancellationTokenSource();
            CancellationToken cancellationToken = ctSource.Token;

            readerTask = new Task(() => StartReading(cancellationToken),cancellationToken);

            readerTask.ContinueWith(originalTask =>
            {
                if(originalTask.IsFaulted)
                {
                    HandleTaskExceptions(originalTask);                    
                }
                
                if(originalTask.IsCanceled)
                {                    
                    log.Debug("Reader task stopped");                    
                }
                
            }, TaskContinuationOptions.NotOnRanToCompletion);

            log.Debug("Read task created");

        }

        private void HandleTaskExceptions(Task originalTask)
        {
            if(originalTask.Exception != null)
            {
                originalTask.Exception.Handle(x =>
                {                   
                    log.Error("Exception occured in reading from the serial port:", x);
                    return true;
                });
            }
        }

        private void StartReading(CancellationToken ct)
        {            
            ct.ThrowIfCancellationRequested();

            while (true)
            {
                if(ct.IsCancellationRequested)
                {
                    log.Debug(string.Format("Closing serial port {0} ", serialPort.PortName));
                    serialPort.Close();
                    ct.ThrowIfCancellationRequested();
                }
                
                ReadDataFromPort();
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            // ReSharper disable once FunctionNeverReturns, this function is executed in a task
        }

        private void ReadDataFromPort()
        {
            if(serialPort.BytesToRead != 0)
            {
                log.Debug(string.Format("Bytes to read: {0} ", serialPort.BytesToRead));
                byte[] buffer = serialPort.ReadBytesToEnd();
     
                if(buffer != null)
                {
                    log.DebugFormat("Bytes received: {0}",buffer.BytesAsString());
                    OnDataReceived(buffer);
                }
            }            
        }

        private bool IsOpen()
        {
            try
            {
                log.Debug(string.Format("Checking if serial port is open {0} ", serialPort.PortName));
                return serialPort.IsOpen;
            }
            catch (IOException ex)
            {
                log.Error("Exception occured when checking serial port was open or not.", ex);
            }
            return false;
        }

        private void OnDataReceived(byte[] data)
        {
            var eventHandler = DataReceived;
            if(eventHandler != null)
            {
                eventHandler(this, new ExternalDataReceivedEventArgs {Data = data});
            }
        }

        private string PrettyPrint(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            if (data != null)
            {
                foreach (var b in data)
                {
                    sb.AppendFormat("{0:X} ", b);
                }
            }
            return sb.ToString();
        }
 
    }



}


