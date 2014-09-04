﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class ResourceOverview : MonoBehaviour
    {
        private IButton roButton;
        private bool roWindowVisible;
		private bool roWindowHover;
		private Rect roWindowPosition;
		private float roWindowHeight;

		private Dictionary<String, DisplayResource> resourceList = new Dictionary<String, DisplayResource>();
		private bool vesselDataFetched = false;
		private float vesselTotalMass;
		private float vesselDryMass;
		private int vesselCrewCapacity;
		private int vesselPartCount;

		private ApplicationLauncherButton appLauncherButton = null;

        public void Start()
        {
			LogDebug("start");
            if (ToolbarManager.ToolbarAvailable) 
            {
                roButton = ToolbarManager.Instance.add("RO", "ROButton");
                roButton.TexturePath = "ResourceOverview/ro_toolbar_button";
                roButton.ToolTip = "Resource Overview Window";
                roButton.OnClick += (e) =>
                {
                    roWindowVisible = !roWindowVisible;
                };
            }
            else
            {
				GameEvents.onGUIApplicationLauncherReady.Add(onGUIAppLauncherReady);
				GameEvents.onGUIApplicationLauncherDestroyed.Add(onGUIAppLauncherDestroyed);
            }
			
			
			GameEvents.onEditorShipModified.Add(onEditorShipModified);
			GameEvents.onPartRemove.Add(onPartRemove);
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
				btnTexture.LoadImage(System.IO.File.ReadAllBytes("GameData/ResourceOverview/ro_app_button.png"));

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
			LogDebug("destroy");

			GameEvents.onEditorShipModified.Remove(onEditorShipModified);
			GameEvents.onPartRemove.Remove(onPartRemove);
			
			if (ToolbarManager.ToolbarAvailable)
			{
				roButton.Destroy();
			}
			else
			{
				if (appLauncherButton != null)
				{
					LogDebug("removing app launcher button from OnDestroy");
					ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
				}
				GameEvents.onGUIApplicationLauncherDestroyed.Remove(onGUIAppLauncherDestroyed);
				GameEvents.onGUIApplicationLauncherReady.Remove(onGUIAppLauncherReady);
			}
		}

		private void onAppLaunchHoverOn()
		{
			roWindowHover = true;
		}

		private void onAppLaunchHoverOff()
		{
			roWindowHover = false;
		}

		private void onAppLaunchToggleOff()
		{
			roWindowVisible = false;
		}

		private void onAppLaunchToggleOn()
		{
			roWindowVisible = true;
		}

        void OnGUI()
        {
			if (roWindowPosition.x == 0 && roWindowPosition.y == 0)
			{
				// set initial size
				roWindowPosition = new Rect(Screen.width - 250, 100, 200, 50);
			}
			if (roWindowVisible || roWindowHover)
			{
				if (EditorLogic.startPod == null) // nothing to display, show only text
				{
					roWindowHeight = 50;
				}
				else // we got something, calculate size
				{
					roWindowHeight = 90 + resourceList.Count * 20;
				}
				roWindowPosition = GUILayout.Window(456123, roWindowPosition, resourceOverviewWindow, "Resource Overview Window",
					GUILayout.Width(200), // overwrite values from roWindowPosition
					GUILayout.Height(roWindowHeight));
			}
        }

		

        private void resourceOverviewWindow(int windowID)
        {
			GUILayout.BeginVertical();

			if (EditorLogic.startPod != null) {
				
				GUILayout.Label("Total Mass: " + String.Format("{0:,0.00}", vesselTotalMass), GUILayout.ExpandWidth(true));
				GUILayout.Label("Dry Mass: " + String.Format("{0:,0.00}", vesselDryMass), GUILayout.ExpandWidth(true));
				GUILayout.Label("Crew Capacity: " + vesselCrewCapacity, GUILayout.ExpandWidth(true));
				GUILayout.Label("Part Count: " + vesselPartCount, GUILayout.ExpandWidth(true));
				GUILayout.Space(10);
				reloadVesselData();
				foreach (String key in resourceList.Keys)
				{
					GUILayout.Label(key + ": " + String.Format("{0:,0.00}", resourceList[key].amount) + " / " + String.Format("{0:,0.00}", resourceList[key].maxAmount), GUILayout.ExpandWidth(true));
				}
				
			}
			if (resourceList.Count == 0)
			{
				GUILayout.Label("No resources to display!", GUILayout.ExpandWidth(true));
			}
			GUILayout.EndVertical();
			GUI.DragWindow();
        }

		
        private void reloadVesselData()
        {
			if (vesselDataFetched)
			{
				return;
			}
			vesselTotalMass = EditorLogic.SortedShipList.Where(p => p.physicalSignificance == Part.PhysicalSignificance.FULL).Sum(p => p.mass + p.GetResourceMass());
			vesselDryMass = EditorLogic.SortedShipList.Where(p => p.physicalSignificance == Part.PhysicalSignificance.FULL).Sum(p => p.mass);
			vesselCrewCapacity = EditorLogic.SortedShipList.Sum(p => p.CrewCapacity);
			vesselPartCount = EditorLogic.SortedShipList.Count;

			foreach (Part part in EditorLogic.SortedShipList)
			{
				LogDebug("getting res for " + part.name);
				foreach (PartResource res in part.Resources.list)
				{
					LogDebug("Resource: " + res.resourceName + ", Amount: " + res.amount);
					if (resourceList.ContainsKey(res.resourceName))
					{
						//res.info.density
						resourceList[res.resourceName].amount += res.amount;
						resourceList[res.resourceName].maxAmount += res.maxAmount;
					}
					else
					{
						resourceList.Add(res.resourceName, new DisplayResource(res.resourceName, res.amount, res.maxAmount));
					}
				}
			}

			//fetchResources(p);

			vesselDataFetched = true;
        }

		private void fetchResources(Part p)
		{
			LogDebug("getting res for " + p.name);
			foreach (PartResource res in p.Resources.list)
			{
				LogDebug("Resource: " + res.resourceName + ", Amount: " + res.amount);
				if (resourceList.ContainsKey(res.resourceName))
				{
					resourceList[res.resourceName].amount += res.amount;
					resourceList[res.resourceName].maxAmount += res.maxAmount;
				}
				else
				{
					resourceList.Add(res.resourceName, new DisplayResource(res.resourceName, res.amount, res.maxAmount));
				}
			}

			foreach (Part ch in p.children)
			{
				fetchResources(ch);
			}
		}

       

		private void onPartRemove(GameEvents.HostTargetAction<Part, Part> data)
		{
			LogDebug("onPartRemove");
			setFetchVesselData();
		}


		void onEditorShipModified(ShipConstruct sc)
		{
			LogDebug("onEditorShipModified");
			setFetchVesselData();
		}

		private void setFetchVesselData()
		{
			LogDebug("vessel data will be refetched");
			vesselDataFetched = false;
			vesselTotalMass = 0;
			vesselCrewCapacity = 0;
			if (resourceList.Count() > 0)
			{
				resourceList.Clear();
			}
		}
        // http://wiki.kerbalspaceprogram.com/wiki/API:GameEvents

		[System.Diagnostics.Conditional("DEBUG")]
		private void LogDebug(String msg)
		{
			Debug.Log("ResourceOverview: " + msg);
		}
		private void Log(String msg)
		{
			Debug.Log("ResourceOverview: " + msg);
		}

	}
}