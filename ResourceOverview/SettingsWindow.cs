using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	class SettingsWindow : BaseWindow
	{
		public Settings settings;
		protected bool showDryMass = true, showTotalMass = true, showCrewCapacity = true, showPartCount = true;


		public SettingsWindow(): base("Resource Overview Settings", 200, 150)
		{
			settings = new Settings("ResourceOverview");
			settings.load();
		}

		public void Start()
		{
			LogDebug("SettingsWindow start");
			

			showTotalMass = settings.get("showTotalMass", true);
			showDryMass = settings.get("showDryMass", true);
			showCrewCapacity = settings.get("showCrewCapacity", true);
			showPartCount = settings.get("showPartCount", true);

			windowPosition.x = settings.get("SettingsWindow.x", (int)(Screen.width / 2 - windowWidth / 2));
			windowPosition.y = settings.get("SettingsWindow.y", (int)(Screen.height / 2 - windowHeight / 2));
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
			showPartCount = GUILayout.Toggle(showPartCount, "Show Part Couht");

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
			settings.set("showTotalMass", showTotalMass);
			settings.set("showDryMass", showDryMass);
			settings.set("showCrewCapacity", showCrewCapacity);
			settings.set("showPartCount", showPartCount);

			settings.set("SettingsWindow.x", (int)windowPosition.x);
			settings.set("SettingsWindow.y", (int)windowPosition.y);

			settings.save();
		}
	}
}
