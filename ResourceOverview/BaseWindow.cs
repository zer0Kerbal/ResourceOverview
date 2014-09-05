using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceOverview
{
	abstract class BaseWindow : PluginBase
	{
		protected string windowTitle;
		private bool _windowVisible;
		public bool windowVisible
			{
				get { return _windowVisible; }
				set {
					if (_windowVisible != value) // value will be changed
					{
						if (value)
						{
							RenderingManager.AddToPostDrawQueue(5, this.drawWindow);
						}
						else
						{
							RenderingManager.RemoveFromPostDrawQueue(5, this.drawWindow);
						}
					}
					_windowVisible = value;
				}
			}

		protected Rect windowPosition;
		protected float windowHeight;
		protected float windowWidth;

		public BaseWindow(string title, float width, float height)
		{
			windowTitle = title;
			windowWidth = width;
			windowHeight = height;
		}

		/*
		 * TODO: drag enabled, drawWindow/drawWindowInternal
		 * 
        //Are we allowing window drag
        if (DragEnabled)
            if (DragRect.height == 0 && DragRect.width == 0)
                GUI.DragWindow();
            else
                GUI.DragWindow(DragRect);
		 */

		protected void drawWindow()
		{
			preDrawGui();
			if (windowPosition.x == 0 && windowPosition.y == 0)
			{
				// set initial size and position
				windowPosition = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);
			}
			if (windowVisible)
			{
				windowPosition = GUILayout.Window(456123, windowPosition, drawGui, windowTitle,
					GUILayout.Width(windowWidth), // overwrite values from windowPosition
					GUILayout.Height(windowHeight));
			}

		}

		protected abstract void preDrawGui();
		protected abstract void drawGui(int windowID);
	}
}
