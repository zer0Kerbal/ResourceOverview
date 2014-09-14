using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	class Settings : PluginBase
	{
		
		protected ConfigNode mainNode;
		protected string filePath = "";
		
		public void Load()
		{
			mainNode = ConfigNode.Load(filePath);
		}

		
	}
}
