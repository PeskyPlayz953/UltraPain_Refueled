using HarmonyLib;
using UnityEngine;

namespace Ultrapain.Patches
{
    class Virtue_Start_Patch
    {
        static void Postfix(Drone __instance, ref EnemyIdentifier ___eid)
        {
            VirtueFlag flag = __instance.gameObject.AddComponent<VirtueFlag>();
            flag.virtue = __instance;
        }
    }

    class Virtue_Death_Patch
    {
        static bool Prefix(Drone __instance, ref EnemyIdentifier ___eid)
        {
            if(___eid.enemyType != EnemyType.Virtue)
                return true;

            __instance.GetComponent<VirtueFlag>().DestroyProjectiles();
            return true;
        }
    }

    class VirtueFlag : MonoBehaviour
    {
        public AudioSource lighningBoltSFX;
        public GameObject ligtningBoltAud;
        public Transform windupObj;
        private EnemyIdentifier eid;
        public Drone virtue;

        public void Awake()
        {
            eid = GetComponent<EnemyIdentifier>();
            ligtningBoltAud = Instantiate(Plugin.lighningBoltSFX, transform);
            lighningBoltSFX = ligtningBoltAud.GetComponent<AudioSource>();
        }

        public void SpawnLightningBolt()
        {
            LightningStrikeExplosive lightningStrikeExplosive = Instantiate(Plugin.lightningStrikeExplosiveSetup.gameObject, windupObj.transform.position, Quaternion.identity).GetComponent<LightningStrikeExplosive>();
            lightningStrikeExplosive.safeForPlayer = false;
            lightningStrikeExplosive.damageMultiplier = eid.totalDamageModifier * ((virtue.isEnraged)? ConfigManager.virtueEnragedLightningDamage.value : ConfigManager.virtueNormalLightningDamage.value);
            
            if(windupObj != null)
                Destroy(windupObj.gameObject);
        }

        public void DestroyProjectiles()
        {
            CancelInvoke("SpawnLightningBolt");

            if (windupObj != null)
                Destroy(windupObj.gameObject);
        }
    }

    class Virtue_SpawnInsignia_Patch
    {
        static bool Prefix(Drone __instance, ref EnemyIdentifier ___eid, ref int ___difficulty, ref int ___usedAttacks)
        {

            if (___eid.enemyType != EnemyType.Virtue)
                return true;

            GameObject createInsignia(Drone instance, EnemyIdentifier eid, int difficulty, EnemyTarget target, int damage = 30, float lastMultiplier = 1)
            {
                /*GameObject gameObject = GameObject.Instantiate<GameObject>(__instance.projectile.ToAsset(), target.position, Quaternion.identity);
                VirtueInsignia component = gameObject.GetComponent<VirtueInsignia>();
                component.target = new EnemyTarget(MonoSingleton<PlayerTracker>.Instance.GetPlayer());
                component.parentDrone = __instance;
                component.hadParent = true;
                if(lastMultiplier < 1)
                {
                    component.explosionLength *= lastMultiplier;
                }
                component.explosionLength *= 2;
                __instance.chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (__instance.enraged)
                {
                    component.predictive = true;
                }
              

                if (MonoSingleton<PlayerTracker>.Instance.playerType == PlayerType.Platformer)
                {
                    gameObject.transform.localScale *= 0.75f;
                    component.windUpSpeedMultiplier *= 0.875f;
                }
                component.windUpSpeedMultiplier *= ___eid.totalSpeedModifier;
                component.damage = Mathf.RoundToInt((float)component.damage * ___eid.totalDamageModifier);

                return gameObject;*/

                instance.enemy.parryable = false;
                GameObject gameObject = Object.Instantiate<GameObject>(instance.projectile.ToAsset(), target.position, Quaternion.identity);
                VirtueInsignia component = gameObject.GetComponent<VirtueInsignia>();
                component.target = target;
                component.parentDrone = __instance;
                component.hadParent = true;
                instance.chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (instance.isEnraged)
                {
                    component.predictive = true;
                }
                if (instance.difficulty >= 4)
                {
                    component.explosionLength = ((__instance.difficulty >= 5) ? 5f : 3.5f)*lastMultiplier;
                }
                if (MonoSingleton<PlayerTracker>.Instance.playerType == PlayerType.Platformer)
                {
                    gameObject.transform.localScale *= 0.75f;
                    component.windUpSpeedMultiplier *= 0.875f;
                }
                component.windUpSpeedMultiplier *= eid.totalSpeedModifier;
                component.damage = damage;
                component.damage = Mathf.RoundToInt((float)component.damage * eid.totalDamageModifier);
                instance.usedAttacks++;
                return gameObject;
            }

            if (__instance.isEnraged && !ConfigManager.virtueTweakEnragedAttackToggle.value)
                return true;
            if (!__instance.isEnraged && !ConfigManager.virtueTweakNormalAttackToggle.value)
                return true;

            bool insignia = (__instance.isEnraged) ? ConfigManager.virtueEnragedAttackType.value == ConfigManager.VirtueAttackType.Insignia
                : ConfigManager.virtueNormalAttackType.value == ConfigManager.VirtueAttackType.Insignia;

            if (insignia)
            {
                bool xAxis = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaXtoggle.value : ConfigManager.virtueNormalInsigniaXtoggle.value;
                bool yAxis = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaYtoggle.value : ConfigManager.virtueNormalInsigniaYtoggle.value;
                bool zAxis = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaZtoggle.value : ConfigManager.virtueNormalInsigniaZtoggle.value;
                int damage = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaXdamage.value : ConfigManager.virtueNormalInsigniaXdamage.value;
                float lastingmulti = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaLastMulti.value : ConfigManager.virtueNormalInsigniaLastMulti.value;
                if (xAxis)
                {
                    GameObject obj = createInsignia(__instance, ___eid, ___difficulty, __instance.eid.target, damage, lastingmulti);
                    float size = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaXsize.value : ConfigManager.virtueNormalInsigniaXsize.value;
                    obj.transform.localScale = new Vector3(size, obj.transform.localScale.y, size);
                    obj.transform.Rotate(new Vector3(90f, 0, 0));
                    obj.GetComponent<VirtueInsignia>().windUpSpeedMultiplier *= 1.01f;
                }
                if (yAxis)
                {
                    GameObject obj = createInsignia(__instance, ___eid, ___difficulty, __instance.eid.target, damage, lastingmulti);
                    float size = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaYsize.value : ConfigManager.virtueNormalInsigniaYsize.value;
                    obj.transform.localScale = new Vector3(size, obj.transform.localScale.y, size);
                }
                if (zAxis)
                {
                    GameObject obj = createInsignia(__instance, ___eid, ___difficulty, __instance.eid.target, damage, lastingmulti);
                    float size = (__instance.isEnraged) ? ConfigManager.virtueEnragedInsigniaZsize.value : ConfigManager.virtueNormalInsigniaZsize.value;
                    obj.transform.localScale = new Vector3(size, obj.transform.localScale.y, size);
                    obj.transform.Rotate(new Vector3(0, 0, 90f));
                    obj.GetComponent<VirtueInsignia>().windUpSpeedMultiplier *= 0.99f;
                }
            }
            else
            {
                Vector3 predictedPos;
                if (___difficulty <= 1)
                    predictedPos = MonoSingleton<PlayerTracker>.Instance.GetPlayer().position;
                else
                {
                    Vector3 vector = new Vector3(MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().x, 0f, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().z);
                    predictedPos = MonoSingleton<PlayerTracker>.Instance.GetPlayer().position + vector.normalized * Mathf.Min(vector.magnitude, 5.0f);
                }

                GameObject currentWindup = GameObject.Instantiate<GameObject>(Plugin.lighningStrikeWindup.gameObject, predictedPos, Quaternion.identity);
                foreach (Follow follow in currentWindup.GetComponents<Follow>())
                {
                    if (follow.speed != 0f)
                    {
                        if (___difficulty >= 2)
                        {
                            follow.speed *= (float)___difficulty;
                        }
                        else if (___difficulty == 1)
                        {
                            follow.speed /= 2f;
                        }
                        else
                        {
                            follow.enabled = false;
                        }
                        follow.speed *= ___eid.totalSpeedModifier;
                    }
                }

                VirtueFlag flag = __instance.GetComponent<VirtueFlag>();
                flag.lighningBoltSFX.Play();
                flag.windupObj = currentWindup.transform;
                flag.Invoke("SpawnLightningBolt", (__instance.isEnraged)? ConfigManager.virtueEnragedLightningDelay.value : ConfigManager.virtueNormalLightningDelay.value);
            }

            ___usedAttacks += 1;
            if (___difficulty > 2 && ___usedAttacks > 2)
            {
                __instance.Invoke("Enrage", 3f / ___eid.totalSpeedModifier);
            }

            return false;
        }

