﻿using PluginBaseFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class ResourceOverview : PluginBase
    {
        private IButton roButton;
		private ApplicationLauncherButton appLauncherButton = null;
		
		private ResourceWindow roWindow;
		
		private String pluginPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public void Start()
        {
			Log("start");
			KSPSettings.load();

			// TODO: add settingsChanged listener to add/remove toolbar/applauncher

			roWindow = gameObject.AddComponent<ResourceWindow>();
			
            if (KSPSettings.get("showToolbar", false) && ToolbarManager.ToolbarAvailable) 
            {
				LogDebug("add toolbar button");
                roButton = ToolbarManager.Instance.add("RO", "ROButton");
                roButton.TexturePath = pluginPath + "/icons/ro_toolbar_button";
                roButton.ToolTip = "Resource Overview Window";
                roButton.OnClick += (e) =>
                {
					roWindow.windowVisible = !roWindow.windowVisible;
                };
            }
            if(KSPSettings.get("showAppLauncher", true))
            {
				GameEvents.onGUIApplicationLauncherReady.Add(onGUIAppLauncherReady);
				GameEvents.onGUIApplicationLauncherDestroyed.Add(onGUIAppLauncherDestroyed);
            }
			
        }



		private void onGUIAppLauncherDestroyed()
		{
			LogDebug("onGUIAppLauncherDestroyed");
			if (appLauncherButton != null)
			{
				LogDebug("removing app launcher button from onGUIAppLauncherDestroyed");
				ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton); 
			}
		}


		private void onGUIAppLauncherReady()
		{
			LogDebug("onGUIAppLauncherReady");
			if (appLauncherButton == null)
			{
				LogDebug("onGUIAppLauncherReady adding button");
				Texture2D btnTexture = new Texture2D(38, 38);
				try{
					btnTexture.LoadImage(System.IO.File.ReadAllBytes(pluginPath + "/icons/ro_app_button.png"));
				}
				catch(Exception ex){
					Log("Couldn't load texture for AppLauncher button: " +pluginPath +"/icons/ro_app_button.png")
				}

				appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
					onAppLaunchToggleOn, onAppLaunchToggleOff,
					onAppLaunchHoverOn, onAppLaunchHoverOff,
					null, null,
					ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
					(Texture)btnTexture);
			}
		}


		public void Update()
		{

		}

		void OnDestroy()
		{
			Log("destroy");
		
			if (KSPSettings.get("showToolbar", false) && ToolbarManager.ToolbarAvailable)
			{
				roButton.Destroy();
			}
			if (KSPSettings.get("showAppLauncher", true))
			{
				if (appLauncherButton != null)
				{
					Log("removing app launcher button");
					ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
				}
				GameEvents.onGUIApplicationLauncherDestroyed.Remove(onGUIAppLauncherDestroyed);
				GameEvents.onGUIApplicationLauncherReady.Remove(onGUIAppLauncherReady);
			}
		}

		private void onAppLaunchHoverOn()
		{
			roWindow.windowHover = true;
		}

		private void onAppLaunchHoverOff()
		{
			roWindow.windowHover = false;
		}

		private void onAppLaunchToggleOn()
		{
			roWindow.windowVisible = !roWindow.windowVisible;
		}

		private void onAppLaunchToggleOff()
		{
			roWindow.windowVisible = !roWindow.windowVisible;
		}

	}
}