﻿using System.Collections.Generic;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SensorType { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}