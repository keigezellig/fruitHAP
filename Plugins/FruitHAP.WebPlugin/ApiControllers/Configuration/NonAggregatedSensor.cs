using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class NonAggregatedSensor : SensorConfigurationItem
    {
        private Dictionary<string,object> parameters;

        public NonAggregatedSensor(string name, string displayName, string description, string category, string type, string valueType, Dictionary<string,string> operations, Dictionary<string,object> parameters) : base(name, displayName, description, category, type, valueType, operations)
        {
            this.parameters = parameters;
        }

        public object Parameters
        {
            get
            {
                return this.parameters;
            }
        }


    }
}

