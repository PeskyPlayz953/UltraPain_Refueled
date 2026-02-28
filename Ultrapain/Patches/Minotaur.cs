using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ultrapain.Patches
{
    class Minotaur_Charge_Count : MonoBehaviour
    {
        public int doubledash = 0;
    }
    class Minotaur_Init_Patch
    {
        static bool Prefix(Minotaur __instance)
        {
            __instance.gameObject.AddComponent<Minotaur_Charge_Count>();
            __instance.GetComponent<Minotaur_Charge_Count>().doubledash = 0;
            return true;
        }
    }
    class Minotaur_DoubleRamEnd_Patch
    {
        static void Postfix(Minotaur __instance)
        {
            if (__instance.GetComponent<Minotaur_Charge_Count>().doubledash >= 1) {
                __instance.GetComponent<Minotaur_Charge_Count>().doubledash = 0;
                __instance.RamStart();
            }
        }
    }
    class Minotaur_DoubleRamStart_Patch
    {
        static void Postfix(Minotaur __instance)
        {
            __instance.GetComponent<Minotaur_Charge_Count>().doubledash = 1;
        }
    }
}
