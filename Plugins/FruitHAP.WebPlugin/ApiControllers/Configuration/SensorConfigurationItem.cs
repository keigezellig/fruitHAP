using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class SensorConfigurationItem
    {
        protected string name;
        protected string description;
        protected string category;
        protected string type;
        protected string valueType;
        protected Dictionary<string,string> operations;

 

        protected SensorConfigurationItem(string name, string description, string category, string type, string valueType, Dictionary<string,string> operations)
        {
            this.name = name;
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

        public Dictionary<string,string> SupportedOperations
        {
            get
            {
                return this.operations;
            }
        }

    }
}

