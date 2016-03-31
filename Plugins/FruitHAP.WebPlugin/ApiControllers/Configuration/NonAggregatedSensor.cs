﻿using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class NonAggregatedSensor : SensorConfigurationItem
    {
        private Dictionary<string,object> parameters;

        public NonAggregatedSensor(string name, string description, string category, string type, Dictionary<string,string> operations, Dictionary<string,object> parameters) : base(name, description, category, type, operations)
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

