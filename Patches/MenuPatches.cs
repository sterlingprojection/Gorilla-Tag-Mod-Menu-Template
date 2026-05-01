using HarmonyLib;
using GorillaLocomotion;
using Testplate.Menu;

namespace Testplate.Patches
{
    [HarmonyPatch(typeof(GTPlayer))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class MenuPatches
    {
        public static void Postfix()
        {
            MenuManager.OnUpdate();
        }
    }
}
