using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class AggregatedSensor : SensorConfigurationItem
    {
        private List<string> inputs;

        public AggregatedSensor(string name, string description, string category, string type, string valueType, Dictionary<string,string> operations, List<string> inputs) : base(name, description, category, type, valueType, operations)
        {
            this.inputs = inputs;
        }

        public List<string> Inputs
        {
            get
            {
                return inputs;
            }
        }
    }
}

