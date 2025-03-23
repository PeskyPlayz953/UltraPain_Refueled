using UnityEngine;

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
            GameObject gameObject = Object.Instantiate<GameObject>(MonoSingleton<DefaultReferenceManager>.Instance.parryableFlash, __instance.transform.position + Vector3.up * 6f + __instance.transform.forward * 3f, __instance.transform.rotation);
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
            GameObject gameObject = Object.Instantiate<GameObject>(MonoSingleton<DefaultReferenceManager>.Instance.parryableFlash, __instance.transform.position + Vector3.up * 6f + __instance.transform.forward * 3f, __instance.transform.rotation);
            gameObject.transform.localScale *= 5f;
            gameObject.transform.SetParent(__instance.transform, true);
        }
    }

    class Statue_GetHurt_Patch
    {
        static bool Prefix(Statue __instance, EnemyIdentifier ___eid)
        {
            CerberusFlag flag = __instance.GetComponent<CerberusFlag>();
            if (flag == null)
                return true;

            if (___eid.hitter != "punch" && ___eid.hitter != "shotgunzone")
                return true;

            float deltaTime = Time.time - flag.lastParryTime;
            if (deltaTime > ConfigManager.cerberusParryableDuration.value / ___eid.totalSpeedModifier)
                return true;

            flag.lastParryTime = 0;
            ___eid.health -= ConfigManager.cerberusParryDamage.value;
            MonoSingleton<FistControl>.Instance.currentPunch.Parry(false, ___eid);
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
