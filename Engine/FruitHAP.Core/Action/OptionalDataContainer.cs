using System;

namespace FruitHap.Core.Action
{
	public class OptionalDataContainer
	{
		public object Content { get; set; }

		public OptionalDataContainer (object content)
		{			
			Content = content;
		}
	}
}

