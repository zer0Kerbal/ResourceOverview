using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using KSP.IO;

namespace ResourceOverview
{
	class Settings : PluginBase
	{

		public delegate void SettingsChangedEventHandler();
		public static event SettingsChangedEventHandler SettingsChanged;

		protected static PluginConfiguration cfg = PluginConfiguration.CreateForType<Settings>();

		public static void load()
		{
			cfg.load();
		}

		public static void save()
		{
			cfg.save();
			if (SettingsChanged != null)
			{
				SettingsChanged();
			}
		}

		public static object get(string name, object def)
		{
			return cfg.GetValue<object>(name, def);
		}

		public static string get(string name, string def)
		{
			return cfg.GetValue<string>(name, def);
		}

		public static int get(string name, int def)
		{
			return cfg.GetValue<int>(name, def);
		}

		public static float get(string name, float def)
		{
			return cfg.GetValue<float>(name, def);
		}

		public static bool get(string name, bool def)
		{
			return cfg.GetValue<bool>(name, def);
		}

		public static void set(string name, object val)
		{
			cfg.SetValue(name, val);
		}
		
	}
}
