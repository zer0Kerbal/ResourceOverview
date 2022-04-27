using PluginBaseFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using ToolbarControl_NS;
using static ResourceOverview.RegisterToolbar;

namespace ResourceOverview
{

    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    partial class ResourceOverview : BaseWindow
    {
        internal static ResourceOverview Instance;
        private static ToolbarControl toolbarControl = null;

        internal const string MODID = "ResourceOveriew";
        internal const string MODNAME = "Resource Overiew";


        public void Start()
        {
            Log.Info("Start");
            Instance = this;

            if (toolbarControl == null)
            {
                Log.Info("Initting toolbarControl");
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(
                    onAppLaunchToggle, onAppLaunchToggle,
                    onAppLaunchHoverOn, onAppLaunchHoverOff,
                    null, null,
                    ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.FLIGHT,
                    MODID,
                    "RO_Btn",
                    "ResourceOverview/PluginData/ro_app_button_active.png",
                    "ResourceOverview/PluginData/ro_app_button.png",
                    "ResourceOverview/PluginData/ro_toolbar_button_active",
                    "ResourceOverview/PluginData/ro_toolbar_button",
                    MODNAME);
            }

            KSPSettings.load();
            KSPSettings.SettingsChanged += new KSPSettings.SettingsChangedEventHandler(onSettingsChanged);

            GameEvents.onPartRemove.Add(onPartRemove);
            if (HighLogic.LoadedSceneIsEditor)
            {
                GameEvents.onEditorShipModified.Add(onEditorShipModified);
            }
            if (HighLogic.LoadedSceneIsFlight)
                SetUpUpdateCoroutine();
        }

        private void onAppLaunchHoverOn()
        {
            windowHover = true;
        }

        private void onAppLaunchHoverOff()
        {
            windowHover = false;
        }

        private void onAppLaunchToggle()
        {
            windowVisible = !windowVisible;
        }
    }
}