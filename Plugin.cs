using BepInEx;
using HarmonyLib;
using Testplate.Menu;

namespace Testplate
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony harmony;

        void Awake()
        {
            harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            
            MenuManager.Initialize();
            Logger.LogInfo("Testplate Mod Menu Loaded!");
        }

        void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}
