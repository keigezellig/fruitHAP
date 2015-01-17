using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorProcessing.Common
{
    public interface IBinding : IDisposable
    {
        void Start();
        void Stop();
    }
}