        /*static void Postfix(Drone __instance, ref EnemyIdentifier ___eid, ref int ___difficulty, ref Transform ___target, bool __state)
        {
            if (!__state)
                return;

            GameObject createInsignia(Drone __instance, ref EnemyIdentifier ___eid, ref int ___difficulty, ref Transform ___target)
            {
                GameObject gameObject = GameObject.Instantiate<GameObject>(__instance.projectile, ___target.transform.position, Quaternion.identity);
                VirtueInsignia component = gameObject.GetComponent<VirtueInsignia>();
                component.target = MonoSingleton<PlayerTracker>.Instance.GetPlayer();
                component.parentDrone = __instance;
                component.hadParent = true;
                __instance.chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                if (__instance.enraged)
                {
                    component.predictive = true;
                }
                if (___difficulty == 1)
                {
                    component.windUpSpeedMultiplier = 0.875f;
                }
                else if (___difficulty == 0)
                {
                    component.windUpSpeedMultiplier = 0.75f;
                }
                if (MonoSingleton<PlayerTracker>.Instance.playerType == PlayerType.Platformer)
                {
                    gameObject.transform.localScale *= 0.75f;
                    component.windUpSpeedMultiplier *= 0.875f;
                }
                component.windUpSpeedMultiplier *= ___eid.totalSpeedModifier;
                component.damage = Mathf.RoundToInt((float)component.damage * ___eid.totalDamageModifier);

                return gameObject;
            }

            GameObject xAxisInsignia = createInsignia(__instance, ref ___eid, ref ___difficulty, ref ___target);
            xAxisInsignia.transform.Rotate(new Vector3(90, 0, 0));
            xAxisInsignia.transform.localScale = new Vector3(xAxisInsignia.transform.localScale.x * horizontalInsigniaScale, xAxisInsignia.transform.localScale.y, xAxisInsignia.transform.localScale.z * horizontalInsigniaScale);
            GameObject zAxisInsignia = createInsignia(__instance, ref ___eid, ref ___difficulty, ref ___target);
            zAxisInsignia.transform.Rotate(new Vector3(0, 0, 90));
            zAxisInsignia.transform.localScale = new Vector3(zAxisInsignia.transform.localScale.x * horizontalInsigniaScale, zAxisInsignia.transform.localScale.y, zAxisInsignia.transform.localScale.z * horizontalInsigniaScale);
        }*/
    }
}
