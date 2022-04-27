using UnityEngine;
using System.IO;
using SpaceTuxUtility;
using static ResourceOverview.RegisterToolbar;

namespace ResourceOverview
{
	class KSPSettings 
	{
		static internal  bool
			showDryMass = true,
			showTotalMass = true,
			showCrewCapacity = true,
			showPartCount = true,
			showTWR = false,
			useStockSkin = false;


		public delegate void SettingsChangedEventHandler();
		public static event SettingsChangedEventHandler SettingsChanged;

		internal static readonly string CFG_PATH = "/GameData/ResourceOverview/PluginData/";
		static readonly string CFG_FILE = CFG_PATH + "ResourceOverview.cfg";
		internal static readonly string DISPLAYINFO_NODENAME = "ResourceOverview";

		static public void SaveData()
		{
			string fullPath = KSPUtil.ApplicationRootPath + CFG_FILE;
			var configFile = new ConfigNode();
			var configFileNode = new ConfigNode(DISPLAYINFO_NODENAME);

			configFileNode.AddValue("showTotalMass", showTotalMass);
			configFileNode.AddValue("showDryMass", showDryMass);
			configFileNode.AddValue("showCrewCapacity", showCrewCapacity);
			configFileNode.AddValue("showPartCount", showPartCount);
			configFileNode.AddValue("showTWR", showTWR);
			configFileNode.AddValue("useStockSkin", useStockSkin);

			configFileNode.AddValue("SettingsWindowx", SettingsWindow.Instance.windowPosition.x);
			configFileNode.AddValue("SettingsWindowy", SettingsWindow.Instance.windowPosition.y);

			configFileNode.AddValue("Windowx", ResourceOverview.Instance.windowPosition.x);
			configFileNode.AddValue("Windowy", ResourceOverview.Instance.windowPosition.y);

			configFile.AddNode(configFileNode);
			configFile.Save(fullPath);
		}

		static public void LoadData()
		{
			string fullPath = KSPUtil.ApplicationRootPath + CFG_FILE;
			Log.Info("LoadData, fullpath: " + fullPath);
			if (File.Exists(fullPath))
			{
				Log.Info("file exists");
				var configFile = ConfigNode.Load(fullPath);
				if (configFile != null)
				{
					Log.Info("configFile loaded");
					var configFileNode = configFile.GetNode(DISPLAYINFO_NODENAME);
					if (configFileNode != null)
					{
						Log.Info("configFileNode loaded");
						showTotalMass = configFileNode.SafeLoad("showTotalMass", showTotalMass);
						showDryMass = configFileNode.SafeLoad("showDryMass", showDryMass);
						showCrewCapacity = configFileNode.SafeLoad("showCrewCapacity", showCrewCapacity);
						showPartCount = configFileNode.SafeLoad("showPartCount", showPartCount);
						showTWR = configFileNode.SafeLoad("showTWR", showTWR);
						useStockSkin = configFileNode.SafeLoad("useStockSkin", useStockSkin);

						if (SettingsWindow.Instance != null)
						{
							SettingsWindow.Instance.windowPosition.x = configFileNode.SafeLoad("SettingsWindowx", (Screen.width / 2 - SettingsWindow.Instance.windowWidth / 2));
							SettingsWindow.Instance.windowPosition.y = configFileNode.SafeLoad("SettingsWindowy", (Screen.height / 2 - SettingsWindow.Instance.windowHeight / 2));
						}
						if (ResourceOverview.Instance != null)
						{
							ResourceOverview.Instance.windowPosition.x = configFileNode.SafeLoad("Windowx", (Screen.width / 2 - ResourceOverview.Instance.windowWidth / 2));
							ResourceOverview.Instance.windowPosition.y = configFileNode.SafeLoad("Windowy", (Screen.height / 2 - ResourceOverview.Instance.windowHeight / 2));
						}
					}
				}
			}
		}

		public static void load()
		{
			LoadData();
		}

		public static void save()
		{
			SaveData();
			if (SettingsChanged != null)
			{
				SettingsChanged();
			}
		}		
	}
}
