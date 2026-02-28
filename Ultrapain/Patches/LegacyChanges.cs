using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace Ultrapain.Patches
{
    class LegacyChanges_RemoveFullArsenal
    {
    }
    class LegacyChanges_RemoveViolenceFeatures
    {
        static void Prefix(NewMovement __instance)
        {
            if (__instance.gameObject.transform.Find("Main Camera") == null){
                return;
            }
            if (__instance.gameObject.transform.Find("Main Camera").transform.Find("Punch") == null)
            {
                return;
            }
            if (__instance.gameObject.transform.Find("Main Camera").transform.Find("Projectile Parry Zone") == null)
            {
                return;
            }
            if (ConfigManager.violenceDowngrade.value == true)
            {
                __instance.gameObject.transform.Find("Main Camera").transform.Find("Punch").transform.Find("Projectile Parry Zone").localScale = new Vector3(1f, 1f, 6f);
            }
            else
            {
                __instance.gameObject.transform.Find("Main Camera").transform.Find("Punch").transform.Find("Projectile Parry Zone").localScale = new Vector3(3f, 3f, 6f);
            }
        }
    }
    class LegacyChanges_AddBrutalStacking
    {
        static bool Prefix(EnemyIdentifier __instance, ref float multiplier)
        {
            if (ConfigManager.brutalStatStacking.value == true)
            {
                multiplier /= (2f / 1.5f);
            }
            return true;
        }
    }
}
