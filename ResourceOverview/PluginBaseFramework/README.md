PluginBaseFramework
===================

A base framework for KSP plugins.

Contains a BaseWindow class which handles all the basic window stuff. Just derive from it and override

  protected abstract void preDrawGui();
	protected abstract void drawGui(int windowID);

Also a Settings class which handles Settings saving and loading (to an XML File).
