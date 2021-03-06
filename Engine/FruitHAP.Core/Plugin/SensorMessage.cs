﻿using System;

namespace FruitHAP.Core.Plugin
{	
	public class SensorMessage
	{
		public DateTime TimeStamp {get; set;}
		public string SensorName { get; set;}
		public object Data {get; set;}

	    public override string ToString ()
		{
			return string.Format ("[SensorMessage: TimeStamp={0}, SensorName={1}, Data={2}", TimeStamp, SensorName, Data);
		}
		
	}


}

