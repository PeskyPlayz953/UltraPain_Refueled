using BepInEx;
using GameConsole.pcon;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ScriptingUtility;
using Ultrapain.Patches;
using TMPro;
using UnityEngine.UI;
using DitzelGames.FastIK;

namespace Ultrapain.Patches
{
    /*[HarmonyPatch(typeof(GameProgressSaver), "GetGameProgress", new Type[] { typeof(int) })]
    class CustomProgress_GetGameProgress
    {
        static bool Prefix(ref int __0)
        {
            if (Plugin.ultrapainDifficulty)
                __0 = 100;
            return true;
        }
    }

    [HarmonyPatch(typeof(GameProgressSaver), "GetGameProgress", new Type[] { typeof(string), typeof(int) }, new ArgumentType[] { ArgumentType.Out, ArgumentType.Normal })]
    class CustomProgress_GetGameProgress2
    {
        static bool Prefix(ref int __1)
        {
            if (Plugin.ultrapainDifficulty)
                __1 = 100;
            return true;
        }
    }

    [HarmonyPatch(typeof(GameProgressSaver), "GetPrime")]
    class CustomProgress_GetPrime
    {
        static bool Prefix(ref int __1)
        {
            if (Plugin.ultrapainDifficulty)
                __1 = 100;
            return true;
        }
    }

    [HarmonyPatch(typeof(GameProgressSaver), "GetPrime")]
    class CustomProgress_GetProgress
    {
        static bool Prefix(ref int __0)
        {
            if (Plugin.ultrapainDifficulty)
                __0 = 100;
            return true;
        }
    }

    [HarmonyPatch(typeof(RankData), MethodType.Constructor, new Type[] { typeof(StatsManager) })]
    class CustomProgress_RankDataCTOR
    {
        static bool Prefix(RankData __instance, out int __state)
        {
            __state = -1;
            if (Plugin.ultrapainDifficulty)
            {
                __state = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
                MonoSingleton<PrefsManager>.Instance.SetInt("difficulty", 100);
            }

            return true;
        }

        static void Postfix(RankData __instance, int __state)
        {
            if (__state >= 0)
            {
                MonoSingleton<PrefsManager>.Instance.SetInt("difficulty", __state);
            }
        }
    }*/

    /*[HarmonyPatch(typeof(PrefsManager), "GetInt")]
    class StatsManager_DifficultyOverride
    {
        static bool Prefix(string __0, ref int __result)
        {
            if (__0 == "difficulty" && Plugin.realUltrapainDifficulty)
            {
                __result = 5;
                return false;
            }

            return true;
        }
    }*/

    class PrefsManager_Ctor
    {
        static void Postfix(ref Dictionary<string, Func<object, object>> ___propertyValidators)
        {
            ___propertyValidators.Clear();
        }
    }

