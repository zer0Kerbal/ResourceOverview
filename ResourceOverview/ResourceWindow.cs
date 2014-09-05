using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	class ResourceWindow : BaseWindow
	{

		private Dictionary<String, DisplayResource> resourceList = new Dictionary<String, DisplayResource>();
		public bool vesselDataFetched = false;
		private float vesselTotalMass;
		private float vesselDryMass;
		private int vesselCrewCapacity;
		private int vesselPartCount;

		public ResourceWindow(): base("Resource Overview Window", 200, 50)
		{
			

		}

		public void Start()
		{
			LogDebug("window start");
			
			GameEvents.onEditorShipModified.Add(onEditorShipModified);
			GameEvents.onPartRemove.Add(onPartRemove);
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

		protected override void preDrawGui()
		{
			if (EditorLogic.startPod == null) // nothing to display, show only text
			{
				windowHeight = 50;
			}
			else // we got something, calculate size
			{
				windowHeight = 90 + resourceList.Count * 20;
			}
		}

		protected override void drawGui(int windowID)
		{
			GUILayout.BeginVertical();

			if (EditorLogic.startPod != null)
			{
				reloadVesselData();
				GUILayout.Label("Total Mass: " + String.Format("{0:,0.00}", vesselTotalMass), GUILayout.ExpandWidth(true));
				GUILayout.Label("Dry Mass: " + String.Format("{0:,0.00}", vesselDryMass), GUILayout.ExpandWidth(true));
				GUILayout.Label("Crew Capacity: " + vesselCrewCapacity, GUILayout.ExpandWidth(true));
				GUILayout.Label("Part Count: " + vesselPartCount, GUILayout.ExpandWidth(true));
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
			LogDebug("window destroy");

			GameEvents.onEditorShipModified.Remove(onEditorShipModified);
			GameEvents.onPartRemove.Remove(onPartRemove);
		}
	}
}
