﻿using PluginBaseFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ResourceOverview
{
	class SettingsWindow : BaseWindow
	{
		protected bool showDryMass = true, showTotalMass = true, showCrewCapacity = true, showPartCount = true, showTWR = false, showToolbar = true, showAppLauncher = true;


		public SettingsWindow(): base("Resource Overview Settings", 200, 160)
		{

		}

		public void Start()
		{
			LogDebug("SettingsWindow start");
			

			showTotalMass = KSPSettings.get("showTotalMass", true);
			showDryMass = KSPSettings.get("showDryMass", true);
			showCrewCapacity = KSPSettings.get("showCrewCapacity", true);
			showPartCount = KSPSettings.get("showPartCount", true);
			showTWR = KSPSettings.get("showTWR", false);
			showToolbar = KSPSettings.get("showToolbar", false);
			showAppLauncher = KSPSettings.get("showAppLauncher", true);

			windowPosition.x = KSPSettings.get("SettingsWindow.x", (int)(Screen.width / 2 - windowWidth / 2));
			windowPosition.y = KSPSettings.get("SettingsWindow.y", (int)(Screen.height / 2 - windowHeight / 2));
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
			showTWR = GUILayout.Toggle(showTWR, "Show TWR");
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
			KSPSettings.set("showTotalMass", showTotalMass);
			KSPSettings.set("showDryMass", showDryMass);
			KSPSettings.set("showCrewCapacity", showCrewCapacity);
			KSPSettings.set("showPartCount", showPartCount);
			KSPSettings.set("showTWR", showTWR);
			KSPSettings.set("showToolbar", showToolbar);
			KSPSettings.set("showAppLauncher", showAppLauncher);

			// TODO: error if toolbar AND applauncher disabled

			KSPSettings.set("SettingsWindow.x", (int)windowPosition.x);
			KSPSettings.set("SettingsWindow.y", (int)windowPosition.y);

			KSPSettings.save();
		}
	}
}
