using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ProjectProphet;
using ProjectProphet.Behaviours.Props;
using HarmonyLib;


namespace Ultrapain.Patches
{
    class CrossmodSupport_MD_SplendorConductor
    {
        static bool Prefix(Explosion __instance)
        {
            if(__instance.hitterWeapon == "gabriel.splendorblast")
            {
                __instance.electric = true;
            }
            return true;
        }
    }
}
