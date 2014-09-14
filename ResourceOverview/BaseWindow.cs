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
		protected bool addedToDrawQueue;

		protected bool _windowVisible;
		public bool windowVisible
			{
				get { return _windowVisible; }
				set {
					_windowVisible = value;
					doQueueAdd(value);
				}
			}

		protected bool _windowHover;
		public bool windowHover
		{
			get { return _windowHover; }
			set
			{
				_windowHover = value;
				doQueueAdd(value);
			}
		}

		protected void doQueueAdd(bool newval) {
			if (newval)
			{
				if (!addedToDrawQueue)
				{
					RenderingManager.AddToPostDrawQueue(1, this.drawWindow);
					LogDebug("adding to draw queue");
					addedToDrawQueue = true;
				}
				
			}
			else
			{
				if (addedToDrawQueue && !_windowVisible)
				{
					RenderingManager.RemoveFromPostDrawQueue(1, this.drawWindow);
					LogDebug("removing from draw queue");
					addedToDrawQueue = false;
				}
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
			if (windowVisible || windowHover)
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
