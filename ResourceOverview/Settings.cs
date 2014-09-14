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

		protected PluginConfiguration cfg = null;
		protected string prefix;

		public Settings(string prefix)
		{
			cfg = PluginConfiguration.CreateForType<Settings>();
			this.prefix = prefix;
		}

		public  void load()
		{
			cfg.load();
		}

		public  void save()
		{
			cfg.save();
		}

		public  object get(string name, object def)
		{
			return cfg.GetValue<object>(prefix+"."+name, def);
		}

		public  string get(string name, string def)
		{
			return cfg.GetValue<string>(prefix + "." + name, def);
		}

		public  int get(string name, int def)
		{
			return cfg.GetValue<int>(prefix + "." + name, def);
		}

		public  float get(string name, float def)
		{
			return cfg.GetValue<float>(prefix + "." + name, def);
		}

		public  void set(string name, object val)
		{
			cfg.SetValue(prefix + "." + name, val);
		}
		
	}
}
