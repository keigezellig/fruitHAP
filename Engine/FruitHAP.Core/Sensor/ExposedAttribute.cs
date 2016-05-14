using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExposedAttribute : Attribute
    {
        private bool isExposed;
        public bool IsExposed 
        { 
            get
            {
                return isExposed;
            }
        }

        public ExposedAttribute(bool isExposed)
        {
            this.isExposed = isExposed;
        }
    }
}
