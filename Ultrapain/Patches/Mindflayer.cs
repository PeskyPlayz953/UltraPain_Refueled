﻿using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace Ultrapain.Patches
{
    class Mindflayer_Start_Patch
    {
        static void Postfix(Mindflayer __instance, ref EnemyIdentifier ___eid)
        {
            __instance.gameObject.AddComponent<MindflayerPatch>();
            //___eid.SpeedBuff();
        }
    }

    class Mindflayer_ShootProjectiles_Patch
    {
        public static float maxProjDistance = 5;
        public static float initialProjectileDistance = -1f;
        public static float distancePerProjShot = 0.2f;

        static bool Prefix(Mindflayer __instance, ref EnemyIdentifier ___eid, ref LayerMask ___environmentMask, ref bool ___enraged)
        {
            /*for(int i = 0; i < 20; i++)
            {
                Quaternion randomRotation = Quaternion.LookRotation(MonoSingleton<PlayerTracker>.Instance.GetTarget().position - __instance.transform.position);
                randomRotation.eulerAngles += new Vector3(UnityEngine.Random.Range(-15.0f, 15.0f), UnityEngine.Random.Range(-15.0f, 15.0f), UnityEngine.Random.Range(-15.0f, 15.0f));
                Projectile componentInChildren = GameObject.Instantiate(Plugin.homingProjectile.gameObject, __instance.transform.position + __instance.transform.forward, randomRotation).GetComponentInChildren<Projectile>();

                Vector3 randomPos = __instance.tentacles[UnityEngine.Random.RandomRangeInt(0, __instance.tentacles.Length)].position;
                if (!Physics.Raycast(__instance.transform.position, randomPos - __instance.transform.position, Vector3.Distance(randomPos, __instance.transform.position), ___environmentMask))
                    componentInChildren.transform.position = randomPos;

                componentInChildren.speed = 10f * ___eid.totalSpeedModifier * UnityEngine.Random.Range(0.5f, 1.5f);
                componentInChildren.turnSpeed *= UnityEngine.Random.Range(0.5f, 1.5f);
                componentInChildren.target = MonoSingleton<PlayerTracker>.Instance.GetTarget();
                componentInChildren.safeEnemyType = EnemyType.Mindflayer;
                componentInChildren.damage *= ___eid.totalDamageModifier;
            }
            
            __instance.chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            __instance.cooldown = (float)UnityEngine.Random.Range(4, 5);

            return false;*/

            MindflayerPatch counter = __instance.GetComponent<MindflayerPatch>();
            if (counter == null)
                return true;

            if (counter.shotsLeft == 0)
            {
                counter.shotsLeft = ConfigManager.mindflayerShootAmount.value;
                __instance.chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                __instance.cooldown = (float)UnityEngine.Random.Range(4, 5);
                return false;
            }

            Quaternion randomRotation = Quaternion.LookRotation(MonoSingleton<PlayerTracker>.Instance.GetTarget().position - __instance.transform.position);
            randomRotation.eulerAngles += new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
            Projectile componentInChildren = GameObject.Instantiate(Plugin.homingProjectile, __instance.transform.position + __instance.transform.forward, randomRotation).GetComponentInChildren<Projectile>();

            Vector3 randomPos = __instance.tentacles[UnityEngine.Random.RandomRangeInt(0, __instance.tentacles.Length)].position;
            if (!Physics.Raycast(__instance.transform.position, randomPos - __instance.transform.position, Vector3.Distance(randomPos, __instance.transform.position), ___environmentMask))
                componentInChildren.transform.position = randomPos;

            int shotCount = ConfigManager.mindflayerShootAmount.value - counter.shotsLeft;
            componentInChildren.transform.position += componentInChildren.transform.forward * Mathf.Clamp(initialProjectileDistance + shotCount * distancePerProjShot, 0, maxProjDistance);

            componentInChildren.speed = ConfigManager.mindflayerShootInitialSpeed.value * ___eid.totalSpeedModifier;
            componentInChildren.turningSpeedMultiplier = ConfigManager.mindflayerShootTurnSpeed.value;
            componentInChildren.target = new EnemyTarget(MonoSingleton<PlayerTracker>.Instance.GetTarget());
            componentInChildren.safeEnemyType = EnemyType.Mindflayer;
            componentInChildren.damage *= ___eid.totalDamageModifier;
            componentInChildren.sourceWeapon = __instance.gameObject;
            counter.shotsLeft -= 1;
            __instance.Invoke("ShootProjectiles", ConfigManager.mindflayerShootDelay.value / ___eid.totalSpeedModifier);

            return false;
        }
    }

    class EnemyIdentifier_DeliverDamage_MF
    {
        static bool Prefix(EnemyIdentifier __instance, ref float __3, GameObject __6)
        {
            if (__instance.enemyType != EnemyType.Mindflayer)
                return true;

            if (__6 == null || __6.GetComponent<Mindflayer>() == null)
                return true;

            __3 *= ConfigManager.mindflayerProjectileSelfDamageMultiplier.value / 100f;
            return true;
        }
    }

    class SwingCheck2_CheckCollision_Patch
    {
        static FieldInfo goForward = typeof(Mindflayer).GetField("goForward", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo meleeAttack = typeof(Mindflayer).GetMethod("MeleeAttack", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(Collider __0, out int __state)
        {
            __state = __0.gameObject.layer;
            return true;
        }

        static void Postfix(SwingCheck2 __instance, Collider __0, int __state)
        {
            if (__0.tag == "Player")
                Debug.Log($"Collision with {__0.name} with tag {__0.tag} and layer {__state}");
            if (__0.gameObject.tag != "Player" || __state == 15)
                return;

            if (__instance.transform.parent == null)
                return;

            Debug.Log("Parent check");
            Mindflayer mf = __instance.transform.parent.gameObject.GetComponent<Mindflayer>();

            if (mf == null)
                return;

            MindflayerPatch patch = mf.gameObject.GetComponent<MindflayerPatch>();

            Debug.Log("Attempting melee combo");
            __instance.DamageStop();
            goForward.SetValue(mf, false);
            meleeAttack.Invoke(mf, new object[] { });

            if (patch.swingComboLeft > 0)
            {
                patch.swingComboLeft -= 1;
                __instance.DamageStop();
                goForward.SetValue(mf, false);
                meleeAttack.Invoke(mf, new object[] { });
            }
            else
                patch.swingComboLeft = 2;
        }
    }

    class Mindflayer_MeleeTeleport_Patch
    {
        public static Vector3 deltaPosition = new Vector3(0, -10, 0);

        static bool Prefix(Mindflayer __instance, ref EnemyIdentifier ___eid, ref LayerMask ___environmentMask, ref bool ___goingLeft, ref Animator ___anim, ref bool ___enraged)
        {
            if (___eid.drillers.Count > 0)
                return false;

            Vector3 targetPosition = MonoSingleton<PlayerTracker>.Instance.PredictPlayerPosition(0.9f) + deltaPosition;
            float distance = Vector3.Distance(__instance.transform.position, targetPosition);

            Ray targetRay = new Ray(__instance.transform.position, targetPosition - __instance.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(targetRay, out hit, distance, ___environmentMask, QueryTriggerInteraction.Ignore))
            {
                targetPosition = targetRay.GetPoint(Mathf.Max(0.0f, hit.distance - 1.0f));
            }

            MonoSingleton<HookArm>.Instance.StopThrow(1f, true);
            __instance.transform.position = targetPosition;
            ___goingLeft = !___goingLeft;

            GameObject.Instantiate<GameObject>(__instance.teleportSound, __instance.transform.position, Quaternion.identity);
            GameObject gameObject = GameObject.Instantiate<GameObject>(__instance.decoy, __instance.transform.GetChild(0).position, __instance.transform.GetChild(0).rotation);
            Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
            AnimatorStateInfo currentAnimatorStateInfo = ___anim.GetCurrentAnimatorStateInfo(0);
            componentInChildren.Play(currentAnimatorStateInfo.shortNameHash, 0, currentAnimatorStateInfo.normalizedTime);
            componentInChildren.speed = 0f;
            if (___enraged)
            {
                gameObject.GetComponent<MindflayerDecoy>().enraged = true;
            }

            ___anim.speed = 0f;
            __instance.CancelInvoke("ResetAnimSpeed");
            __instance.Invoke("ResetAnimSpeed", 0.25f / ___eid.totalSpeedModifier);

            return false;
        }
    }

    class SwingCheck2_DamageStop_Patch
    {
        static void Postfix(SwingCheck2 __instance)
        {
            if (__instance.transform.parent == null)
                return;
            GameObject parent = __instance.transform.parent.gameObject;
            Mindflayer mf = parent.GetComponent<Mindflayer>();
            if (mf == null)
                return;

            MindflayerPatch patch = parent.GetComponent<MindflayerPatch>();
            patch.swingComboLeft = 2;
        }
    }

    class MindflayerPatch : MonoBehaviour
    {
        public int shotsLeft = ConfigManager.mindflayerShootAmount.value;
        public int swingComboLeft = 2;
    }
}
