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
		private Rect roWindowPosition;

		private Dictionary<String, DisplayResource> resourceList = new Dictionary<String, DisplayResource>();
		private bool resourcesFetched = false;

        public void Start()
        {
            if (ToolbarManager.ToolbarAvailable) 
            {


                roButton = ToolbarManager.Instance.add("RO", "ROButton");
                roButton.TexturePath = "ResourceOverview/button_icon";
                roButton.ToolTip = "Resource Overview Window";
                roButton.OnClick += (e) =>
                {
                    roWindowVisible = !roWindowVisible;
                };
            }
            else
            {
                roWindowVisible = true;
            }
			
			
			GameEvents.onEditorShipModified.Add(onEditorShipModified);
			GameEvents.onPartRemove.Add(onPartRemove);
        }

        public void Update()
        {

           
        }

        void OnGUI()
        {
			if (roWindowPosition.x == 0 && roWindowPosition.y == 0)
			{
				// set initial size
				roWindowPosition = new Rect(Screen.width - 250, 100, 200, 50);
			}
			if (roWindowVisible)
			{
				roWindowPosition = GUILayout.Window(456123, roWindowPosition, resourceOverviewWindow, "Resource Overview Window",
					GUILayout.Width(200), // overwrite values from roWindowPosition
					GUILayout.Height(50 + resourceList.Count * 20));
			}
        }

		

        private void resourceOverviewWindow(int windowID)
        {
			GUILayout.BeginVertical();
			
			if (EditorLogic.startPod != null) { 
				getAllResources(EditorLogic.startPod);
				foreach (String key in resourceList.Keys)
				{
					GUILayout.Label(key + ": " + String.Format("{0:,0.00}", resourceList[key].amount) + " / " + String.Format("{0:,0.00}", resourceList[key].maxAmount), GUILayout.ExpandWidth(true));
				}
				
			}
			else
			{
				GUILayout.Label("No resources to display!", GUILayout.ExpandWidth(true));
			}
			GUILayout.EndVertical();
			GUI.DragWindow();
        }

        private void getAllResources(Part p)
        {
			if (resourcesFetched)
			{
				return;
			}

			fetchResources(p);

			resourcesFetched = true;
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

        void OnDestroy()
        {
            LogDebug("destroy");
           
			GameEvents.onEditorShipModified.Remove(onEditorShipModified);
			GameEvents.onPartRemove.Remove(onPartRemove);
            if (ToolbarManager.ToolbarAvailable)
            {
                roButton.Destroy();
            }
        }

		private void onPartRemove(GameEvents.HostTargetAction<Part, Part> data)
		{
			LogDebug("onPartRemove");
			setFetchResourceAgain();
		}


		void onEditorShipModified(ShipConstruct sc)
		{
			LogDebug("onEditorShipModified");
			setFetchResourceAgain();
		}

		private void setFetchResourceAgain()
		{
			LogDebug("clearing resources");
			resourcesFetched = false;
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