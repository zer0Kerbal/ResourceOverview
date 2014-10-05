using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	class SettingsWindow : BaseWindow
	{
		protected bool showDryMass = true, showTotalMass = true, showCrewCapacity = true, showPartCount = true, showToolbar = true, showAppLauncher = true;


		public SettingsWindow(): base("Resource Overview Settings", 200, 160)
		{

		}

		public void Start()
		{
			LogDebug("SettingsWindow start");
			

			showTotalMass = Settings.get("showTotalMass", true);
			showDryMass = Settings.get("showDryMass", true);
			showCrewCapacity = Settings.get("showCrewCapacity", true);
			showPartCount = Settings.get("showPartCount", true);
			showToolbar = Settings.get("showToolbar", false);
			showAppLauncher = Settings.get("showAppLauncher", true);

			windowPosition.x = Settings.get("SettingsWindow.x", (int)(Screen.width / 2 - windowWidth / 2));
			windowPosition.y = Settings.get("SettingsWindow.y", (int)(Screen.height / 2 - windowHeight / 2));
		}

		protected override void preDrawGui()
		{
			
		}

		protected override void drawGui(int windowID)
		{
			GUILayout.BeginVertical();
			showTotalMass = GUILayout.Toggle(showTotalMass, "Show Total Mass");
			showDryMass = GUILayout.Toggle(showDryMass, "Show Dry Mass");
			showCrewCapacity = GUILayout.Toggle(showCrewCapacity, "Show Crew Capacity");
			showPartCount = GUILayout.Toggle(showPartCount, "Show Part Count");
			showToolbar = GUILayout.Toggle(showToolbar, "Enable Toolbar");
			showAppLauncher = GUILayout.Toggle(showAppLauncher, "Enable AppLauncher (stock)");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				saveSettings();
			}
			if (GUILayout.Button("Close"))
			{
				windowVisible = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		public void OnDestroy()
		{
			LogDebug("SettingsWindow destroy");
			saveSettings();
		}

		protected void saveSettings()
		{
			Settings.set("showTotalMass", showTotalMass);
			Settings.set("showDryMass", showDryMass);
			Settings.set("showCrewCapacity", showCrewCapacity);
			Settings.set("showPartCount", showPartCount);
			Settings.set("showToolbar", showToolbar);
			Settings.set("showAppLauncher", showAppLauncher);

			// TODO: error if toolbar AND applauncher disabled

			Settings.set("SettingsWindow.x", (int)windowPosition.x);
			Settings.set("SettingsWindow.y", (int)windowPosition.y);

			Settings.save();
		}
	}
}
