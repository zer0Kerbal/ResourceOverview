using UnityEngine;
using ToolbarControl_NS;
using KSP_Log;

namespace ResourceOverview
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        internal static Log Log = null;
        void Awake()
        {
            if (Log == null)
#if DEBUG
                Log = new Log("ResourceOverview", Log.LEVEL.INFO);
#else
                Log = new Log("ResourceOverview", Log.LEVEL.ERROR);
#endif

            DontDestroyOnLoad(this);

        }

        void Start()
        {
            ToolbarControl.RegisterMod(ResourceOverview.MODID, ResourceOverview.MODNAME);
        }
    }
}