using HarmonyLib;
using Steamworks;
using System.Text.RegularExpressions;

namespace Ultrapain.Patches
{
    class SteamFriends_SetRichPresence_Patch
    {
        static bool Prefix(string __key, ref string __value)
        {
            if (__value != null)
            {
                __value = ConfigManager.pluginName.value;
            }
            return true;
        }
    }
}
