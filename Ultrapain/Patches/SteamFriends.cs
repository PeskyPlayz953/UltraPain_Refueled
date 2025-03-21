using HarmonyLib;
using Steamworks;
using System.Text.RegularExpressions;

namespace Ultrapain.Patches
{
    class SteamController_FetchSceneActivity_Patch
    {
        static void Postfix(ref string scene)
        {
            SteamFriends.SetRichPresence("difficulty", "ULTRAPAIN");
        }
    }
}
