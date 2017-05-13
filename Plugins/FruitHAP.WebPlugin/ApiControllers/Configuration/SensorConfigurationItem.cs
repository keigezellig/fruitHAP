using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class SensorConfigurationItem
    {
        protected string name;
        protected string displayName;
        protected string description;
        protected string category;
        protected string type;
        protected string valueType;
        protected Dictionary<string,string> operations;

 

        protected SensorConfigurationItem(string name, string displayName, string description, string category, string type, string valueType, Dictionary<string,string> operations)
        {
            this.name = name;
            this.displayName = displayName;
            this.description = description;
            this.category = category;
            this.type = type;
            this.operations = operations;
            this.valueType = valueType;
        }


        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string Category
        {
            get
            {
                return this.category;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
        }

        public string ValueType
        {
            get
            {
                return this.valueType;
            }
        }
        public Dictionary<string,string> SupportedOperations
        {
            get
            {
                return this.operations;
            }
        }

    }
}

