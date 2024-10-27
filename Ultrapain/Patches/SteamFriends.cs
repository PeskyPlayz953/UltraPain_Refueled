using HarmonyLib;
using Steamworks;
using System.Text.RegularExpressions;

namespace Ultrapain.Patches
{
    class SteamController_FetchSceneActivity_Patch
    {
        static bool Prefix(ref string scene)
        {
            return true;
        }
    }
}
