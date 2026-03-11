using HarmonyLib;
using System;
using UnityEngine;

namespace Ultrapain.Patches
{
    class SwingCheck2_CheckCollision_Patch2
    {
        static bool Prefix(SwingCheck2 __instance, ref Collider __0)
        {
            //if (__instance.transform.parent == null)
            //   return true;

            EnemyIdentifier eid = __instance.eid;
            if (eid == null || eid.enemyType != EnemyType.Filth)
                return true;

            if (__instance.damaging == false) return true;

            if (__0.name != "Player") return true;

            GameObject expObj = GameObject.Instantiate(Plugin.explosion, eid.transform.position, Quaternion.identity);
            foreach(Explosion exp in expObj.GetComponentsInChildren<Explosion>())
            {
                exp.enemy = true;
                exp.damage = (int)(ConfigManager.filthExplosionDamage.value * eid.totalDamageModifier);
                exp.maxSize *= ConfigManager.filthExplosionSize.value;
                exp.speed *= ConfigManager.filthExplosionSize.value;
                exp.toIgnore.Add(EnemyType.Filth);
            }
            __instance.damaging = false;

            if (ConfigManager.filthExplodeKills.value)
            {
                eid.Death();
            }
            
            return true;
        }
    }

    class Filth_Zombie_SetSpeed_Patch
    {
        static void Postfix(Zombie __instance)
        {
            __instance.speedMultiplier = 1.5f;
            if (__instance.zm)
            {
                __instance.nma.acceleration = 120f;
                __instance.nma.angularSpeed = 9000f;
                __instance.nma.speed = 20f;
            }
            else if (__instance.eid.enemyType == EnemyType.Soldier)
            {
                __instance.nma.speed = 20f * __instance.speedMultiplier;
                __instance.anim.SetFloat("RunSpeed", ((__instance.difficulty == 5) ? 1.75f : 1f) * __instance.speedMultiplier);
            }
            else
            {
                __instance.nma.speed = 10f * __instance.speedMultiplier;
            }
            if (__instance.nma)
            {
                __instance.defaultSpeed = __instance.nma.speed;
            }
            if (__instance.anim)
            {
                if (__instance.variableSpeed)
                {
                    __instance.anim.speed = 1f * __instance.speedMultiplier;
                    return;
                }
                if (__instance.difficulty >= 2)
                {
                    __instance.anim.speed = 1f * __instance.eid.totalSpeedModifier;
                    return;
                }
            }

            }
    }

    class Filth_ZombieMelee_GetSpeed_Patch
    {
        static void Postfix(ZombieMelee __instance, ref EnemyMovementData __result)
        {
            __result = new EnemyMovementData
            {
                acceleration = 60f,
                angularSpeed = 2600f,
                speed = 20f
            };
        }
    }

    class Filth_ZombieMelee_Start_Patch
    {
        static void Postfix(ZombieMelee __instance)
        {
            __instance.defaultCoolDown = 0.25f;
        }
    }
}
