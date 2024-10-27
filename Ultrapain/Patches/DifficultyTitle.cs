using HarmonyLib;
using UnityEngine.UI;

namespace Ultrapain.Patches
{
    public class DifficultyTitle_Check_Patch
    {
        static void Postfix(DifficultyTitle __instance)
        {
            if (__instance.txt2.text.Contains("ULTRAKILL MUST DIE") && Plugin.realUltrapainDifficulty)
                __instance.txt2.text = __instance.txt2.text.Replace("ULTRAKILL MUST DIE", ConfigManager.pluginName.value);

            //else if (___txt.text == "-- VIOLENT --" && Plugin.ultrapainDifficulty)
            //    ___txt.text = "-- ULTRAPAIN --";
        }
    }
}
