using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ResourceOverview.RegisterToolbar;

namespace PluginBaseFramework
{
    abstract class BaseWindow : PluginBase
    {
        protected int windowID;
        protected string windowTitle;

        internal Rect windowPosition = new Rect();
        internal float windowHeight;
        internal float windowWidth;

        protected bool _windowVisible = false;
        public bool windowVisible
        {
            get { return _windowVisible; }
            set
            {
                _windowVisible = value;
            }
        }

        protected bool _windowHover;
        public bool windowHover
        {
            get { return _windowHover; }
            set
            {
                _windowHover = value;
            }
        }

        public BaseWindow(string title, float width, float height)
        {
            windowTitle = title;
            windowWidth = width;
            windowHeight = height;
            windowID = UnityEngine.Random.Range(1000, 2000000) + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.GetHashCode(); // generate window ID
            Log.Debug("BaseWindow constructor for " + title);

            if (windowPosition.x == 0 && windowPosition.y == 0)
            {
                // set initial size and position
                windowPosition = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);
            }

        }


        void OnGUI()
        {
            preDrawGui();
            if (windowVisible || windowHover)
            {
                windowPosition = GUILayout.Window(windowID, windowPosition, drawGui, windowTitle,
                    GUILayout.Width(windowWidth), // overwrite values from windowPosition
                    GUILayout.Height(windowHeight));
            }

        }

        protected abstract void preDrawGui();
        protected abstract void drawGui(int windowID);

    }
}
