using HarmonyLib;
using System;
using System.ComponentModel;
using UnityEngine;

namespace Ultrapain.Patches
{
    class Mannequin_SetSpeed_Patch
    {
        static void Postfix(Mannequin __instance)
        {
            __instance.anim.speed = 1.3f;
            __instance.walkSpeed = 24f;
            __instance.skitterSpeed = 70f;
        }
    }
}
