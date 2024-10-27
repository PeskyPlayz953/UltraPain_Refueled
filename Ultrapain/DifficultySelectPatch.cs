using HarmonyLib;
using UnityEngine.UI;

namespace Ultrapain
{
    class DifficultySelectPatch
    {
        static void Postfix(DifficultySelectButton __instance)
        {
            string difficultyName = __instance.gameObject.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text;
            Plugin.ultrapainDifficulty = difficultyName == ConfigManager.pluginName.value || ConfigManager.globalDifficultySwitch.value;
            Plugin.realUltrapainDifficulty = difficultyName == ConfigManager.pluginName.value;
        }
    }
}
