﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Ultrapain.Patches
{
    class FleshObamium_Start
    {
        static bool Prefix(FleshPrison __instance)
        {
            if (__instance.altVersion)
                return true;

            if (__instance.eid == null)
                __instance.eid = __instance.GetComponent<EnemyIdentifier>();
            __instance.eid.overrideFullName = ConfigManager.fleshObamiumName.value;
            return true;
        }

        static void Postfix(FleshPrison __instance)
        {
            if (__instance.altVersion)
                return;

            GameObject fleshObamium = GameObject.Instantiate(Plugin.fleshObamium, __instance.transform);
            fleshObamium.transform.parent = __instance.transform.Find("fleshprisonrigged/Armature/root/prism/");
            fleshObamium.transform.localScale = new Vector3(36, 36, 36);
            fleshObamium.transform.localPosition = Vector3.zero;
            fleshObamium.transform.localRotation = Quaternion.identity;
            fleshObamium.transform.Rotate(new Vector3(180, 0, 0), Space.Self);
            fleshObamium.GetComponent<MeshRenderer>().material.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            fleshObamium.layer = 24;

            // __instance.transform.Find("FleshPrison2/FleshPrison2_Head").GetComponent<SkinnedMeshRenderer>().enabled = false;

        }
    }

    class FleshPrisonProjectile : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 50f, ForceMode.VelocityChange);
        }
    }

    class FleshPrisonRotatingInsignia : MonoBehaviour
    {
        List<VirtueInsignia> insignias = new List<VirtueInsignia>();
        public FleshPrison prison;
        public float damageMod = 1f;
        public float speedMod = 1f;

        void SpawnInsignias()
        {
            insignias.Clear();

            int projectileCount = (prison.altVersion ? ConfigManager.panopticonSpinAttackCount.value : ConfigManager.fleshPrisonSpinAttackCount.value);
            float anglePerProjectile = 360f / projectileCount;
            float distance = (prison.altVersion ? ConfigManager.panopticonSpinAttackDistance.value : ConfigManager.fleshPrisonSpinAttackDistance.value);
            Vector3 currentNormal = Vector3.forward;
            GameObject rotator = new GameObject();
            rotator.transform.SetPositionAndRotation(prison.transform.position, prison.transform.rotation);
            for (int i = 0; i < projectileCount; i++)
            {
                GameObject insignia = Instantiate(Plugin.virtueInsignia, prison.transform.position, Quaternion.identity);
                VirtueInsignia comp = insignia.GetComponent<VirtueInsignia>();
                GameObject compT = new GameObject("InsigniaHolder");
                compT.transform.SetParent(transform);
                compT.transform.position = prison.transform.position + distance * rotator.transform.forward;
                rotator.transform.localRotation = rotator.transform.localRotation * Quaternion.Euler(0, anglePerProjectile, 0);
                comp.explosionLength = 5f;
                comp.hadParent = false;
                comp.target = new EnemyTarget(compT.transform);
                comp.name = "SpinningInsignia" + i;
                comp.target.isPlayer = true;
                comp.noTracking = true;
                comp.predictive = true;
                comp.predictiveVersion = null;
                comp.otherParent = transform;
                comp.windUpSpeedMultiplier = (prison.altVersion ? ConfigManager.panopticonSpinAttackActivateSpeed.value : ConfigManager.fleshPrisonSpinAttackActivateSpeed.value) * speedMod;
                comp.damage = (int)((prison.altVersion ? ConfigManager.panopticonSpinAttackDamage.value : ConfigManager.fleshPrisonSpinAttackDamage.value) * damageMod);
                float size = Mathf.Abs(prison.altVersion ? ConfigManager.panopticonSpinAttackSize.value : ConfigManager.fleshPrisonSpinAttackSize.value);
                insignia.transform.localScale = new Vector3(size, insignia.transform.localScale.y, size);
                compT.transform.SetParent(transform);
                insignia.transform.SetParent(transform);
                insignias.Add(comp);
                gameObject.name = "FP_InsigniaStorage";
            }
        }
        FieldInfo inAction;
        public float anglePerSecond = 1f;
        void Start()
        {
            
            SpawnInsignias();
            inAction = typeof(FleshPrison).GetField("inAction", BindingFlags.Instance | BindingFlags.NonPublic);
            anglePerSecond = prison.altVersion ? ConfigManager.panopticonSpinAttackTurnSpeed.value : ConfigManager.fleshPrisonSpinAttackTurnSpeed.value;
            if (UnityEngine.Random.RandomRangeInt(0, 100) < 50)
                anglePerSecond *= -1;
        }

        bool markedForDestruction = false;
        void Update()
        {
            //DO NOT TOUCH, HELD TOGETHER BY DUCT TAPE AND A DREAM
            foreach (VirtueInsignia v in insignias)
            {
                v.target.targetTransform.RotateAround(prison.transform.position, Vector3.up, (anglePerSecond * Time.deltaTime * speedMod));
            }
            transform.Rotate(new Vector3(0, (anglePerSecond * Time.deltaTime * speedMod), 0));

            transform.position = prison.transform.position;

            if (!markedForDestruction && (prison == null || !(bool)inAction.GetValue(prison)))
            {
                markedForDestruction = true;
                return;
            }

            if (insignias.Count == 0 || insignias[0] == null)
                if (markedForDestruction)
                {
                    Destroy(gameObject);
                }
                else
                    SpawnInsignias();
        }
    }

    class FleshPrisonStart
    {
        static void Postfix(FleshPrison __instance)
        {
            if (__instance.altVersion)
                return;

            //__instance.homingProjectile = GameObject.Instantiate(Plugin.hideousMassProjectile, Vector3.positiveInfinity, Quaternion.identity);
            //__instance.homingProjectile.hideFlags = HideFlags.HideAndDontSave;
            //SceneManager.MoveGameObjectToScene(__instance.homingProjectile, SceneManager.GetSceneByName(""));
            //__instance.homingProjectile.AddComponent<FleshPrisonProjectile>();
        }
    }

    class FleshPrisonShoot
    {
        static void Postfix(FleshPrison __instance, ref Animator ___anim, EnemyIdentifier ___eid)
        {
            if (__instance.altVersion)
                return;

            GameObject obj = new GameObject();
            obj.transform.position = __instance.transform.position + Vector3.up;
            FleshPrisonRotatingInsignia flag = obj.AddComponent<FleshPrisonRotatingInsignia>();
            flag.prison = __instance;
            flag.damageMod = ___eid.totalDamageModifier;
            flag.speedMod = ___eid.totalSpeedModifier;
        }
    }

    /*[HarmonyPatch(typeof(FleshPrison), "SpawnInsignia")]
    class FleshPrisonInsignia
    {
        static bool Prefix(FleshPrison __instance, ref bool ___inAction, ref float ___fleshDroneCooldown, EnemyIdentifier ___eid,
            Statue ___stat, float ___maxHealth)
        {
            if (__instance.altVersion)
                return true;

            if (!Plugin.ultrapainDifficulty || !ConfigManager.enemyTweakToggle.value)
                return true;

            ___inAction = false;

            GameObject CreateInsignia()
            {
                GameObject gameObject = GameObject.Instantiate<GameObject>(__instance.insignia, MonoSingleton<PlayerTracker>.Instance.GetPlayer().position, Quaternion.identity);
                VirtueInsignia virtueInsignia;
                if (gameObject.TryGetComponent<VirtueInsignia>(out virtueInsignia))
                {
                    virtueInsignia.predictive = true;
                    virtueInsignia.noTracking = true;
                    virtueInsignia.otherParent = __instance.transform;
                    if (___stat.health > ___maxHealth / 2f)
                    {
                        virtueInsignia.charges = 2;
                    }
                    else
                    {
                        virtueInsignia.charges = 3;
                    }
                    virtueInsignia.charges++;
                    virtueInsignia.windUpSpeedMultiplier = 0.5f;
                    virtueInsignia.windUpSpeedMultiplier *= ___eid.totalSpeedModifier;
                    virtueInsignia.damage = Mathf.RoundToInt((float)virtueInsignia.damage * ___eid.totalDamageModifier);
                    virtueInsignia.target = MonoSingleton<PlayerTracker>.Instance.GetPlayer();
                    virtueInsignia.predictiveVersion = null;
                    Light light = gameObject.AddComponent<Light>();
                    light.range = 30f;
                    light.intensity = 50f;
                }
                gameObject.transform.localScale = new Vector3(5f, 2f, 5f);
                GoreZone componentInParent = __instance.GetComponentInParent<GoreZone>();
                if (componentInParent)
                {
                    gameObject.transform.SetParent(componentInParent.transform, true);
                }
                else
                {
                    gameObject.transform.SetParent(__instance.transform, true);
                }

                return gameObject;
            }

            GameObject InsigniaY = CreateInsignia();
            GameObject InsigniaX = CreateInsignia();
            GameObject InsigniaZ = CreateInsignia();

            InsigniaX.transform.eulerAngles = new Vector3(0, MonoSingleton<PlayerTracker>.Instance.GetTarget().transform.rotation.eulerAngles.y, 0);
            InsigniaX.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
            InsigniaZ.transform.eulerAngles = new Vector3(0, MonoSingleton<PlayerTracker>.Instance.GetTarget().transform.rotation.eulerAngles.y, 0);
            InsigniaZ.transform.Rotate(new Vector3(0, 0, 90), Space.Self);

            if (___fleshDroneCooldown < 1f)
            {
                ___fleshDroneCooldown = 1f;
            }

            return false;
        }
    }*/
}
