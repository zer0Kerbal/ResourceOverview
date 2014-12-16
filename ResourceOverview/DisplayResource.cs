using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceOverview
{
	class DisplayResource
	{

		public double amount;
		public double maxAmount;
		public string name;
		public float density;

		public DisplayResource(string name, double amount, double maxAmount, float density)
		{
			this.name = name;
			this.amount = amount;
			this.maxAmount = maxAmount;
			this.density = density;
		}
	}
}
