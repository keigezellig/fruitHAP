using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class SensorParameterItem
    {
        public string Parameter { get; set; }
        public string Type { get; set; }
        public List<AllowedValueItem> allowedValues;     
    }

    public class AllowedValueItem
    {
        public string Name { get; set;}
        public int Value { get; set;}
    }
}

