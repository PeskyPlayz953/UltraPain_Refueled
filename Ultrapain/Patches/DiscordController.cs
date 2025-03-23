using Discord;
using HarmonyLib;
using System;
using System.Text.RegularExpressions;


namespace Ultrapain.Patches
{
    class DiscordController_SendActivity_Patch
    {
        static bool Prefix(DiscordController __instance)
        {
            if (__instance.discord == null || __instance.activityManager == null || __instance.disabled)
            {
                return false;
            }
            __instance.activityManager.UpdateActivity(__instance.cachedActivity, delegate (Result result)
            {
            });
            return false;
        }
    }
    class DiscordController_FetchSceneActivity_Patch
    {
        static bool Prefix(DiscordController __instance, ref string scene)
        {
            if (!DiscordController.Instance || DiscordController.Instance.disabled || DiscordController.Instance.discord == null || !(Plugin.ultrapainDifficulty || (MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0) >= 6)))
            {
                return true;
            }
            __instance.ResetActivityCache();
            if (SceneHelper.IsPlayingCustom)
            {
                __instance.cachedActivity.State = "Playing Custom Level";
                __instance.cachedActivity.Assets = __instance.customLevelActivityAssets.Deserialize();
            }
            else
            {
                StockMapInfo instance = StockMapInfo.Instance;
                if (instance)
                {
                    __instance.cachedActivity.Assets = instance.assets.Deserialize();
                    if (string.IsNullOrEmpty(__instance.cachedActivity.Assets.LargeImage))
                    {
                        __instance.cachedActivity.Assets.LargeImage = __instance.missingActivityAssets.Deserialize().LargeImage;
                    }
                    if (string.IsNullOrEmpty(__instance.cachedActivity.Assets.LargeText))
                    {
                        __instance.cachedActivity.Assets.LargeText = __instance.missingActivityAssets.Deserialize().LargeText;
                    }
                }
                else
                {
                    __instance.cachedActivity.Assets = __instance.missingActivityAssets.Deserialize();
                }
                if (scene == "Main Menu")
                {
                    __instance.cachedActivity.State = "Main Menu";
                }
                else
                {
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
                    __instance.cachedActivity.State = "DIFFICULTY: " + text;
                }
            }
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long start = (long)(DateTime.UtcNow - d).TotalSeconds;
            __instance.cachedActivity.Timestamps = new ActivityTimestamps
            {
                Start = start
            };
            __instance.SendActivity();
            return false;
        }
    }
}
