using HarmonyLib;
using Steamworks;
using System.Text.RegularExpressions;

namespace Ultrapain.Patches
{
    class SteamController_FetchSceneActivity_Patch
    {
        static bool Prefix(ref string scene)
        {
            if (!SteamClient.IsValid)
            {
                return false;
            }
            if (SceneHelper.IsPlayingCustom)
            {
                SteamFriends.SetRichPresence("steam_display", "#AtCustomLevel");
                return false;
            }
            StockMapInfo instance = StockMapInfo.Instance;
            if (scene == "Main Menu")
            {
                SteamFriends.SetRichPresence("steam_display", "#AtMainMenu");
                return false;
            }
            if (scene == "Endless")
            {
                SteamFriends.SetRichPresence("steam_display", "#AtCyberGrind");
                SteamFriends.SetRichPresence("difficulty", MonoSingleton<PresenceController>.Instance.diffNames[MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0)]);
                SteamFriends.SetRichPresence("wave", "0");
                return false;
            }
            if (instance != null && !string.IsNullOrEmpty(instance.assets.Deserialize().LargeText))
            {
                SteamFriends.SetRichPresence("steam_display", "#AtStandardLevel");
                int i = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
                string text = string.Empty;
                switch (i)
                {
                    case 0:
                        text += "HARMLESS"; break;
                    case 1:
                        text += "LENIENT"; break;
                    case 2:
                        text += "STANDARD"; break;
                    case 3:
                        text += "VIOLENT"; break;
                    case 4:
                        text += "BRUTAL"; break;
                    case 5:
                        text += "ULTRAKILL MUST DIE"; break;
                    case 6:
                        text += ConfigManager.pluginName.value; break;
                    case 7:
                        text += "REFUELED"; break;
                    case 8:
                        text += "DUAL"; break;
                    case 9:
                        text += "SYSTEM OVERLOAD"; break;
                    case 10:
                        text += "CUSTOM"; break;

                }
                SteamFriends.SetRichPresence("difficulty", "DIFFICULTY: " + text);
                SteamFriends.SetRichPresence("level", instance.assets.Deserialize().LargeText);
                return false;
            }
            SteamFriends.SetRichPresence("steam_display", "#UnknownLevel");
            return false;
        }

    }
}
