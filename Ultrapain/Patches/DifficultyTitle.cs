using HarmonyLib;
using TMPro;
using UnityEngine.UI;

namespace Ultrapain.Patches
{
    public class DifficultyTitle_Check_Patch
    {
        static bool Prefix(DifficultyTitle __instance)
        {
			int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
			string text = "";
			if (__instance.lines)
			{
				text += "-- ";
			}
			switch (@int)
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
					text += "ULTRAPAIN"; break;
				case 7:
					text += "REFUELED"; break;
				case 8:
					text += "DUAL"; break;
				case 9:
					text += "SYSTEM OVERLOAD"; break;
				case 10:
					text += "CUSTOM"; break;

			}
			if (__instance.lines)
			{
				text += " --";
			}
            if (!__instance.txt2)
            {
                __instance.txt2 = __instance.gameObject.GetComponent<TMP_Text>();
            }
            if (__instance.txt2)
            {
                __instance.txt2.text = text;
                return false;
            }
            return false;
        }
    }
}
