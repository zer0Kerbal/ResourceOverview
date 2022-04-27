using PluginBaseFramework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpaceTuxUtility;
using static ResourceOverview.RegisterToolbar;


namespace ResourceOverview
{
    class SettingsWindow : BaseWindow
    {
        internal static SettingsWindow Instance;
        public SettingsWindow() : base("Resource Overview Settings", 200, 160)
        {

        }

        public void Start()
        {
            Log.Debug("SettingsWindow start");
            Instance = this;
            _windowVisible = true;
            KSPSettings.load();
        }

        protected override void preDrawGui()
        {

        }

        protected override void drawGui(int windowID)
        {
            GUILayout.BeginVertical();
            KSPSettings.showTotalMass = GUILayout.Toggle(KSPSettings.showTotalMass, "Show Total Mass");
            KSPSettings.showDryMass = GUILayout.Toggle(KSPSettings.showDryMass, "Show Dry Mass");
            KSPSettings.showCrewCapacity = GUILayout.Toggle(KSPSettings.showCrewCapacity, "Show Crew Capacity");
            KSPSettings.showPartCount = GUILayout.Toggle(KSPSettings.showPartCount, "Show Part Count");
            KSPSettings.showTWR = GUILayout.Toggle(KSPSettings.showTWR, "Show TWR");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                saveSettings();
            }
            if (GUILayout.Button("Close"))
            {
                windowVisible = false;
                Destroy(this);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public void OnDestroy()
        {
            Log.Debug("SettingsWindow destroy");
            saveSettings();
        }

        protected void saveSettings()
        {
            KSPSettings.save();
        }
    }
}
