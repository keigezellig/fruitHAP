using System;

namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    public class SensorConfigurationItem
    {
        protected string name;
        protected string description;
        protected string category;
        protected string type; 

        protected SensorConfigurationItem(string name, string description, string category, string type)
        {
            this.name = name;
            this.description = description;
            this.category = category;
            this.type = type;
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

    }
}

