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
			Debug.Log("ResourceOverview: loading settings for "+prefix);
			cfg.load();
		}

		public  void save()
		{
			cfg.save();
			Debug.Log("ResourceOverview: saving settings for " +prefix);
		}

		public  object get(string name, object def)
		{
			Debug.Log("ResourceOverview: get object: " + prefix + "_" + name);
			return cfg.GetValue<object>(prefix+"_"+name, def);
		}

		public  string get(string name, string def)
		{
			Debug.Log("ResourceOverview: get string: " + prefix + "_" + name);
			return cfg.GetValue<string>(prefix + "_" + name, def);
		}

		public  int get(string name, int def)
		{
			Debug.Log("ResourceOverview: get int: " + prefix + "_" + name);
			return cfg.GetValue<int>(prefix + "_" + name, def);
		}

		public  float get(string name, float def)
		{
			Debug.Log("ResourceOverview: get float: " + prefix + "_" + name);
			return cfg.GetValue<float>(prefix + "_" + name, def);
		}

		public  void set(string name, object val)
		{
			cfg.SetValue(prefix + "_" + name, val);
			Debug.Log(String.Format("ResourceOverview: setting {0}: {1} ({2})", prefix + "_" + name, val, val.GetType()));
			save();
		}
		
	}
}
