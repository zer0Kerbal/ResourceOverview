using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

using PluginBaseFramework;
using static ResourceOverview.RegisterToolbar;

namespace ResourceOverview
{
    partial class ResourceOverview
    {

        private Dictionary<String, DisplayResource> resourceList = new Dictionary<String, DisplayResource>();
        public bool vesselDataFetched = false;
        private float vesselTotalMass;
        private float vesselDryMass;
        private float vesselTWR;
        private float vesselMaxThrust;
        private int vesselCrewCapacity;
        private int vesselPartCount;

        protected SettingsWindow settingsWindow;

        public ResourceOverview() : base("Resource Overview", 250, 50)
        {

        }

        public void onSettingsChanged()
        {
            KSPSettings.load();
            Log.Info("onSettingsChanged");
        }

        void SetUpUpdateCoroutine()
        {
            StartCoroutine(UpdateResourcesCoroutine());
        }

        IEnumerator UpdateResourcesCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                setFetchVesselData();
            }
        }

        private void onPartRemove(GameEvents.HostTargetAction<Part, Part> data)
        {
            Log.Info("onPartRemove");
            setFetchVesselData();
        }


        void onEditorShipModified(ShipConstruct sc)
        {
            Log.Info("onEditorShipModified");
            setFetchVesselData();
        }

        private void setFetchVesselData()
        {
            Log.Info("vessel data will be refetched");
            vesselDataFetched = false;
            vesselTotalMass = 0;
            vesselCrewCapacity = 0;
            if (resourceList.Count > 0)
            {
                resourceList.Clear();
            }
        }


         Vector3 SetDirection(int ctrlDir, Vessel vessel)
        {
            if (ctrlDir == 0)
            {
                return (vessel.rootPart.transform.up);
            }
            if (ctrlDir == 1)
            {
                return (vessel.rootPart.transform.forward);
            }
            if (ctrlDir == 2)
            {
                return (-vessel.rootPart.transform.up);
            }
            if (ctrlDir == 3)
            {
                return (-vessel.rootPart.transform.forward);
            }
            if (ctrlDir == 4)
            {
                return (vessel.rootPart.transform.right);
            }
            if (ctrlDir == 5)
            {
                return (-vessel.rootPart.transform.right);
            }
            else
            {
                return (vessel.rootPart.transform.up);
            }
        }
         int controlDirection = 0; //control direction         
         ModuleEngines TWR1EngineModule;
         ModuleEnginesFX TWR1EngineModuleFX;

        public  double GetThrustInfo(double altitude, out double minThrust, out double maxThrust)
        {
            Vessel activeVessel = FlightGlobals.ActiveVessel;

            double TWR1MaxThrust = 0;
            double TWR1MaxThrustVertical = 0;
            double TWR1MinThrust = 0;
            double TWR1MinThrustVertical = 0;

            double actualThrustLastFrame = 0;
            var TWR1ControlUp = SetDirection(controlDirection, activeVessel);
            for (int i = 0; i < activeVessel.Parts.Count; i++)
            {
                Part part = activeVessel.Parts[i];
                if (part.Modules.Contains("ModuleEngines") || part.Modules.Contains("ModuleEnginesFX")) //is part an engine?
                {
                    float DavonThrottleID = 0;
                    if (part.Modules.Contains("DifferentialThrustEngineModule")) //Devon Throttle Control Installed?
                    {
                        foreach (PartModule pm in part.Modules)
                        {

                            if (pm.moduleName == "DifferentialThrustEngineModule")
                            {
                                DavonThrottleID = (float)pm.Fields.GetValue("throttleFloatSelect"); //which throttle is engine assigned to?
                            }
                        }

                    }
                    if (DavonThrottleID == 0f)
                    {
                        foreach (PartModule TWR1PartModule in part.Modules) //change from part to partmodules
                        {
                            if (TWR1PartModule.moduleName == "ModuleEngines") //find partmodule engine on th epart
                            {
                                TWR1EngineModule = (ModuleEngines)TWR1PartModule; //change from partmodules to moduleengines

                                double offsetMultiplier;
                                try
                                {
                                    offsetMultiplier = Math.Max(0, Math.Cos(Mathf.Deg2Rad * Vector3.Angle(TWR1EngineModule.thrustTransforms[0].forward, -TWR1ControlUp)));
                                }
                                catch
                                {
                                    offsetMultiplier = 1;
                                }

                                if ((bool)TWR1PartModule.Fields.GetValue("throttleLocked") && TWR1EngineModule.isOperational)//if throttlelocked is true, this is solid rocket booster. then check engine is operational. if the engine is flamedout, disabled via-right click or not yet activated via stage control, isOperational returns false
                                {
                                    TWR1MaxThrust += ((TWR1EngineModule.finalThrust) * offsetMultiplier); //add engine thrust to MaxThrust
                                    TWR1MaxThrustVertical += (double)(TWR1EngineModule.finalThrust);
                                    TWR1MinThrust += ((TWR1EngineModule.finalThrust) * offsetMultiplier); //add engine thrust to MinThrust since this is an SRB
                                    TWR1MinThrustVertical += (double)(TWR1EngineModule.finalThrust);
                                }
                                else if (TWR1EngineModule.isOperational)//we know it is an engine and not a solid rocket booster so:
                                {
                                    TWR1MaxThrust += ((TWR1EngineModule.maxFuelFlow * TWR1EngineModule.g * TWR1EngineModule.atmosphereCurve.Evaluate((float)(TWR1EngineModule.vessel.staticPressurekPa * PhysicsGlobals.KpaToAtmospheres)) * TWR1EngineModule.thrustPercentage / 100F) * offsetMultiplier); //add engine thrust to MaxThrust
                                                                                                                                                                                                                                                                                                                    // errLine = "16d1";
                                    TWR1MaxThrustVertical += ((TWR1EngineModule.maxFuelFlow * TWR1EngineModule.g * TWR1EngineModule.atmosphereCurve.Evaluate((float)(TWR1EngineModule.vessel.staticPressurekPa * PhysicsGlobals.KpaToAtmospheres)) * TWR1EngineModule.thrustPercentage / 100F));
                                }
                                actualThrustLastFrame += TWR1EngineModule.finalThrust * (float)offsetMultiplier;
                            }
                            else if (TWR1PartModule.moduleName == "ModuleEnginesFX") //find partmodule engine on th epart
                            {
                                TWR1EngineModuleFX = (ModuleEnginesFX)TWR1PartModule; //change from partmodules to moduleengines
                                double offsetMultiplier;
                                try
                                {
                                    offsetMultiplier = Math.Cos(Mathf.Deg2Rad * Vector3.Angle(TWR1EngineModuleFX.thrustTransforms[0].forward, -TWR1ControlUp)); //how far off vertical is this engine?
                                }
                                catch
                                {
                                    offsetMultiplier = 1;
                                }
                                if ((bool)TWR1PartModule.Fields.GetValue("throttleLocked") && TWR1EngineModuleFX.isOperational)//if throttlelocked is true, this is solid rocket booster. then check engine is operational. if the engine is flamedout, disabled via-right click or not yet activated via stage control, isOperational returns false
                                {
                                    TWR1MaxThrust += (double)((TWR1EngineModuleFX.finalThrust) * offsetMultiplier); //add engine thrust to MaxThrust
                                    TWR1MaxThrustVertical += (double)TWR1EngineModuleFX.finalThrust;
                                    TWR1MinThrust += (double)((TWR1EngineModuleFX.finalThrust) * offsetMultiplier); //add engine thrust to MinThrust since this is an SRB
                                    TWR1MinThrustVertical += TWR1EngineModuleFX.finalThrust;
                                }
                                else if (TWR1EngineModuleFX.isOperational)//we know it is an engine and not a solid rocket booster so:
                                {
                                    TWR1MaxThrust += (double)((TWR1EngineModuleFX.maxFuelFlow * TWR1EngineModuleFX.g * TWR1EngineModuleFX.atmosphereCurve.Evaluate((float)(TWR1EngineModuleFX.vessel.staticPressurekPa * PhysicsGlobals.KpaToAtmospheres)) * TWR1EngineModuleFX.thrustPercentage / 100F) * offsetMultiplier); //add engine thrust to MaxThrust
                                                                                                                                                                                                                                                                                                                              // errLine = "17e1";
                                    TWR1MaxThrustVertical += ((TWR1EngineModuleFX.maxFuelFlow * TWR1EngineModuleFX.g * TWR1EngineModuleFX.atmosphereCurve.Evaluate((float)(TWR1EngineModuleFX.vessel.staticPressurekPa * PhysicsGlobals.KpaToAtmospheres)) * TWR1EngineModuleFX.thrustPercentage / 100F));
                                }
                                actualThrustLastFrame += TWR1EngineModuleFX.finalThrust * (float)offsetMultiplier;
                            }

                        }
                    }
                }
            }
            minThrust = TWR1MinThrust;
            maxThrust = TWR1MaxThrust;
            return actualThrustLastFrame;
        }
        void LoadFlightData()
        {
            Log.Info("LoadFlightData");
            var vessel = FlightGlobals.ActiveVessel;
            vesselTotalMass = 0;
            vesselDryMass = 0;
            vesselCrewCapacity = 0;
            for (int i = 0; i < vessel.Parts.Count; i++)
            {
                Part p = vessel.Parts[i];
                vesselDryMass += p.mass;
                vesselCrewCapacity += p.CrewCapacity;

                foreach (PartResource res in p.Resources)
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
            vesselPartCount = vessel.Parts.Count;
            vesselMaxThrust = (float)GetThrustInfo(FlightGlobals.ActiveVessel.altitude, out double  minThrust, out double maxThrust);
            vesselTotalMass = FlightGlobals.ActiveVessel.GetTotalMass();
            vesselTWR = (vesselMaxThrust / vesselTotalMass) / (float)9.81;

        }

        void LoadEditorData()
        {
            vesselTotalMass = EditorLogic.SortedShipList.Where(p => p.physicalSignificance == Part.PhysicalSignificance.FULL).Sum(p => p.mass + p.GetResourceMass());
            vesselDryMass = EditorLogic.SortedShipList.Where(p => p.physicalSignificance == Part.PhysicalSignificance.FULL).Sum(p => p.mass);
            vesselCrewCapacity = EditorLogic.SortedShipList.Sum(p => p.CrewCapacity);
            vesselPartCount = EditorLogic.SortedShipList.Count;

            // thanks to mechjeb for this part:
            var engines = (from part in EditorLogic.fetch.ship.parts
                           where part.inverseStage == StageManager.LastStage
                           from engine in part.Modules.OfType<ModuleEngines>()
                           select engine);
            var enginesfx = (from part in EditorLogic.fetch.ship.parts
                             where part.inverseStage == StageManager.LastStage
                             from engine in part.Modules.OfType<ModuleEnginesFX>()
                             where engine.isEnabled
                             select engine);
            vesselMaxThrust = engines.Sum(e => e.thrustPercentage / 100f * e.maxThrust) + enginesfx.Sum(e => e.thrustPercentage / 100f * e.maxThrust);

            vesselTWR = (vesselMaxThrust / vesselTotalMass) / (float)9.81;

            foreach (Part part in EditorLogic.SortedShipList)
            {
                foreach (PartResource res in part.Resources)
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
        }

        private void reloadVesselData()
        {
            if (vesselDataFetched)
            {
                return;
            }
            if (HighLogic.LoadedSceneIsEditor)
                LoadEditorData();
            else
                LoadFlightData();

            vesselDataFetched = true;
        }

        protected override void preDrawGui()
        {
            if (HighLogic.LoadedSceneIsEditor && EditorLogic.RootPart == null) // nothing to display, show only text
            {
                windowHeight = 50;
            }
            else // we got something, calculate size
            {
                windowHeight = 0;
                if (KSPSettings.showTotalMass
                    || KSPSettings.showDryMass
                    || KSPSettings.showCrewCapacity
                    || KSPSettings.showPartCount
                    || KSPSettings.showTWR)
                {
                    windowHeight += 10; // add some space before resources
                }

                if (KSPSettings.showTotalMass) windowHeight += 20;
                if (KSPSettings.showDryMass) windowHeight += 20;
                if (KSPSettings.showCrewCapacity) windowHeight += 20;
                if (KSPSettings.showPartCount) windowHeight += 20;
                if (KSPSettings.showTWR) windowHeight += 20;

                windowHeight += resourceList.Count * 20;
            }
        }

        protected override void drawGui(int windowID)
        {
            if (GUI.Button(new Rect(windowPosition.width - 22, 2, 20, 20), "s"))
            {
                settingsWindow = gameObject.AddComponent<SettingsWindow>();
            }

            GUILayout.BeginVertical();

            if (HighLogic.LoadedSceneIsFlight || EditorLogic.RootPart != null)
            {
                reloadVesselData();
                if (KSPSettings.showTotalMass) GUILayout.Label("Total Mass: " + String.Format("{0:,0.00}", vesselTotalMass), GUILayout.ExpandWidth(true));
                if (KSPSettings.showDryMass) GUILayout.Label("Dry Mass: " + String.Format("{0:,0.00}", vesselDryMass), GUILayout.ExpandWidth(true));
                if (KSPSettings.showCrewCapacity) GUILayout.Label("Crew Capacity: " + vesselCrewCapacity, GUILayout.ExpandWidth(true));
                if (KSPSettings.showPartCount) GUILayout.Label("Part Count: " + vesselPartCount, GUILayout.ExpandWidth(true));
                if (KSPSettings.showTWR) GUILayout.Label("TWR: " + String.Format("{0:,0.00}", vesselTWR), GUILayout.ExpandWidth(true));
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
            Log.Info("window destroy");
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorShipModified.Remove(onEditorShipModified);
            GameEvents.onPartRemove.Remove(onPartRemove);

            KSPSettings.SaveData();
        }


    }
}
