using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class SensorUpdateDTO
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public List<SensorParameterDTO> Parameters { get; set; }
    }

    public class SensorParameterDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}

