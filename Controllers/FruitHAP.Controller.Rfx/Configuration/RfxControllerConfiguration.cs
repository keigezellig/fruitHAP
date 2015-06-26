using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Controller.Rfx.Configuration
{
    public class RfxControllerConfiguration
    {
        public string ConnectionString { get; set; }
		public List<PacketType> PacketTypes { get; set; }
    }

	public class PacketType 
	{
		public string Name { get; set; }
		public byte Id { get; set; }
		public byte Length { get; set; }
		public List<SubPacketType> SubTypes { get; set; }

	}

	public class SubPacketType
	{
		public string Name { get; set; }
		public byte Id {get; set;}
		public bool IsEnabled{ get; set; }
		public string SensitivityFlag {get; set;}

	}
}
