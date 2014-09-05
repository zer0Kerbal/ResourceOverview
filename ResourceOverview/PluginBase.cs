using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	class PluginBase : MonoBehaviour
	{

		[System.Diagnostics.Conditional("DEBUG")]
		protected void LogDebug(String msg)
		{
			Log(msg);
		}
		protected void Log(String msg)
		{
			Debug.Log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ": " + msg);
		}
	}
}
