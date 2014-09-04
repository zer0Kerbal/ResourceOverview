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
        }

        public void Update()
        {

           
        }

        void OnGUI()
        {
			Log("reslist: " + resourceList.Count);
            if (roWindowVisible)
                GUILayout.Window(456123, new Rect(Screen.width - 250, 100, 200, 50+resourceList.Count*20), resourceOverviewWindow, "Resource Overview Window");
        }

		private Dictionary<String, double> resourceList = new Dictionary<String, double>();
		
		private bool resourcesFetched = false;

        private void resourceOverviewWindow(int windowID)
        {
			if (EditorLogic.startPod != null) { 
				getAllResources(EditorLogic.startPod);
				GUILayout.BeginVertical();
				GUILayout.Label("Resource Overview", GUILayout.ExpandWidth(true));
				foreach (String key in resourceList.Keys)
				{
					GUILayout.Label(key+": "+resourceList[key], GUILayout.ExpandWidth(true));
				}
				GUILayout.EndVertical();
			}
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
			Log("getting res for " + p.name);
			foreach (PartResource res in p.Resources.list)
			{
				Log("Resource: " + res.resourceName + ", Amount: " + res.amount);
				if (resourceList.ContainsKey(res.resourceName))
				{
					resourceList[res.resourceName] += res.amount;
				}
				else
				{
					resourceList.Add(res.resourceName, res.amount);
				}
			}

			foreach (Part ch in p.children)
			{
				fetchResources(ch);
			}
		}

        void OnDestroy()
        {
            Log("destroy");
           
			GameEvents.onEditorShipModified.Remove(onEditorShipModified);
            if (ToolbarManager.ToolbarAvailable)
            {
                roButton.Destroy();
            }
        }
		
		void onEditorShipModified(ShipConstruct sc)
		{
			Log("onEditorShipModified");
			setFetchResourceAgain();
		}

		private void setFetchResourceAgain()
		{
			Log("clearing resources");
			resourcesFetched = false;
			if (resourceList.Count() > 0)
			{
				resourceList.Clear();
			}
		}
        // http://wiki.kerbalspaceprogram.com/wiki/API:GameEvents

		private void Log(String msg)
		{
			Debug.Log("ResourceOverview: " + msg);
		}
    }
}