    class PrefsManager_EnsureValid
    {
        static bool Prefix(string __0, object __1, ref object __result)
        {
            __result = __1;
            return false;
        }
    }
    class RankData_Fix
    {
        static bool Prefix(RankData __instance, ref StatsManager sman)
        {
            int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
            __instance.levelNumber = sman.levelNumber;
            RankData rank = GameProgressSaver.GetRank(true, -1);
            if (rank != null)
            {
                __instance.ranks = rank.ranks;
                if (rank.majorAssists != null)
                {
                    __instance.majorAssists = rank.majorAssists;
                }
                else
                {
                    __instance.majorAssists = new bool[11];
                }
                if (rank.stats != null)
                {
                    __instance.stats = rank.stats;
                }
                else
                {
                    __instance.stats = new RankScoreData[11];
                }
                if (__instance.majorAssists.Length < 11)
                {
                    //FIX SAVE LENGTH CODE (I FUCKING HOPE THIS DOESNT CORRUPT SAVES)

                    //Good news past me! You're clear! (mostly)

                    bool[] tmp_assists = new bool[11];
                    for (int a = 0; a < 11; a++)
                    {
                        if (a < __instance.majorAssists.Length)
                        {
                            tmp_assists[a] = __instance.majorAssists[a];
                        }
                        else
                        {
                            tmp_assists[a] = false;
                        }
                    }
                    __instance.majorAssists = tmp_assists;
                }
                if (__instance.ranks.Length < 11)
                {
                    int[] tmp_ranks1 = new int[11];
                    for (int r = 0; r < 11; r++)
                    {
                        if (r < __instance.ranks.Length)
                        {
                            tmp_ranks1[r] = __instance.ranks[r];
                        }
                        else
                        {
                            tmp_ranks1[r] = -1;
                        }
                    }
                    __instance.ranks = tmp_ranks1;
                }
                if (rank.ranks.Length < 11)
                {
                    int[] tmp_ranks2 = new int[11];
                    for (int rr = 0; rr < 11; rr++)
                    {
                        if (rr < rank.ranks.Length)
                        {
                            tmp_ranks2[rr] = rank.ranks[rr];
                        }
                        else
                        {
                            tmp_ranks2[rr] = -1;
                        }
                    }
                    rank.ranks = tmp_ranks2;
                }
                if (rank.majorAssists.Length < 11)
                {
                    bool[] tmp_assist2 = new bool[11];
                    for (int aa = 0; aa < 11; aa++)
                    {
                        if (aa < rank.majorAssists.Length)
                        {
                            tmp_assist2[aa] = rank.majorAssists[aa];
                        }
                        else
                        {
                            tmp_assist2[aa] = false;
                        }
                    }
                    rank.majorAssists = tmp_assist2;
                }
                if (__instance.stats.Length < 11)
                {
                    RankScoreData[] tmp_stats = new RankScoreData[11];
                    for (int s = 0; s < 11; s++)
                    {
                        if (s < __instance.stats.Length)
                        {
                            tmp_stats[s] = __instance.stats[s];
                        }
                    }
                    __instance.stats = tmp_stats;
                }
                if ((sman.rankScore >= rank.ranks[@int] && (rank.majorAssists == null || (!sman.majorUsed && rank.majorAssists[@int]))) || sman.rankScore > rank.ranks[@int] || rank.levelNumber != __instance.levelNumber)
                {
                    __instance.majorAssists[@int] = sman.majorUsed;
                    __instance.ranks[@int] = sman.rankScore;
                    if (__instance.stats[@int] == null)
                    {
                        __instance.stats[@int] = new RankScoreData();
                    }
                    __instance.stats[@int].kills = sman.kills;
                    __instance.stats[@int].style = sman.stylePoints;
                    __instance.stats[@int].time = sman.seconds;
                }
                __instance.secretsAmount = sman.secretObjects.Length;
                __instance.secretsFound = new bool[__instance.secretsAmount];
                int num = 0;
                while (num < __instance.secretsAmount && num < rank.secretsFound.Length)
                {
                    if (sman.secretObjects[num] == null || rank.secretsFound[num])
                    {
                        __instance.secretsFound[num] = true;
                    }
                    num++;
                }
                __instance.challenge = rank.challenge;
                return false;
            }
            __instance.ranks = new int[11];
            __instance.stats = new RankScoreData[11];
            if (__instance.stats[@int] == null)
            {
                __instance.stats[@int] = new RankScoreData();
            }
            __instance.majorAssists = new bool[11];
            for (int i = 0; i < __instance.ranks.Length; i++)
            {
                __instance.ranks[i] = -1;
            }
            __instance.ranks[@int] = sman.rankScore;
            __instance.majorAssists[@int] = sman.majorUsed;
            __instance.stats[@int].kills = sman.kills;
            __instance.stats[@int].style = sman.stylePoints;
            __instance.stats[@int].time = sman.seconds;
            __instance.secretsAmount = sman.secretObjects.Length;
            __instance.secretsFound = new bool[__instance.secretsAmount];
            for (int j = 0; j < __instance.secretsAmount; j++)
            {
                if (sman.secretObjects[j] == null)
                {
                    __instance.secretsFound[j] = true;
                }
            }
            return false;
        }
    }
    public static class GameProgressSaver_GetProgress_Fix
    {
        public static bool Prefix(ref int difficulty, ref int __result)
        {
            int num = 1;
            for (int i = difficulty; i <= 10; i++)
            {
                GameProgressData gameProgress = GameProgressSaver.GetGameProgress(i);
                if (gameProgress != null && gameProgress.difficulty == i && gameProgress.levelNum > num)
                {
                    num = gameProgress.levelNum;
                }
            }
            __result = num;
            return false;
        }
    }
    public static class GameProgressSaver_GetEncoreProgress_Fix
    {
        public static bool Prefix(ref int difficulty, ref int __result)
        {
            int num = 0;
            for (int i = difficulty; i <= 10; i++)
            {
                GameProgressData gameProgress = GameProgressSaver.GetGameProgress(i);
                if (gameProgress != null && gameProgress.difficulty == i && gameProgress.encores > num)
                {
                    num = gameProgress.encores;
                }
            }
            __result = num;
            return false;
        }
    }
    public static class GameProgressSaver_GetPrime_Fix
    {
        public static bool Prefix(ref int difficulty, ref int level, ref int __result)
        {
            if (SceneHelper.IsPlayingCustom)
            {
                __result = 0;
                return false;
            }
            level--;
            int num = 0;
            for (int i = difficulty; i <= 10; i++)
            {
                GameProgressData gameProgress = GameProgressSaver.GetGameProgress(i);
                if (gameProgress != null && gameProgress.difficulty == i && gameProgress.primeLevels != null && gameProgress.primeLevels.Length > level && gameProgress.primeLevels[level] > num)
                {
                    Debug.Log("Highest: . Data: " + gameProgress.primeLevels[level].ToString());
                    if (gameProgress.primeLevels[level] >= 2)
                    {
                        __result = 2;
                        return false;
                    }
                    num = gameProgress.primeLevels[level];
                }
            }
            __result = num;
            return false;
        }
    }
    class LevelSelectPanel_Fix
    {
        static bool Prefix(LevelSelectPanel __instance)
        {

            __instance.Setup();
            __instance.rectTransform = __instance.gameObject.GetComponent<RectTransform>();
            if (__instance.levelNumber == 666)
            {
                __instance.tempInt = GameProgressSaver.GetPrime(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0), __instance.levelNumberInLayer);
            }
            else if (__instance.levelNumber == 100)
            {
                __instance.tempInt = GameProgressSaver.GetEncoreProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0));
            }
            else
            {
                __instance.tempInt = GameProgressSaver.GetProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0));
            }
            int num = __instance.levelNumber;
            if (__instance.levelNumber == 666 || __instance.levelNumber == 100)
            {
                num += __instance.levelNumberInLayer - 1;
            }
            __instance.origName = GetMissionName.GetMission(num);
            if ((__instance.levelNumber == 666 && __instance.tempInt == 0) || (__instance.levelNumber == 100 && __instance.tempInt < __instance.levelNumberInLayer - 1) || (__instance.levelNumber != 666 && __instance.levelNumber != 100 && __instance.tempInt < __instance.levelNumber) || __instance.forceOff)
            {
                string str = __instance.ls.layerNumber.ToString();
                if (__instance.ls.layerNumber == 666)
                {
                    str = "P";
                }
                if (__instance.ls.layerNumber == 100)
                {
                    __instance.gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = (__instance.levelNumberInLayer - 1).ToString() + "-E: ???";
                }
                else
                {
                    __instance.gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = str + "-" + __instance.levelNumberInLayer.ToString() + ": ???";
                }
                __instance.gameObject.transform.Find("Image").GetComponent<Image>().sprite = __instance.lockedSprite;
                __instance.gameObject.GetComponent<Button>().enabled = false;
                __instance.rectTransform.sizeDelta = new Vector2(__instance.rectTransform.sizeDelta.x, __instance.collapsedHeight);
                __instance.leaderboardPanel.SetActive(false);
            }
            else
            {
                bool flag;
                if (__instance.tempInt == __instance.levelNumber || (__instance.levelNumber == 100 && __instance.tempInt == __instance.levelNumberInLayer - 1) || (__instance.levelNumber == 666 && __instance.tempInt == 1))
                {
                    flag = false;
                    __instance.gameObject.transform.Find("Image").GetComponent<Image>().sprite = __instance.unlockedSprite;
                    __instance.gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = (__instance.levelNumberInLayer - 1).ToString() + "-E: ???";
                }
                else
                {
                    flag = true;
                    __instance.gameObject.transform.Find("Image").GetComponent<Image>().sprite = __instance.origSprite;
                }
                if (__instance.levelNumber != 100 || __instance.tempInt != __instance.levelNumberInLayer - 1)
                {
                    __instance.gameObject.transform.Find("Name").GetComponent<TMP_Text>().text = __instance.origName;
                }
                __instance.gameObject.GetComponent<Button>().enabled = true;
                if (__instance.challengeIcon != null)
                {
                    if (__instance.challengeChecker == null)
                    {
                        __instance.challengeChecker = __instance.challengeIcon.transform.Find("EventTrigger").gameObject;
                    }
                    if (__instance.tempInt > __instance.levelNumber)
                    {
                        __instance.challengeChecker.SetActive(true);
                    }
                }
                if (MonoSingleton<PrefsManager>.Instance.GetBool("levelLeaderboards", false) && flag)
                {
                    __instance.rectTransform.sizeDelta = new Vector2(__instance.rectTransform.sizeDelta.x, __instance.expandedHeight);
                    __instance.leaderboardPanel.SetActive(true);
                }
                else
                {
                    __instance.rectTransform.sizeDelta = new Vector2(__instance.rectTransform.sizeDelta.x, __instance.collapsedHeight);
                    __instance.leaderboardPanel.SetActive(false);
                }
            }
            RankData rank = GameProgressSaver.GetRank(num, false);
            //rank fix here (fucking horrendous code)
            if (rank.ranks.Length < 11)
            {
                int[] tmp_ranks2 = new int[11];
                for (int rr = 0; rr < 11; rr++)
                {
                    if (rr < rank.ranks.Length)
                    {
                        tmp_ranks2[rr] = rank.ranks[rr];
                    }
                }
                rank.ranks = tmp_ranks2;
            }
            if (rank.majorAssists.Length < 11)
            {
                bool[] tmp_assist2 = new bool[11];
                for (int aa = 0; aa < 11; aa++)
                {
                    if (aa < rank.majorAssists.Length)
                    {
                        tmp_assist2[aa] = rank.majorAssists[aa];
                    }
                }
                rank.majorAssists = tmp_assist2;
            }

            if (rank == null)
            {
                Debug.Log("Didn't Find Level " + __instance.levelNumber.ToString() + " Data");
                Image component = __instance.gameObject.transform.Find("Stats").Find("Rank").GetComponent<Image>();
                component.color = Color.white;
                component.sprite = __instance.unfilledPanel;
                component.GetComponentInChildren<TMP_Text>().text = "";
                __instance.allSecrets = false;
                foreach (Image image in __instance.secretIcons)
                {
                    image.enabled = true;
                    image.sprite = __instance.unfilledPanel;
                }
                return false;
            }
            int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
            if (rank.levelNumber == __instance.levelNumber || ((__instance.levelNumber == 666 || __instance.levelNumber == 100) && rank.levelNumber == __instance.levelNumber + __instance.levelNumberInLayer - 1))
            {
                TMP_Text componentInChildren = __instance.gameObject.transform.Find("Stats").Find("Rank").GetComponentInChildren<TMP_Text>();
                if (rank.ranks[@int] == 12 && (rank.majorAssists == null || !rank.majorAssists[@int]))
                {
                    componentInChildren.text = "<color=#FFFFFF>P</color>";
                    Image component2 = componentInChildren.transform.parent.GetComponent<Image>();
                    component2.color = new Color(1f, 0.686f, 0f, 1f);
                    component2.sprite = __instance.filledPanel;
                    __instance.ls.AddScore(4, true);
                }
                else if (rank.majorAssists != null && rank.majorAssists[@int])
                {
                    if (rank.ranks[@int] < 0)
                    {
                        componentInChildren.text = "";
                    }
                    else
                    {
                        switch (rank.ranks[@int])
                        {
                            case 1:
                                componentInChildren.text = "C";
                                __instance.ls.AddScore(1, false);
                                break;
                            case 2:
                                componentInChildren.text = "B";
                                __instance.ls.AddScore(2, false);
                                break;
                            case 3:
                                componentInChildren.text = "A";
                                __instance.ls.AddScore(3, false);
                                break;
                            case 4:
                            case 5:
                            case 6:
                                __instance.ls.AddScore(4, false);
                                componentInChildren.text = "S";
                                break;
                            default:
                                __instance.ls.AddScore(0, false);
                                componentInChildren.text = "D";
                                break;
                        }
                        Image component3 = componentInChildren.transform.parent.GetComponent<Image>();
                        component3.color = new Color(0.3f, 0.6f, 0.9f, 1f);
                        component3.sprite = __instance.filledPanel;
                    }
                }
                else if (rank.ranks[@int] < 0)
                {
                    componentInChildren.text = "";
                    Image component4 = componentInChildren.transform.parent.GetComponent<Image>();
                    component4.color = Color.white;
                    component4.sprite = __instance.unfilledPanel;
                }
                else
                {
                    switch (rank.ranks[@int])
                    {
                        case 1:
                            componentInChildren.text = "<color=#4CFF00>C</color>";
                            __instance.ls.AddScore(1, false);
                            break;
                        case 2:
                            componentInChildren.text = "<color=#FFD800>B</color>";
                            __instance.ls.AddScore(2, false);
                            break;
                        case 3:
                            componentInChildren.text = "<color=#FF6A00>A</color>";
                            __instance.ls.AddScore(3, false);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            __instance.ls.AddScore(4, false);
                            componentInChildren.text = "<color=#FF0000>S</color>";
                            break;
                        default:
                            __instance.ls.AddScore(0, false);
                            componentInChildren.text = "<color=#0094FF>D</color>";
                            break;
                    }
                    Image component5 = componentInChildren.transform.parent.GetComponent<Image>();
                    component5.color = Color.white;
                    component5.sprite = __instance.unfilledPanel;
                }
                if (rank.secretsAmount > 0)
                {
                    __instance.allSecrets = true;
                    for (int j = 0; j < 5; j++)
                    {
                        if (j < rank.secretsAmount && rank.secretsFound[j])
                        {
                            __instance.secretIcons[j].sprite = __instance.filledPanel;
                        }
                        else
                        {
                            __instance.allSecrets = false;
                            __instance.secretIcons[j].sprite = __instance.unfilledPanel;
                        }
                    }
                }
                else
                {
                    Image[] array = __instance.secretIcons;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].enabled = false;
                    }
                }
                if (__instance.challengeIcon)
                {
                    if (rank.challenge)
                    {
                        __instance.challengeIcon.sprite = __instance.filledPanel;
                        TMP_Text componentInChildren2 = __instance.challengeIcon.GetComponentInChildren<TMP_Text>();
                        componentInChildren2.text = "C O M P L E T E";
                        if (rank.ranks[@int] == 12 && (__instance.allSecrets || rank.secretsAmount == 0))
                        {
                            componentInChildren2.color = new Color(0.6f, 0.4f, 0f, 1f);
                        }
                        else
                        {
                            componentInChildren2.color = Color.black;
                        }
                    }
                    else
                    {
                        __instance.challengeIcon.sprite = __instance.unfilledPanel;
                        TMP_Text componentInChildren3 = __instance.challengeIcon.GetComponentInChildren<TMP_Text>();
                        componentInChildren3.text = "C H A L L E N G E";
                        componentInChildren3.color = Color.white;
                    }
                }
            }
            else
            {
                Debug.Log("Error in finding " + __instance.levelNumber.ToString() + " Data");
                Image component6 = __instance.gameObject.transform.Find("Stats").Find("Rank").GetComponent<Image>();
                component6.color = Color.white;
                component6.sprite = __instance.unfilledPanel;
                component6.GetComponentInChildren<TMP_Text>().text = "";
                __instance.allSecrets = false;
                foreach (Image image2 in __instance.secretIcons)
                {
                    image2.enabled = true;
                    image2.sprite = __instance.unfilledPanel;
                }
            }
            if ((rank.challenge || !__instance.challengeIcon) && rank.ranks[@int] == 12 && (__instance.allSecrets || rank.secretsAmount == 0))
            {
                __instance.ls.Gold();
                __instance.gameObject.GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 0.75f);
                return false;
            }
            __instance.gameObject.GetComponent<Image>().color = __instance.defaultColor;
            return false;
        }
    }
    class EndlessHighScore_Fix
    {
        static bool Prefix()
        {
            int diff = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
            if (diff > 5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
