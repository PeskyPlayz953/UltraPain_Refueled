using HarmonyLib;
using System;
using UnityEngine;

namespace Ultrapain.Patches
{
        class Gutterman_GetSpeed_Patch
        {
            static void Postfix(Gutterman __instance)
            {
                __instance.anim.speed = 1f;
                __instance.defaultMovementSpeed = 10f;
                __instance.windupSpeed = 1f;
                __instance.trackingSpeedMultiplier = ((__instance.difficulty == 2) ? 0.8f : 1f);
	            if (__instance.eid.puppet)
                {
                    __instance.trackingSpeedMultiplier *= 0.75f;
                }
                __instance.anim.speed *= __instance.eid.totalSpeedModifier;
                __instance.defaultMovementSpeed *= __instance.eid.totalSpeedModifier;
                __instance.nma.speed = (__instance.slowMode? (__instance.defaultMovementSpeed / 2f) : __instance.defaultMovementSpeed);
                __instance.windupSpeed *= __instance.eid.totalSpeedModifier;
                __instance.defaultTrackingSpeed = 1f;
	            if (__instance.trackingSpeed< __instance.defaultTrackingSpeed)
                {
                    __instance.trackingSpeed = __instance.defaultTrackingSpeed;
                }
            }
        }
}
