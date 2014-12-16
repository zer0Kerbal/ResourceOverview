using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
﻿using PluginBaseFramework;

namespace ResourceOverview
{
	class ResourceWindow : BaseWindow
	{

		private Dictionary<String, DisplayResource> resourceList = new Dictionary<String, DisplayResource>();
		public bool vesselDataFetched = false;
		private float vesselTotalMass;
		private float vesselDryMass;
		private float vesselTWR;
		private int vesselCrewCapacity;
		private int vesselPartCount;

		protected SettingsWindow settingsWindow;

		public ResourceWindow(): base("Resource Overview", 250, 50)
		{

		}

		public void Start()
		{
			Log("window start");
			
			
			settingsWindow = gameObject.AddComponent<SettingsWindow>();
			KSPSettings.SettingsChanged += new KSPSettings.SettingsChangedEventHandler(onSettingsChanged);


			GameEvents.onEditorShipModified.Add(onEditorShipModified);
			GameEvents.onPartRemove.Add(onPartRemove);

			windowPosition.x = KSPSettings.get("ResourceWindow.x", (int)(Screen.width / 2 - windowWidth / 2));
			windowPosition.y = KSPSettings.get("ResourceWindow.y", (int)(Screen.height / 2 - windowHeight / 2));
		}

		public void onSettingsChanged()
		{
			KSPSettings.load();
			LogDebug("onSettingsChanged");
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
				foreach (PartResource res in part.Resources.list)
				{
					if (resourceList.ContainsKey(res.resourceName))
					{
						//res.info.density
						resourceList[res.resourceName].amount += res.amount;
						resourceList[res.resourceName].maxAmount += res.maxAmount;
					}
					else
					{
						resourceList.Add(res.resourceName, new DisplayResource(res.resourceName, res.amount, res.maxAmount, res.info.density));
					}
				}
			}

			//fetchResources(p);

			vesselDataFetched = true;
		}

		protected override void preDrawGui()
		{
			if (EditorLogic.RootPart == null) // nothing to display, show only text
			{
				windowHeight = 50;
			}
			else // we got something, calculate size
			{
				windowHeight = 0;
				if (KSPSettings.get("showTotalMass", true)
					|| KSPSettings.get("showDryMass", true)
					|| KSPSettings.get("showCrewCapacity", true)
					|| KSPSettings.get("showPartCount", true)
					|| KSPSettings.get("showTWR", true))
				{
					windowHeight += 10; // add some space before resources
				}

				if (KSPSettings.get("showTotalMass", true)) windowHeight += 20;
				if (KSPSettings.get("showDryMass", true)) windowHeight += 20;
				if (KSPSettings.get("showCrewCapacity", true)) windowHeight += 20;
				if (KSPSettings.get("showPartCount", true)) windowHeight += 20;
				if (KSPSettings.get("showTWR", true)) windowHeight += 20;

				windowHeight += resourceList.Count * 20;
			}
		}

		protected override void drawGui(int windowID)
		{
			if (GUI.Button(new Rect(windowPosition.width - 22, 2, 20, 20), "s"))
			{
				settingsWindow.windowVisible = true;
			}
			
			GUILayout.BeginVertical();

			if (EditorLogic.RootPart != null)
			{
				reloadVesselData();
				if (KSPSettings.get("showTotalMass", true)) GUILayout.Label("Total Mass: " + String.Format("{0:,0.00}", vesselTotalMass), GUILayout.ExpandWidth(true));
				if (KSPSettings.get("showDryMass", true)) GUILayout.Label("Dry Mass: " + String.Format("{0:,0.00}", vesselDryMass), GUILayout.ExpandWidth(true));
				if (KSPSettings.get("showCrewCapacity", true)) GUILayout.Label("Crew Capacity: " + vesselCrewCapacity, GUILayout.ExpandWidth(true));
				if (KSPSettings.get("showPartCount", true)) GUILayout.Label("Part Count: " + vesselPartCount, GUILayout.ExpandWidth(true));
				if (KSPSettings.get("showTWR", true)) GUILayout.Label("TWR: " + vesselPartCount, GUILayout.ExpandWidth(true));
				GUILayout.Space(10);
				
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

		void OnDestroy()
		{
			Log("window destroy");

			GameEvents.onEditorShipModified.Remove(onEditorShipModified);
			GameEvents.onPartRemove.Remove(onPartRemove);

			KSPSettings.set("ResourceWindow.x", (int)windowPosition.x);
			KSPSettings.set("ResourceWindow.y", (int)windowPosition.y);
			KSPSettings.save();
		}

		
	}
}
