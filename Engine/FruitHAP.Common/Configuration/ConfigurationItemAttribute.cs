using System;

namespace FruitHAP.Common.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationItemAttribute : Attribute
    {
        public bool IsSensorSpecific { get; set; }

        public override string ToString()
        {
            return string.Format("[ConfigurationItemAttribute]");
        }
    }
}

