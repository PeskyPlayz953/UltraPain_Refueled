using UnityEngine;
using UnityEngine.AI;

namespace Ultrapain.Patches
{
    class CerberusFlag : MonoBehaviour
    {
        public Transform head;
        public float lastParryTime;
        private EnemyIdentifier eid;

        private void Awake()
        {
            eid = GetComponent<EnemyIdentifier>();
            head = transform.Find("Armature/Control/Waist/Chest/Chest_001/Head");
            if (head == null)
                head = UnityUtils.GetChildByTagRecursively(transform, "Head");
        }

        public void MakeParryable()
        {
            lastParryTime = Time.time;
        }
    }

    class StatueBoss_SetSpeed_Patch
    {
        static void Postfix(StatueBoss __instance)
        {
            switch (__instance.difficulty)
            {
                case 0:
                    __instance.anim.speed = 0.6f;
                    break;
                case 1:
                    __instance.anim.speed = 0.8f;
                    break;
                case 2:
                    __instance.anim.speed = 1f;
                    break;
                case 3:
                    __instance.anim.speed = 1.2f;
                    break;
                case 4:
                case 5:
                    __instance.anim.speed = 1.35f;
                    break;
                default:
                    __instance.anim.speed = 1.35f;
                    break;
            }
            __instance.anim.speed *= __instance.realSpeedModifier;
            if (__instance.enraged)
            {
                if (__instance.difficulty > 3)
                {
                    __instance.anim.speed = 1.5f * __instance.realSpeedModifier;
                }
                else if (__instance.difficulty == 3)
                {
                    __instance.anim.speed = 1.25f * __instance.realSpeedModifier;
                }
                else
                {
                    __instance.anim.speed *= 1.2f;
                }
                __instance.anim.SetFloat("WalkSpeed", 1.5f);
            }
        }
    }

    class StatueBoss_StopTracking_Patch
    {
        static void Postfix(StatueBoss __instance, Animator ___anim)
        {
            CerberusFlag flag = __instance.GetComponent<CerberusFlag>();
            if (flag == null)
                return;

            if (___anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Tackle")
                return;

            flag.MakeParryable();
            Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "TryParry");
            __instance.gameObject.GetComponent<Statue>().parryFramesLeft = (int)(ConfigManager.cerberusParryableDuration.value * 60f / __instance.eid.totalSpeedModifier);
            GameObject gameObject = Object.Instantiate<GameObject>(MonoSingleton<DefaultReferenceManager>.Instance.parryableFlash, __instance.transform.position + Vector3.up * 6f + __instance.transform.forward * 3f, __instance.transform.rotation);
            if (gameObject != null) { Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "ParryFlashExists"); }
            //gameObject.transform.localPosition = new Vector3(0, 6, 3);
            gameObject.transform.localScale *= 5f;
            gameObject.transform.SetParent(__instance.transform, true);
        }
    }

    class StatueBoss_Stomp_Patch
    {
        static void Postfix(StatueBoss __instance)
        {
            CerberusFlag flag = __instance.GetComponent<CerberusFlag>();
            if (flag == null)
                return;

            flag.MakeParryable();
            Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "TryParry");
            __instance.gameObject.GetComponent<Statue>().parryFramesLeft = (int)(ConfigManager.cerberusParryableDuration.value * 60f / __instance.eid.totalSpeedModifier);
            GameObject gameObject = Object.Instantiate<GameObject>(MonoSingleton<DefaultReferenceManager>.Instance.parryableFlash, __instance.transform.position + Vector3.up * 6f + __instance.transform.forward * 3f, __instance.transform.rotation);
            //gameObject.transform.localPosition = new Vector3(-0.1f, 7.2f, 3f);
            if (gameObject != null) { Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "ParryFlashExists"); }
            gameObject.transform.localScale *= 5f;
            gameObject.transform.SetParent(__instance.transform, true);
        }
    }

    class Statue_GetHurt_Patch
    {
        static bool Prefix(Enemy __instance)
        {
            if (__instance.eid.enemyType == EnemyType.Cerberus)
            {
                CerberusFlag flag = __instance.GetComponent<CerberusFlag>();
                if (flag == null)
                {
                    Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "flag not found");
                    return true;
                }
    
                if (__instance.eid.hitter != "punch" && __instance.eid.hitter != "shotgunzone")
                {
                    Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "incorrect damage type");
                return true;
                }


                float deltaTime = Time.time - flag.lastParryTime;
                if (deltaTime > ConfigManager.cerberusParryableDuration.value / __instance.eid.totalSpeedModifier) {
                    Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, deltaTime);
                    Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, ConfigManager.cerberusParryableDuration.value / __instance.eid.totalSpeedModifier);
                    return true;
                }else
                {
                    Plugin.BepLog.Log(BepInEx.Logging.LogLevel.Message, "success????");
                }
    
                    flag.lastParryTime = 0;
                __instance.eid.health -= ConfigManager.cerberusParryDamage.value;
                MonoSingleton<FistControl>.Instance.currentPunch.Parry(false, __instance.eid);
                return false;
            }
            return true;
            
        }
    }
    class StatueBoss_Tackle_Patch
    {
        public static void Postfix(StatueBoss __instance)
        {
            if (Plugin.ultrapainDifficulty)
            {
                __instance.extraTackles += 1;
            }
        }
    }
    class StatueBoss_StopDash_Patch
    {
        static bool Prefix(StatueBoss __instance)
        {
            __instance.dashPower = 0f;
            if (__instance.gc.onGround)
            {
                __instance.rb.isKinematic = true;
            }
            else
            {
                __instance.rb.velocity = Vector3.zero;
            }
            __instance.damaging = false;
            __instance.partAud.Stop();
            __instance.StopDamage();
            if (__instance.extraTackles > 0)
            {
                __instance.dontFall = true;
                __instance.extraTackles--;
                __instance.tracking = true;
                __instance.anim.speed = 0.1f;
                __instance.anim.Play("Tackle", -1, 0.4f);
                __instance.Invoke("DelayedTackle", 0.5f / __instance.realSpeedModifier);
                return false;
            }
            __instance.dontFall = false;
            return false;
        }
    }
    class StatueBoss_Start_Patch
    {
        static void Postfix(StatueBoss __instance)
        {
            __instance.gameObject.AddComponent<CerberusFlag>();
        }
    }
}
