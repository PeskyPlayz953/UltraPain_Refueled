﻿using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using HarmonyLib;
using System.IO;
using Ultrapain.Patches;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
using Steamworks;
using Unity.Audio;
using System.Text;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UIElements;
using PluginConfig.API;
using ProjectProphet.Behaviours;
using ProjectProphet;
using ProjectProphet.Behaviours.Props;
using TMPro;
using GameConsole.pcon;

namespace Ultrapain
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.eternalUnion.pluginConfigurator", "1.6.0")]
    [BepInDependency("maranara_project_prophet", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("plonk.straymode", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        //Keeping GUID for compatibility with AngryLevelLoader
        public const string PLUGIN_GUID = "com.eternalUnion.ultraPain";
        public const string PLUGIN_NAME = "ULTRAPAIN: REFUELED";
        public const string PLUGIN_VERSION = "1.1.1";

        public static Plugin instance;

        private static bool addressableInit = false;
        public static T LoadObject<T>(string path)
        {
            if (!addressableInit)
            {
                Addressables.InitializeAsync().WaitForCompletion();
                addressableInit = true;

			}
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }

        public static Vector3 PredictPlayerPosition(Collider safeCollider, float speedMod)
        {   
            Transform target = MonoSingleton<PlayerTracker>.Instance.GetTarget();
            if (MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude == 0f)
                return target.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(target.position, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity(), out raycastHit, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude * 0.35f / speedMod, 4096, QueryTriggerInteraction.Collide) && raycastHit.collider == safeCollider)
                return target.position;
            else if (Physics.Raycast(target.position, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity(), out raycastHit, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude * 0.35f / speedMod, LayerMaskDefaults.Get(LMD.EnvironmentAndBigEnemies), QueryTriggerInteraction.Collide))
            {
                return raycastHit.point;
            }
            else {
                Vector3 projectedPlayerPos = target.position + MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity() * 0.35f / speedMod;
                return new Vector3(projectedPlayerPos.x, target.transform.position.y + (target.transform.position.y - projectedPlayerPos.y) * 0.5f, projectedPlayerPos.z);
            }
        }

        public static GameObject projectileSpread;
        public static GameObject homingProjectile;
        public static GameObject hideousMassProjectile;
        public static GameObject decorativeProjectile2;
        public static GameObject shotgunGrenade;
        public static GameObject beam;
        public static GameObject turretBeam;
        public static GameObject lightningStrikeExplosiveSetup;
        public static GameObject lightningStrikeExplosive;
        public static GameObject lighningStrikeWindup;
        public static GameObject explosion;
        public static GameObject bigExplosion;
        public static GameObject sandExplosion;
        public static GameObject virtueInsignia;
        public static GameObject rocket;
        public static GameObject revolverBullet;
        public static GameObject maliciousCannonBeam;
        public static GameObject lightningBoltSFX;
        public static GameObject revolverBeam;
        public static GameObject blastwave;
        public static GameObject cannonBall;
        public static GameObject shockwave;
        public static GameObject sisyphiusExplosion;
        public static GameObject sisyphiusPrimeExplosion;
        public static GameObject explosionWaveKnuckleblaster;
        public static GameObject chargeEffect;
        public static GameObject maliciousFaceProjectile;
        public static GameObject hideousMassSpear;
        public static GameObject coin;
        public static GameObject sisyphusDestroyExplosion;

        //public static GameObject idol;
        public static GameObject ferryman;
        public static GameObject minosPrime;
        //public static GameObject maliciousFace;
        public static GameObject somethingWicked;
        public static Turret turret;

        public static GameObject turretFinalFlash;
        public static GameObject enrageEffect;
        public static GameObject v2flashUnparryable;
        public static GameObject ricochetSfx;
        public static GameObject parryableFlash;

        public static AudioClip cannonBallChargeAudio;
        public static Material gabrielFakeMat;

        public static Sprite blueRevolverSprite;
        public static Sprite greenRevolverSprite;
        public static Sprite redRevolverSprite;
        public static Sprite blueShotgunSprite;
        public static Sprite greenShotgunSprite;
        public static Sprite blueNailgunSprite;
        public static Sprite greenNailgunSprite;
        public static Sprite blueSawLauncherSprite;
        public static Sprite greenSawLauncherSprite;

        public static GameObject rocketLauncherAlt;
        public static GameObject maliciousRailcannon;

        // Variables
        public static float SoliderShootAnimationStart = 1.2f;
        public static float SoliderGrenadeForce = 10000f;

        public static float SwordsMachineKnockdownTimeNormalized = 0.8f;
        public static float SwordsMachineCoreSpeed = 80f;

        public static float MinGrenadeParryVelocity = 40f;

        public static GameObject _lighningBoltSFX;
        public static GameObject lighningBoltSFX
        {
            get
            {
                if (_lighningBoltSFX == null)
                    _lighningBoltSFX = ferryman.gameObject.transform.Find("LightningBoltChimes").gameObject;

                return _lighningBoltSFX;
            }
        }

        private static bool loadedPrefabs = false;
        public void LoadPrefabs()
        {
            if (loadedPrefabs)
                return;
            loadedPrefabs = true;

            // Assets/Prefabs/Attacks and Projectiles/Projectile Spread.prefab
            projectileSpread = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Spread.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Projectile Homing.prefab
            homingProjectile = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Homing.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Projectile Decorative 2.prefab
            decorativeProjectile2 = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Decorative 2.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Grenade.prefab
            shotgunGrenade = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Grenade.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Turret Beam.prefab
            turretBeam = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Turret Beam.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Malicious Beam.prefab
            beam = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Malicious Beam.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Lightning Strike Explosive.prefab
            lightningStrikeExplosiveSetup = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Lightning Strike Explosive.prefab");
            // Assets/Particles/Environment/LightningBoltWindupFollow Variant.prefab
            lighningStrikeWindup = LoadObject<GameObject>("Assets/Particles/Environment/LightningBoltWindupFollow Variant.prefab");
            //[bundle-0][assets/prefabs/enemies/idol.prefab]
            //idol = LoadObject<GameObject>("assets/prefabs/enemies/idol.prefab");
            // Assets/Prefabs/Enemies/Ferryman.prefab
            ferryman = LoadObject<GameObject>("Assets/Prefabs/Enemies/Ferryman.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion.prefab
            explosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion.prefab");
            //Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Super.prefab
            bigExplosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Super.prefab");
            //Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sand.prefab
            sandExplosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sand.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Virtue Insignia.prefab
            virtueInsignia = LoadObject<GameObject>("f53d12327d16b8c4cb8c0ddd759db126");
            // Assets/Prefabs/Attacks and Projectiles/Projectile Explosive HH.prefab
            hideousMassProjectile = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Explosive HH.prefab");
            // Assets/Particles/Enemies/RageEffect.prefab
            enrageEffect = LoadObject<GameObject>("Assets/Particles/Enemies/RageEffect.prefab");
            // Assets/Particles/Flashes/V2FlashUnparriable.prefab
            v2flashUnparryable = LoadObject<GameObject>("Assets/Particles/Flashes/V2FlashUnparriable.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Rocket.prefab
            rocket = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Rocket.prefab");
            // Assets/Prefabs/Attacks and Projectiles/RevolverBullet.prefab
            revolverBullet = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/RevolverBullet.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Railcannon Beam Malicious.prefab
            maliciousCannonBeam = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Railcannon Beam Malicious.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam.prefab
            revolverBeam = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave Enemy.prefab
            blastwave = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave Enemy.prefab");
            // Assets/Prefabs/Enemies/MinosPrime.prefab
            minosPrime = LoadObject<GameObject>("Assets/Prefabs/Enemies/MinosPrime.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Cannonball.prefab
            cannonBall = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Cannonball.prefab");
            // get from Assets/Prefabs/Weapons/Rocket Launcher Cannonball.prefab
            cannonBallChargeAudio = LoadObject<GameObject>("Assets/Prefabs/Weapons/Rocket Launcher Cannonball.prefab").transform.Find("RocketLauncher/Armature/Body_Bone/HologramDisplay").GetComponent<AudioSource>().clip;
            // Assets/Prefabs/Attacks and Projectiles/PhysicalShockwave.prefab
            shockwave = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/PhysicalShockwave.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave Sisyphus.prefab
            sisyphiusExplosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave Sisyphus.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sisyphus Prime.prefab
            sisyphiusPrimeExplosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sisyphus Prime.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave.prefab
            explosionWaveKnuckleblaster = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Wave.prefab");
            // Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Lightning.prefab - [bundle-0][assets/prefabs/explosionlightning variant.prefab]
            lightningStrikeExplosive = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Lightning.prefab");
            // Assets/Prefabs/Weapons/Rocket Launcher Cannonball.prefab
            rocketLauncherAlt = LoadObject<GameObject>("Assets/Prefabs/Weapons/Rocket Launcher Cannonball.prefab");
            // Assets/Prefabs/Weapons/Railcannon Malicious.prefab
            maliciousRailcannon = LoadObject<GameObject>("Assets/Prefabs/Weapons/Railcannon Malicious.prefab");
            //Assets/Particles/SoundBubbles/Ricochet.prefab
            ricochetSfx = LoadObject<GameObject>("Assets/Particles/SoundBubbles/Ricochet.prefab");
            //Assets/Particles/Flashes/Flash.prefab
            parryableFlash = LoadObject<GameObject>("Assets/Particles/Flashes/Flash.prefab");
            //Assets/Prefabs/Attacks and Projectiles/Spear.prefab
            hideousMassSpear = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Spear.prefab");
            //Assets/Prefabs/Enemies/Wicked.prefab
            somethingWicked = LoadObject<GameObject>("Assets/Prefabs/Enemies/Wicked.prefab");
            //Assets/Textures/UI/SingleRevolver.png
            blueRevolverSprite = LoadObject<Sprite>("Assets/Textures/UI/SingleRevolver.png");
            //Assets/Textures/UI/RevolverSpecial.png
            greenRevolverSprite = LoadObject<Sprite>("Assets/Textures/UI/RevolverSpecial.png");
            //Assets/Textures/UI/RevolverSharp.png
            redRevolverSprite = LoadObject<Sprite>("Assets/Textures/UI/RevolverSharp.png");
            //Assets/Textures/UI/Shotgun.png
            blueShotgunSprite = LoadObject<Sprite>("Assets/Textures/UI/Shotgun.png");
            //Assets/Textures/UI/Shotgun1.png
            greenShotgunSprite = LoadObject<Sprite>("Assets/Textures/UI/Shotgun1.png");
            //Assets/Textures/UI/Nailgun2.png
            blueNailgunSprite = LoadObject<Sprite>("Assets/Textures/UI/Nailgun2.png");
            //Assets/Textures/UI/NailgunOverheat.png
            greenNailgunSprite = LoadObject<Sprite>("Assets/Textures/UI/NailgunOverheat.png");
            //Assets/Textures/UI/SawbladeLauncher.png
            blueSawLauncherSprite = LoadObject<Sprite>("Assets/Textures/UI/SawbladeLauncher.png");
            //Assets/Textures/UI/SawbladeLauncherOverheat.png
            greenSawLauncherSprite = LoadObject<Sprite>("Assets/Textures/UI/SawbladeLauncherOverheat.png");
            //Assets/Prefabs/Attacks and Projectiles/Coin.prefab
            coin = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Coin.prefab");
            //Assets/Materials/GabrielFake.mat
            gabrielFakeMat = LoadObject<Material>("Assets/Materials/GabrielFake.mat");
            //Assets/Prefabs/Enemies/Turret.prefab
            turret = LoadObject<GameObject>("Assets/Prefabs/Enemies/Turret.prefab").GetComponent<Turret>();
            //Assets/Particles/Flashes/GunFlashDistant.prefab
            turretFinalFlash = LoadObject<GameObject>("Assets/Particles/Flashes/GunFlashDistant.prefab");
            //Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sisyphus Prime Charged.prefab
            sisyphusDestroyExplosion = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Sisyphus Prime Charged.prefab");
            //Assets/Prefabs/Effects/Charge Effect.prefab
            chargeEffect = LoadObject<GameObject>("Assets/Prefabs/Effects/Charge Effect.prefab");
            //Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Malicious Beam.prefab
            maliciousFaceProjectile = LoadObject<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Malicious Beam.prefab");
        }

        public static bool ultrapainDifficulty = false;
        public static bool realUltrapainDifficulty = false;
        public static GameObject currentDifficultyButton;
        public static GameObject currentDifficultyPanel;
        public static TMPro.TextMeshProUGUI currentDifficultyInfoText;
        public void OnSceneChange(Scene before, Scene after)
        {
            StyleIDs.RegisterIDs();
            ScenePatchCheck();

            string mainMenuSceneName = "b3e7f2f8052488a45b35549efb98d902";
            string bootSequenceSceneName = "4f8ecffaa98c2614f89922daf31fa22d";
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == mainMenuSceneName)
            {
                LoadPrefabs();

                //Canvas/Difficulty Select (1)/Violent
                GameObject canvas = SceneManager.GetActiveScene().GetRootGameObjects().Where(obj => obj.name == "Canvas").FirstOrDefault();

                if (canvas == null) return;
				Transform difficultySelect = canvas.transform.Find("Difficulty Select (1)").transform.Find("Interactables");
                GameObject ultrapainButton = GameObject.Instantiate(difficultySelect.Find("Brutal").gameObject, difficultySelect);
                currentDifficultyButton = ultrapainButton;

                ultrapainButton.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = ConfigManager.pluginName.value;
                ultrapainButton.GetComponent<DifficultySelectButton>().difficulty = 6;
                RectTransform ultrapainTrans = ultrapainButton.GetComponent<RectTransform>();
                ultrapainTrans.anchoredPosition = new Vector2(20f, -250f);
                //VERY BAD NO GOOD UI CHANGE CODE
                difficultySelect.GetComponent<ObjectActivateInSequence>().delay = 0.04f;
				difficultySelect.Find("LineBreak").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 80f); //Top of MEDIUM section
				difficultySelect.Find("LineBreak (4)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -35f); //Bottom of MEDIUM section
				difficultySelect.Find("LineBreak (2)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -70f); //Top of HARD section
				difficultySelect.Find("LineBreak (5)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -185f); //Bottom of HARD section
				difficultySelect.Find("Normal").GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, 100f); //HARD Label
				difficultySelect.Find("Hard").GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, -50f); //VERY HARD Label
				difficultySelect.Find("Standard").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 50f);
				difficultySelect.Find("Violent").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -5f);
				difficultySelect.Find("Brutal").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -100f);
				difficultySelect.Find("V1 Must Die").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -155f);

                GameObject sectionTop = GameObject.Instantiate(difficultySelect.Find("LineBreak (5)").gameObject, difficultySelect);
				GameObject sectionBottom = GameObject.Instantiate(difficultySelect.Find("LineBreak (5)").gameObject, difficultySelect);
				GameObject sectionLabel = GameObject.Instantiate(difficultySelect.Find("Hard").gameObject, difficultySelect);
				sectionTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -220f);
				sectionBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -280f);
				sectionLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, -200f);
                sectionLabel.GetComponent<TextMeshProUGUI>().text = "CUSTOM";

                GameObject[] newarray = [difficultySelect.Find("Title").gameObject,difficultySelect.Find("Easy").gameObject, difficultySelect.Find("LineBreak (1)").gameObject, difficultySelect.Find("Casual Easy").gameObject, difficultySelect.Find("Casual Hard").gameObject, difficultySelect.Find("LineBreak (3)").gameObject, difficultySelect.Find("Normal").gameObject, difficultySelect.Find("LineBreak").gameObject, difficultySelect.Find("Standard").gameObject, difficultySelect.Find("Violent").gameObject, difficultySelect.Find("LineBreak (4)").gameObject, difficultySelect.Find("Hard").gameObject, difficultySelect.Find("LineBreak (2)").gameObject, difficultySelect.Find("Brutal").gameObject, difficultySelect.Find("V1 Must Die").gameObject, difficultySelect.Find("LineBreak (5)").gameObject, sectionLabel.gameObject, sectionTop.gameObject, ultrapainButton.gameObject, sectionBottom.gameObject, difficultySelect.Find("Assist Tip").gameObject];
                difficultySelect.GetComponent<ObjectActivateInSequence>().objectsToActivate = newarray;

				//Canvas/Difficulty Select (1)/Violent Info
				GameObject info = GameObject.Instantiate(difficultySelect.Find("Brutal Info").gameObject, difficultySelect);
                currentDifficultyPanel = info;
                currentDifficultyInfoText = info.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
                currentDifficultyInfoText.text = ConfigManager.pluginInfo;
                currentDifficultyInfoText.enableVertexGradient = true;
                currentDifficultyInfoText.colorGradient = new TMPro.VertexGradient(new Color(1, 0, 0, 1), new Color(1, 0, 0, 1), new Color(0.6226f, 0, 0, 1), new Color(0.6226f, 0, 0, 1));
                TMPro.TextMeshProUGUI currentDifficultyHeaderText = info.transform.Find("Title (1)").GetComponent<TMPro.TextMeshProUGUI>();
                currentDifficultyHeaderText.text = $"--{ConfigManager.pluginName.value}--";
                currentDifficultyHeaderText.autoSizeTextContainer = true;
                currentDifficultyHeaderText.enableWordWrapping = true;
                info.SetActive(false);

                EventTrigger evt = ultrapainButton.GetComponent<EventTrigger>();
                evt.triggers.Clear();

                /*EventTrigger.TriggerEvent activate = new EventTrigger.TriggerEvent();
                activate.AddListener((BaseEventData data) => info.SetActive(true));
                EventTrigger.TriggerEvent deactivate = new EventTrigger.TriggerEvent();
                activate.AddListener((BaseEventData data) => info.SetActive(false));*/

                EventTrigger.Entry trigger1 = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
                trigger1.callback.AddListener((BaseEventData data) => info.SetActive(true));
                EventTrigger.Entry trigger2 = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
                trigger2.callback.AddListener((BaseEventData data) => info.SetActive(false));

                evt.triggers.Add(trigger1);
                evt.triggers.Add(trigger2);

                foreach(EventTrigger trigger in difficultySelect.GetComponentsInChildren<EventTrigger>())
                {
                    if (trigger.gameObject == ultrapainButton)
                        continue;

                    EventTrigger.Entry closeTrigger = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
                    closeTrigger.callback.AddListener((BaseEventData data) => info.SetActive(false));
                    trigger.triggers.Add(closeTrigger);
                }
            }
            else if(currentSceneName == bootSequenceSceneName)
            {
                LoadPrefabs();

				//Canvas/Difficulty Select (1)/Violent
				GameObject canvas = SceneManager.GetActiveScene().GetRootGameObjects().Where(obj => obj.name == "Canvas").FirstOrDefault();

				if (canvas == null) return;
				Transform difficultySelect = canvas.transform.Find("Difficulty Select (1)").transform.Find("Interactables");
				GameObject ultrapainButton = GameObject.Instantiate(difficultySelect.Find("Brutal").gameObject, difficultySelect);
                currentDifficultyButton = ultrapainButton;

                ultrapainButton.transform.Find("Name").GetComponent<Text>().text = ConfigManager.pluginName.value;
                ultrapainButton.GetComponent<DifficultySelectButton>().difficulty = 6;
                RectTransform ultrapainTrans = ultrapainButton.GetComponent<RectTransform>();
				ultrapainTrans.anchoredPosition = new Vector2(20f, -250f);
                //VERY BAD NO GOOD UI CHANGE CODE
                difficultySelect.GetComponent<ObjectActivateInSequence>().delay = 0.04f;
				difficultySelect.Find("LineBreak").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 80f); //Top of MEDIUM section
				difficultySelect.Find("LineBreak (4)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -35f); //Bottom of MEDIUM section
				difficultySelect.Find("LineBreak (2)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -70f); //Top of HARD section
				difficultySelect.Find("LineBreak (5)").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -185f); //Bottom of HARD section
				difficultySelect.Find("Normal").GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, 100f); //HARD Label
				difficultySelect.Find("Hard").GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, -50f); //VERY HARD Label
				difficultySelect.Find("Standard").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 50f);
				difficultySelect.Find("Violent").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -5f);
				difficultySelect.Find("Brutal").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -100f);
				difficultySelect.Find("V1 Must Die").GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -155f);

				GameObject sectionTop = GameObject.Instantiate(difficultySelect.Find("LineBreak (5)").gameObject, difficultySelect);
				GameObject sectionBottom = GameObject.Instantiate(difficultySelect.Find("LineBreak (5)").gameObject, difficultySelect);
				GameObject sectionLabel = GameObject.Instantiate(difficultySelect.Find("Hard").gameObject, difficultySelect);
				sectionTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -220f);
				sectionBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -280f);
				sectionLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(42f, -200f);
				sectionLabel.GetComponent<TextMeshProUGUI>().text = "CUSTOM";

				GameObject[] newarray = [difficultySelect.Find("Title").gameObject, difficultySelect.Find("Easy").gameObject, difficultySelect.Find("LineBreak (1)").gameObject, difficultySelect.Find("Casual Easy").gameObject, difficultySelect.Find("Casual Hard").gameObject, difficultySelect.Find("LineBreak (3)").gameObject, difficultySelect.Find("Normal").gameObject, difficultySelect.Find("LineBreak").gameObject, difficultySelect.Find("Standard").gameObject, difficultySelect.Find("Violent").gameObject, difficultySelect.Find("LineBreak (4)").gameObject, difficultySelect.Find("Hard").gameObject, difficultySelect.Find("LineBreak (2)").gameObject, difficultySelect.Find("Brutal").gameObject, difficultySelect.Find("V1 Must Die").gameObject, difficultySelect.Find("LineBreak (5)").gameObject, sectionLabel.gameObject, sectionTop.gameObject, ultrapainButton.gameObject, sectionBottom.gameObject, difficultySelect.Find("Assist Tip").gameObject];
				difficultySelect.GetComponent<ObjectActivateInSequence>().objectsToActivate = newarray;

				//Canvas/Difficulty Select (1)/Violent Info
				GameObject info = GameObject.Instantiate(difficultySelect.Find("Brutal Info").gameObject, difficultySelect);
                currentDifficultyPanel = info;
                currentDifficultyInfoText = info.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
                currentDifficultyInfoText.text = ConfigManager.pluginInfo;
                TMPro.TextMeshProUGUI currentDifficultyHeaderText = info.transform.Find("Title (1)").GetComponent<TMPro.TextMeshProUGUI>();
                currentDifficultyHeaderText.text = $"--{ConfigManager.pluginName.value}--";
                currentDifficultyHeaderText.autoSizeTextContainer = true;
                currentDifficultyHeaderText.enableWordWrapping = true;
                info.SetActive(false);

                EventTrigger evt = ultrapainButton.GetComponent<EventTrigger>();
                evt.triggers.Clear();

                EventTrigger.TriggerEvent activate = new EventTrigger.TriggerEvent();
                activate.AddListener((BaseEventData data) => info.SetActive(true));
                EventTrigger.TriggerEvent deactivate = new EventTrigger.TriggerEvent();
                activate.AddListener((BaseEventData data) => info.SetActive(false));

                EventTrigger.Entry trigger1 = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
                trigger1.callback.AddListener((BaseEventData data) => info.SetActive(true));
                EventTrigger.Entry trigger2 = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
                trigger2.callback.AddListener((BaseEventData data) => info.SetActive(false));

                evt.triggers.Add(trigger1);
                evt.triggers.Add(trigger2);

                foreach (EventTrigger trigger in difficultySelect.GetComponentsInChildren<EventTrigger>())
                {
                    if (trigger.gameObject == ultrapainButton)
                        continue;

                    EventTrigger.Entry closeTrigger = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
                    closeTrigger.callback.AddListener((BaseEventData data) => info.SetActive(false));
                    trigger.triggers.Add(closeTrigger);
                }
            }

            // LOAD CUSTOM PREFABS HERE TO AVOID MID GAME LAG
            MinosPrimeCharge.CreateDecoy();
            GameObject shockwaveSisyphus = SisyphusInstructionist_Start.shockwave;
        }

        public static class StyleIDs
        {
            private static bool registered = false;
            public static void RegisterIDs()
            {
                registered = false;
                if (MonoSingleton<StyleHUD>.Instance == null)
                    return;

                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.grenadeBoostStyleText.guid, ConfigManager.grenadeBoostStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.rocketBoostStyleText.guid, ConfigManager.rocketBoostStyleText.formattedString);

                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.orbStrikeRevolverStyleText.guid, ConfigManager.orbStrikeRevolverStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.orbStrikeRevolverChargedStyleText.guid, ConfigManager.orbStrikeRevolverChargedStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.orbStrikeElectricCannonStyleText.guid, ConfigManager.orbStrikeElectricCannonStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.orbStrikeMaliciousCannonStyleText.guid, ConfigManager.orbStrikeMaliciousCannonStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.maliciousChargebackStyleText.guid, ConfigManager.maliciousChargebackStyleText.formattedString);
                MonoSingleton<StyleHUD>.Instance.RegisterStyleItem(ConfigManager.sentryChargebackStyleText.guid, ConfigManager.sentryChargebackStyleText.formattedString);

                registered = true;
                Debug.Log("Registered all style ids");
            }

            private static FieldInfo idNameDict = typeof(StyleHUD).GetField("idNameDict", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            public static void UpdateID(string id, string newName)
            {
                if (!registered || StyleHUD.Instance == null)
                    return;
                (idNameDict.GetValue(StyleHUD.Instance) as Dictionary<string, string>)[id] = newName;
            }
        }

        public static Harmony harmonyTweaks;
        public static Harmony harmonyBase;
        private static MethodInfo GetMethod<T>(string name)
        {
            return typeof(T).GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static Dictionary<MethodInfo, HarmonyMethod> methodCache = new Dictionary<MethodInfo, HarmonyMethod>();
        private static HarmonyMethod GetHarmonyMethod(MethodInfo method)
        {
            if (methodCache.TryGetValue(method, out HarmonyMethod harmonyMethod))
                return harmonyMethod;
            else
            {
                harmonyMethod = new HarmonyMethod(method);
                methodCache.Add(method, harmonyMethod);
                return harmonyMethod;
            }
        }

        private static void PatchAllEnemies()
        {
            if (!ConfigManager.enemyTweakToggle.value)
                return;

            if (ConfigManager.friendlyFireDamageOverrideToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(GetMethod<Explosion_Collide_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Explosion_Collide_FF>("Postfix")));
                harmonyTweaks.Patch(GetMethod<PhysicalShockwave>("CheckCollision"), prefix: GetHarmonyMethod(GetMethod<PhysicalShockwave_CheckCollision_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<PhysicalShockwave_CheckCollision_FF>("Postfix")));
                harmonyTweaks.Patch(GetMethod<VirtueInsignia>("OnTriggerEnter"), prefix: GetHarmonyMethod(GetMethod<VirtueInsignia_OnTriggerEnter_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<VirtueInsignia_OnTriggerEnter_FF>("Postfix")));
                harmonyTweaks.Patch(GetMethod<SwingCheck2>("CheckCollision"), prefix: GetHarmonyMethod(GetMethod<SwingCheck2_CheckCollision_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<SwingCheck2_CheckCollision_FF>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Projectile>("Collided"), prefix: GetHarmonyMethod(GetMethod<Projectile_Collided_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Projectile_Collided_FF>("Postfix")));

                harmonyTweaks.Patch(GetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(GetMethod<EnemyIdentifier_DeliverDamage_FF>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Flammable>("Burn"), prefix: GetHarmonyMethod(GetMethod<Flammable_Burn_FF>("Prefix")));
                harmonyTweaks.Patch(GetMethod<FireZone>("OnTriggerStay"), prefix: GetHarmonyMethod(GetMethod<StreetCleaner_Fire_FF>("Prefix")), postfix: GetHarmonyMethod(GetMethod<StreetCleaner_Fire_FF>("Postfix")));
            }

            harmonyTweaks.Patch(GetMethod<EnemyIdentifier>("UpdateModifiers"), postfix: GetHarmonyMethod(GetMethod<EnemyIdentifier_UpdateModifiers>("Postfix")));

            harmonyTweaks.Patch(GetMethod<StatueBoss>("Start"), postfix: GetHarmonyMethod(GetMethod<StatueBoss_Start_Patch>("Postfix")));
            if (ConfigManager.cerberusDashToggle.value)
                harmonyTweaks.Patch(GetMethod<StatueBoss>("StopDash"), postfix: GetHarmonyMethod(GetMethod<StatueBoss_StopDash_Patch>("Postfix")));
            if(ConfigManager.cerberusParryable.value)
            {
                harmonyTweaks.Patch(GetMethod<StatueBoss>("StopTracking"), postfix: GetHarmonyMethod(GetMethod<StatueBoss_StopTracking_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<StatueBoss>("Stomp"), postfix: GetHarmonyMethod(GetMethod<StatueBoss_Stomp_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Statue>("GetHurt"), prefix: GetHarmonyMethod(GetMethod<Statue_GetHurt_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(GetMethod<Drone>("Start"), postfix: GetHarmonyMethod(GetMethod<Drone_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(GetMethod<Drone>("Shoot"), prefix: GetHarmonyMethod(GetMethod<Drone_Shoot_Patch>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Drone>("PlaySound"), prefix: GetHarmonyMethod(GetMethod<Drone_PlaySound_Patch>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Drone>("Update"), postfix: GetHarmonyMethod(GetMethod<Drone_Update>("Postfix")));
            if(ConfigManager.droneHomeToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Drone>("Death"), prefix: GetHarmonyMethod(GetMethod<Drone_Death_Patch>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Drone>("GetHurt"), prefix: GetHarmonyMethod(GetMethod<Drone_GetHurt_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(GetMethod<Ferryman>("Start"), postfix: GetHarmonyMethod(GetMethod<FerrymanStart>("Postfix")));
            if(ConfigManager.ferrymanComboToggle.value)
                harmonyTweaks.Patch(GetMethod<Ferryman>("StopMoving"), postfix: GetHarmonyMethod(GetMethod<FerrymanStopMoving>("Postfix")));

            if(ConfigManager.filthExplodeToggle.value)
                harmonyTweaks.Patch(GetMethod<SwingCheck2>("CheckCollision"), prefix: GetHarmonyMethod(GetMethod<SwingCheck2_CheckCollision_Patch2>("Prefix")));

            if(ConfigManager.fleshPrisonSpinAttackToggle.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("HomingProjectileAttack"), postfix: GetHarmonyMethod(GetMethod<FleshPrisonShoot>("Postfix")));

            if (ConfigManager.hideousMassInsigniaToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Projectile>("Explode"), postfix: GetHarmonyMethod(GetMethod<Projectile_Explode_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Mass>("ShootExplosive"), postfix: GetHarmonyMethod(GetMethod<HideousMassHoming>("Postfix")), prefix: GetHarmonyMethod(GetMethod<HideousMassHoming>("Prefix")));
            }

            harmonyTweaks.Patch(GetMethod<SpiderBody>("Start"), postfix: GetHarmonyMethod(GetMethod<MaliciousFace_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(GetMethod<SpiderBody>("ChargeBeam"), postfix: GetHarmonyMethod(GetMethod<MaliciousFace_ChargeBeam>("Postfix")));
            harmonyTweaks.Patch(GetMethod<SpiderBody>("BeamChargeEnd"), prefix: GetHarmonyMethod(GetMethod<MaliciousFace_BeamChargeEnd>("Prefix")));
            if (ConfigManager.maliciousFaceHomingProjectileToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<SpiderBody>("ShootProj"), postfix: GetHarmonyMethod(GetMethod<MaliciousFace_ShootProj_Patch>("Postfix")));
            }
            if (ConfigManager.maliciousFaceRadianceOnEnrage.value)
                harmonyTweaks.Patch(GetMethod<SpiderBody>("Enrage"), postfix: GetHarmonyMethod(GetMethod<MaliciousFace_Enrage_Patch>("Postfix")));

            harmonyTweaks.Patch(GetMethod<Mindflayer>("Start"), postfix: GetHarmonyMethod(GetMethod<Mindflayer_Start_Patch>("Postfix")));
            if (ConfigManager.mindflayerShootTweakToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Mindflayer>("ShootProjectiles"), prefix: GetHarmonyMethod(GetMethod<Mindflayer_ShootProjectiles_Patch>("Prefix")));
                harmonyTweaks.Patch(GetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(GetMethod<EnemyIdentifier_DeliverDamage_MF>("Prefix")));
            }
            if (ConfigManager.mindflayerTeleportComboToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<SwingCheck2>("CheckCollision"), postfix: GetHarmonyMethod(GetMethod<SwingCheck2_CheckCollision_Patch>("Postfix")), prefix: GetHarmonyMethod(GetMethod<SwingCheck2_CheckCollision_Patch>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Mindflayer>("MeleeTeleport"), prefix: GetHarmonyMethod(GetMethod<Mindflayer_MeleeTeleport_Patch>("Prefix")));
                //harmonyTweaks.Patch(GetMethod<SwingCheck2>("DamageStop"), postfix: GetHarmonyMethod(GetMethod<SwingCheck2_DamageStop_Patch>("Postfix")));
            }

            if (ConfigManager.minosPrimeRandomTeleportToggle.value)
                harmonyTweaks.Patch(GetMethod<MinosPrime>("ProjectileCharge"), postfix: GetHarmonyMethod(GetMethod<MinosPrimeCharge>("Postfix")));
            if (ConfigManager.minosPrimeTeleportTrail.value)
                harmonyTweaks.Patch(GetMethod<MinosPrime>("Teleport"), postfix: GetHarmonyMethod(GetMethod<MinosPrimeCharge>("TeleportPostfix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("Start"), postfix: GetHarmonyMethod(GetMethod<MinosPrime_Start>("Postfix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("Dropkick"), prefix: GetHarmonyMethod(GetMethod<MinosPrime_Dropkick>("Prefix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("Combo"), postfix: GetHarmonyMethod(GetMethod<MinosPrime_Combo>("Postfix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("StopAction"), postfix: GetHarmonyMethod(GetMethod<MinosPrime_StopAction>("Postfix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("Ascend"), prefix: GetHarmonyMethod(GetMethod<MinosPrime_Ascend>("Prefix")));
            harmonyTweaks.Patch(GetMethod<MinosPrime>("Death"), prefix: GetHarmonyMethod(GetMethod<MinosPrime_Death>("Prefix")));
            if (ConfigManager.minosPrimeCrushAttackToggle.value)
                harmonyTweaks.Patch(GetMethod<MinosPrime>("RiderKick"), prefix: GetHarmonyMethod(GetMethod<MinosPrime_RiderKick>("Prefix")));
            if (ConfigManager.minosPrimeComboExplosiveEndToggle.value)
                harmonyTweaks.Patch(GetMethod<MinosPrime>("ProjectileCharge"), prefix: GetHarmonyMethod(GetMethod<MinosPrime_ProjectileCharge>("Prefix")));

            if (ConfigManager.schismSpreadAttackToggle.value)
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("ShootProjectile"), postfix: GetHarmonyMethod(GetMethod<ZombieProjectile_ShootProjectile_Patch>("Postfix")));

            if (ConfigManager.soliderShootTweakToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("Start"), postfix: GetHarmonyMethod(GetMethod<Solider_Start_Patch>("Postfix")));
            }
            if(ConfigManager.soliderCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("SpawnProjectile"), postfix: GetHarmonyMethod(GetMethod<Solider_SpawnProjectile_Patch>("Postfix")));
            if (ConfigManager.soliderShootGrenadeToggle.value || ConfigManager.soliderShootTweakToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("ThrowProjectile"), postfix: GetHarmonyMethod(GetMethod<Solider_ThrowProjectile_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Grenade>("Explode"), postfix: GetHarmonyMethod(GetMethod<Grenade_Explode_Patch>("Postfix")), prefix: GetHarmonyMethod(GetMethod<Grenade_Explode_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(GetMethod<Stalker>("SandExplode"), prefix: GetHarmonyMethod(GetMethod<Stalker_SandExplode_Patch>("Prefix")));
            harmonyTweaks.Patch(GetMethod<SandificationZone>("Enter"), postfix: GetHarmonyMethod(GetMethod<SandificationZone_Enter_Patch>("Postfix")));

            if (ConfigManager.strayCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("SpawnProjectile"), postfix: GetHarmonyMethod(GetMethod<Swing>("Postfix")));
            if (ConfigManager.strayShootToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("Start"), postfix: GetHarmonyMethod(GetMethod<ZombieProjectile_Start_Patch1>("Postfix")));
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("ThrowProjectile"), postfix: GetHarmonyMethod(GetMethod<ZombieProjectile_ThrowProjectile_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("SwingEnd"), prefix: GetHarmonyMethod(GetMethod<SwingEnd>("Prefix")));
                harmonyTweaks.Patch(GetMethod<ZombieProjectiles>("DamageEnd"), prefix: GetHarmonyMethod(GetMethod<DamageEnd>("Prefix")));
            }

            if(ConfigManager.streetCleanerCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(GetMethod<Streetcleaner>("Start"), postfix: GetHarmonyMethod(GetMethod<StreetCleaner_Start_Patch>("Postfix")));
            if(ConfigManager.streetCleanerPredictiveDodgeToggle.value)
                harmonyTweaks.Patch(GetMethod<BulletCheck>("OnTriggerEnter"), postfix: GetHarmonyMethod(GetMethod<BulletCheck_OnTriggerEnter_Patch>("Postfix")));

            harmonyTweaks.Patch(GetMethod<SwordsMachine>("Start"), postfix: GetHarmonyMethod(GetMethod<SwordsMachine_Start>("Postfix")));
            if (ConfigManager.swordsMachineNoLightKnockbackToggle.value || ConfigManager.swordsMachineSecondPhaseMode.value != ConfigManager.SwordsMachineSecondPhase.None)
            {
                harmonyTweaks.Patch(GetMethod<SwordsMachine>("Knockdown"), prefix: GetHarmonyMethod(GetMethod<SwordsMachine_Knockdown_Patch>("Prefix")));
                harmonyTweaks.Patch(GetMethod<SwordsMachine>("Down"), postfix: GetHarmonyMethod(GetMethod<SwordsMachine_Down_Patch>("Postfix")), prefix: GetHarmonyMethod(GetMethod<SwordsMachine_Down_Patch>("Prefix")));
                //harmonyTweaks.Patch(GetMethod<SwordsMachine>("SetSpeed"), prefix: GetHarmonyMethod(GetMethod<SwordsMachine_SetSpeed_Patch>("Prefix")));
                harmonyTweaks.Patch(GetMethod<SwordsMachine>("EndFirstPhase"), postfix: GetHarmonyMethod(GetMethod<SwordsMachine_EndFirstPhase_Patch>("Postfix")), prefix: GetHarmonyMethod(GetMethod<SwordsMachine_EndFirstPhase_Patch>("Prefix")));
            }
            if (ConfigManager.swordsMachineExplosiveSwordToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<ThrownSword>("Start"), postfix: GetHarmonyMethod(GetMethod<ThrownSword_Start_Patch>("Postfix")));
                harmonyTweaks.Patch(GetMethod<ThrownSword>("OnTriggerEnter"), postfix: GetHarmonyMethod(GetMethod<ThrownSword_OnTriggerEnter_Patch>("Postfix")));
            }

            harmonyTweaks.Patch(GetMethod<Turret>("Start"), postfix: GetHarmonyMethod(GetMethod<TurretStart>("Postfix")));
            if(ConfigManager.turretBurstFireToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Turret>("Shoot"), prefix: GetHarmonyMethod(GetMethod<TurretShoot>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Turret>("StartAiming"), postfix: GetHarmonyMethod(GetMethod<TurretAim>("Postfix")));
            }

            harmonyTweaks.Patch(GetMethod<Explosion>("Start"), postfix: GetHarmonyMethod(GetMethod<V2CommonExplosion>("Postfix")));

            harmonyTweaks.Patch(GetMethod<V2>("Start"), postfix: GetHarmonyMethod(GetMethod<V2FirstStart>("Postfix")));
            harmonyTweaks.Patch(GetMethod<V2>("Update"), prefix: GetHarmonyMethod(GetMethod<V2FirstUpdate>("Prefix")));
            harmonyTweaks.Patch(GetMethod<V2>("ShootWeapon"), prefix: GetHarmonyMethod(GetMethod<V2FirstShootWeapon>("Prefix")));

            harmonyTweaks.Patch(GetMethod<V2>("Start"), postfix: GetHarmonyMethod(GetMethod<V2SecondStart>("Postfix")));
            //if(ConfigManager.v2SecondStartEnraged.value)
            //    harmonyTweaks.Patch(GetMethod<BossHealthBar>("OnEnable"), postfix: GetHarmonyMethod(GetMethod<V2SecondEnrage>("Postfix")));
            harmonyTweaks.Patch(GetMethod<V2>("Update"), prefix: GetHarmonyMethod(GetMethod<V2SecondUpdate>("Prefix")));
            //harmonyTweaks.Patch(GetMethod<V2>("AltShootWeapon"), postfix: GetHarmonyMethod(GetMethod<V2AltShootWeapon>("Postfix")));
            harmonyTweaks.Patch(GetMethod<V2>("SwitchWeapon"), prefix: GetHarmonyMethod(GetMethod<V2SecondSwitchWeapon>("Prefix")));
            harmonyTweaks.Patch(GetMethod<V2>("ShootWeapon"), prefix: GetHarmonyMethod(GetMethod<V2SecondShootWeapon>("Prefix")), postfix: GetHarmonyMethod(GetMethod<V2SecondShootWeapon>("Postfix")));
            if(ConfigManager.v2SecondFastCoinToggle.value)
                harmonyTweaks.Patch(GetMethod<V2>("ThrowCoins"), prefix: GetHarmonyMethod(GetMethod<V2SecondFastCoin>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Cannonball>("OnTriggerEnter"), prefix: GetHarmonyMethod(GetMethod<V2RocketLauncher>("CannonBallTriggerPrefix")));

            if (ConfigManager.v2FirstSharpshooterToggle.value || ConfigManager.v2SecondSharpshooterToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<EnemyRevolver>("PrepareAltFire"), prefix: GetHarmonyMethod(GetMethod<V2CommonRevolverPrepareAltFire>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Projectile>("Collided"), prefix: GetHarmonyMethod(GetMethod<V2CommonRevolverBullet>("Prefix")));
                harmonyTweaks.Patch(GetMethod<EnemyRevolver>("AltFire"), prefix: GetHarmonyMethod(GetMethod<V2CommonRevolverAltShoot>("Prefix")));
            }

            harmonyTweaks.Patch(GetMethod<Drone>("Start"), postfix: GetHarmonyMethod(GetMethod<Virtue_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(GetMethod<Drone>("SpawnInsignia"), prefix: GetHarmonyMethod(GetMethod<Virtue_SpawnInsignia_Patch>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Drone>("Death"), prefix: GetHarmonyMethod(GetMethod<Virtue_Death_Patch>("Prefix")));

            if (ConfigManager.sisyInstJumpShockwave.value)
            {
                harmonyTweaks.Patch(GetMethod<Sisyphus>("Start"), postfix: GetHarmonyMethod(GetMethod<SisyphusInstructionist_Start>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Sisyphus>("Update"), postfix: GetHarmonyMethod(GetMethod<SisyphusInstructionist_Update>("Postfix")));
            }
            if(ConfigManager.sisyInstBoulderShockwave.value)
                harmonyTweaks.Patch(GetMethod<Sisyphus>("SetupExplosion"), postfix: GetHarmonyMethod(GetMethod<SisyphusInstructionist_SetupExplosion>("Postfix")));
            if(ConfigManager.sisyInstStrongerExplosion.value)
                harmonyTweaks.Patch(GetMethod<Sisyphus>("StompExplosion"), prefix: GetHarmonyMethod(GetMethod<SisyphusInstructionist_StompExplosion>("Prefix")));

            harmonyTweaks.Patch(GetMethod<LeviathanTail>("Awake"), postfix: GetHarmonyMethod(GetMethod<LeviathanTail_Start>("Postfix")));
            harmonyTweaks.Patch(GetMethod<LeviathanTail>("BigSplash"), prefix: GetHarmonyMethod(GetMethod<LeviathanTail_BigSplash>("Prefix")));
            harmonyTweaks.Patch(GetMethod<LeviathanTail>("SwingEnd"), prefix: GetHarmonyMethod(GetMethod<LeviathanTail_SwingEnd>("Prefix")));

            harmonyTweaks.Patch(GetMethod<LeviathanHead>("Start"), postfix: GetHarmonyMethod(GetMethod<Leviathan_Start>("Postfix")));
            harmonyTweaks.Patch(GetMethod<LeviathanHead>("ProjectileBurst"), prefix: GetHarmonyMethod(GetMethod<Leviathan_ProjectileBurst>("Prefix")));
            harmonyTweaks.Patch(GetMethod<LeviathanHead>("ProjectileBurstStart"), prefix: GetHarmonyMethod(GetMethod<Leviathan_ProjectileBurstStart>("Prefix")));
            harmonyTweaks.Patch(GetMethod<LeviathanHead>("FixedUpdate"), prefix: GetHarmonyMethod(GetMethod<Leviathan_FixedUpdate>("Prefix")));

            if (ConfigManager.somethingWickedSpear.value)
            {
                harmonyTweaks.Patch(GetMethod<Wicked>("Start"), postfix: GetHarmonyMethod(GetMethod<SomethingWicked_Start>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Wicked>("GetHit"), postfix: GetHarmonyMethod(GetMethod<SomethingWicked_GetHit>("Postfix")));
            }
            if(ConfigManager.somethingWickedSpawnOn43.value)
            {
                harmonyTweaks.Patch(GetMethod<ObjectActivator>("Activate"), prefix: GetHarmonyMethod(GetMethod<ObjectActivator_Activate>("Prefix")));
            }

            if (ConfigManager.panopticonFullPhase.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(GetMethod<Panopticon_Start>("Postfix")));
            if (ConfigManager.panopticonAxisBeam.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("SpawnInsignia"), prefix: GetHarmonyMethod(GetMethod<Panopticon_SpawnInsignia>("Prefix")));
            if (ConfigManager.panopticonSpinAttackToggle.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("HomingProjectileAttack"), postfix: GetHarmonyMethod(GetMethod<Panopticon_HomingProjectileAttack>("Postfix")));
            if (ConfigManager.panopticonBlackholeProj.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("SpawnBlackHole"), postfix: GetHarmonyMethod(GetMethod<Panopticon_SpawnBlackHole>("Postfix")));
            if (ConfigManager.panopticonBalanceEyes.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("SpawnFleshDrones"), prefix: GetHarmonyMethod(GetMethod<Panopticon_SpawnFleshDrones>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Panopticon_SpawnFleshDrones>("Postfix")));
            if (ConfigManager.panopticonBlueProjToggle.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("Update"), transpiler: GetHarmonyMethod(GetMethod<Panopticon_BlueProjectile>("Transpiler")));

            if (ConfigManager.idolExplosionToggle.value)
                harmonyTweaks.Patch(GetMethod<Idol>("Death"), postfix: GetHarmonyMethod(GetMethod<Idol_Death_Patch>("Postfix")));

            // ADDME
            /*
            harmonyTweaks.Patch(GetMethod<GabrielSecond>("Start"), postfix: GetHarmonyMethod(GetMethod<GabrielSecond_Start>("Postfix")));
            harmonyTweaks.Patch(GetMethod<GabrielSecond>("BasicCombo"), postfix: GetHarmonyMethod(GetMethod<GabrielSecond_BasicCombo>("Postfix")));
            harmonyTweaks.Patch(GetMethod<GabrielSecond>("FastCombo"), postfix: GetHarmonyMethod(GetMethod<GabrielSecond_FastCombo>("Postfix")));
            harmonyTweaks.Patch(GetMethod<GabrielSecond>("CombineSwords"), postfix: GetHarmonyMethod(GetMethod<GabrielSecond_CombineSwords>("Postfix")));
            harmonyTweaks.Patch(GetMethod<GabrielSecond>("ThrowCombo"), postfix: GetHarmonyMethod(GetMethod<GabrielSecond_ThrowCombo>("Postfix")));
            */
        }

        private static void PatchAllPlayers()
        {
            if (ConfigManager.crossmodSupport_MD.value == true)
            {
                harmonyTweaks.Patch(GetMethod<Explosion>("Start"), prefix: GetHarmonyMethod(GetMethod<CrossmodSupport_MD_SplendorConductor>("Prefix")));
                harmonyTweaks.Patch(GetMethod<EnemyIdentifier>("Update"), prefix: GetHarmonyMethod(GetMethod<EnemyIdentifier_DeliverDamage_Crossmod>("Prefix")));
            }
            if (!ConfigManager.playerTweakToggle.value)
                return;
            harmonyTweaks.Patch(GetMethod<Punch>("TryParryProjectile"), prefix: GetHarmonyMethod(GetMethod<Punch_TryParryProjectile_Patch>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Grenade>("Explode"), prefix: GetHarmonyMethod(GetMethod<Grenade_Explode_Patch1>("Prefix")));
            harmonyTweaks.Patch(GetMethod<Grenade>("Collision"), prefix: GetHarmonyMethod(GetMethod<Grenade_Collision_Patch>("Prefix")));
            if (ConfigManager.rocketBoostToggle.value)
                harmonyTweaks.Patch(GetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(GetMethod<Explosion_Collide_Patch>("Prefix")));

            if (ConfigManager.rocketGrabbingToggle.value)
                harmonyTweaks.Patch(GetMethod<HookArm>("FixedUpdate"), prefix: GetHarmonyMethod(GetMethod<HookArm_FixedUpdate_Patch>("Prefix")));

            if (ConfigManager.orbStrikeToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Coin>("Start"), postfix: GetHarmonyMethod(GetMethod<Coin_Start>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Punch>("BlastCheck"), prefix: GetHarmonyMethod(GetMethod<Punch_BlastCheck>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Punch_BlastCheck>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(GetMethod<Explosion_Collide>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Coin>("DelayedReflectRevolver"), postfix: GetHarmonyMethod(GetMethod<Coin_DelayedReflectRevolver>("Postfix")));
                harmonyTweaks.Patch(GetMethod<Coin>("ReflectRevolver"), postfix: GetHarmonyMethod(GetMethod<Coin_ReflectRevolver>("Postfix")), prefix: GetHarmonyMethod(GetMethod<Coin_ReflectRevolver>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Grenade>("Explode"), prefix: GetHarmonyMethod(GetMethod<Grenade_Explode>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Grenade_Explode>("Postfix")));
                
                harmonyTweaks.Patch(GetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(GetMethod<EnemyIdentifier_DeliverDamage>("Prefix")), postfix: GetHarmonyMethod(GetMethod<EnemyIdentifier_DeliverDamage>("Postfix")));
                harmonyTweaks.Patch(GetMethod<RevolverBeam>("ExecuteHits"), postfix: GetHarmonyMethod(GetMethod<RevolverBeam_ExecuteHits>("Postfix")), prefix: GetHarmonyMethod(GetMethod<RevolverBeam_ExecuteHits>("Prefix")));
                harmonyTweaks.Patch(GetMethod<RevolverBeam>("HitSomething"), postfix: GetHarmonyMethod(GetMethod<RevolverBeam_HitSomething>("Postfix")), prefix: GetHarmonyMethod(GetMethod<RevolverBeam_HitSomething>("Prefix")));
                harmonyTweaks.Patch(GetMethod<RevolverBeam>("Start"), prefix: GetHarmonyMethod(GetMethod<RevolverBeam_Start>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Cannonball>("Explode"), prefix: GetHarmonyMethod(GetMethod<Cannonball_Explode>("Prefix")));

                harmonyTweaks.Patch(GetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(GetMethod<Explosion_CollideOrbital>("Prefix")));
            }
            
            if(ConfigManager.chargedRevRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(GetMethod<Revolver>("Update"), prefix: GetHarmonyMethod(GetMethod<Revolver_Update>("Prefix")));
            if(ConfigManager.coinRegSpeedMulti.value != 1 || ConfigManager.sharpshooterRegSpeedMulti.value != 1
                || ConfigManager.railcannonRegSpeedMulti.value != 1 || ConfigManager.rocketFreezeRegSpeedMulti.value != 1
                || ConfigManager.rocketCannonballRegSpeedMulti.value != 1 || ConfigManager.nailgunAmmoRegSpeedMulti.value != 1
                || ConfigManager.sawAmmoRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(GetMethod<WeaponCharges>("Charge"), prefix: GetHarmonyMethod(GetMethod<WeaponCharges_Charge>("Prefix")));
            if(ConfigManager.nailgunHeatsinkRegSpeedMulti.value != 1 || ConfigManager.sawHeatsinkRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(GetMethod<Nailgun>("Update"), prefix: GetHarmonyMethod(GetMethod<NailGun_Update>("Prefix")));
            if(ConfigManager.staminaRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(GetMethod<NewMovement>("Update"), prefix: GetHarmonyMethod(GetMethod<NewMovement_Update>("Prefix")));
            
            if(ConfigManager.playerHpDeltaToggle.value || ConfigManager.maxPlayerHp.value != 100 || ConfigManager.playerHpSupercharge.value != 200 || ConfigManager.whiplashHardDamageCap.value != 50 || ConfigManager.whiplashHardDamageSpeed.value != 1)
            {
                harmonyTweaks.Patch(GetMethod<NewMovement>("GetHealth"), prefix: GetHarmonyMethod(GetMethod<NewMovement_GetHealth>("Prefix")));
                harmonyTweaks.Patch(GetMethod<NewMovement>("SuperCharge"), prefix: GetHarmonyMethod(GetMethod<NewMovement_SuperCharge>("Prefix")));
                harmonyTweaks.Patch(GetMethod<NewMovement>("Respawn"), postfix: GetHarmonyMethod(GetMethod<NewMovement_Respawn>("Postfix")));
                harmonyTweaks.Patch(GetMethod<NewMovement>("Start"), postfix: GetHarmonyMethod(GetMethod<NewMovement_Start>("Postfix")));
                harmonyTweaks.Patch(GetMethod<NewMovement>("GetHurt"), transpiler: GetHarmonyMethod(GetMethod<NewMovement_GetHurt>("Transpiler")));
                harmonyTweaks.Patch(GetMethod<HookArm>("FixedUpdate"), transpiler: GetHarmonyMethod(GetMethod<HookArm_FixedUpdate>("Transpiler")));
                harmonyTweaks.Patch(GetMethod<NewMovement>("ForceAntiHP"), transpiler: GetHarmonyMethod(GetMethod<NewMovement_ForceAntiHP>("Transpiler")));
            }

            // ADDME
            harmonyTweaks.Patch(GetMethod<Revolver>("Shoot"), transpiler: GetHarmonyMethod(GetMethod<Revolver_Shoot>("Transpiler")));
            harmonyTweaks.Patch(GetMethod<Shotgun>("Shoot"), transpiler: GetHarmonyMethod(GetMethod<Shotgun_Shoot>("Transpiler")), prefix: GetHarmonyMethod(GetMethod<Shotgun_Shoot>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Shotgun_Shoot>("Postfix")));
            harmonyTweaks.Patch(GetMethod<Shotgun>("ShootSinks"), transpiler: GetHarmonyMethod(GetMethod<Shotgun_ShootSinks>("Transpiler")));
            harmonyTweaks.Patch(GetMethod<Nailgun>("Shoot"), transpiler: GetHarmonyMethod(GetMethod<Nailgun_Shoot>("Transpiler")));
            harmonyTweaks.Patch(GetMethod<Nailgun>("SuperSaw"), transpiler: GetHarmonyMethod(GetMethod<Nailgun_SuperSaw>("Transpiler")));
            
            if (ConfigManager.hardDamagePercent.normalizedValue != 1)
                harmonyTweaks.Patch(GetMethod<NewMovement>("GetHurt"), prefix: GetHarmonyMethod(GetMethod<NewMovement_GetHurt>("Prefix")), postfix: GetHarmonyMethod(GetMethod<NewMovement_GetHurt>("Postfix")));

            harmonyTweaks.Patch(GetMethod<HealthBar>("Start"), postfix: GetHarmonyMethod(GetMethod<HealthBar_Start>("Postfix")));
			harmonyTweaks.Patch(GetMethod<HealthBar>("Update"), transpiler: GetHarmonyMethod(GetMethod<HealthBar_Update>("Transpiler")));
			foreach (HealthBarTracker hb in HealthBarTracker.instances)
            {
                if (hb != null)
                    hb.SetSliderRange();
            }
            
            harmonyTweaks.Patch(GetMethod<Harpoon>("Start"), postfix: GetHarmonyMethod(GetMethod<Harpoon_Start>("Postfix")));
            if(ConfigManager.screwDriverHomeToggle.value)
                harmonyTweaks.Patch(GetMethod<Harpoon>("Punched"), postfix: GetHarmonyMethod(GetMethod<Harpoon_Punched>("Postfix")));
            if(ConfigManager.screwDriverSplitToggle.value)
                harmonyTweaks.Patch(GetMethod<Harpoon>("OnTriggerEnter"), prefix: GetHarmonyMethod(GetMethod<Harpoon_OnTriggerEnter_Patch>("Prefix")));
        }

        private static void PatchAllMemes()
        {
            if (ConfigManager.enrageSfxToggle.value)
                harmonyTweaks.Patch(GetMethod<EnrageEffect>("Start"), postfix: GetHarmonyMethod(GetMethod<EnrageEffect_Start>("Postfix")));
            
            if(ConfigManager.funnyDruidKnightSFXToggle.value)
            {
                harmonyTweaks.Patch(GetMethod<Mandalore>("FullBurst"), postfix: GetHarmonyMethod(GetMethod<DruidKnight_FullBurst>("Postfix")), prefix: GetHarmonyMethod(GetMethod<DruidKnight_FullBurst>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Mandalore>("FullerBurst"), prefix: GetHarmonyMethod(GetMethod<DruidKnight_FullerBurst>("Prefix")));
                harmonyTweaks.Patch(GetMethod<Drone>("Explode"), prefix: GetHarmonyMethod(GetMethod<Drone_Explode>("Prefix")), postfix: GetHarmonyMethod(GetMethod<Drone_Explode>("Postfix")));
            }

            if (ConfigManager.fleshObamiumToggle.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(GetMethod<FleshObamium_Start>("Postfix")), prefix: GetHarmonyMethod(GetMethod<FleshObamium_Start>("Prefix")));
            if (ConfigManager.obamapticonToggle.value)
                harmonyTweaks.Patch(GetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(GetMethod<Obamapticon_Start>("Postfix")), prefix: GetHarmonyMethod(GetMethod<Obamapticon_Start>("Prefix")));
        }

        public static bool methodsPatched = false;
        
        public static void ScenePatchCheck()
        {
            if(methodsPatched && !ultrapainDifficulty)
            {
                harmonyTweaks.UnpatchSelf();
                methodsPatched = false;
            }
            else if(!methodsPatched && ultrapainDifficulty)
            {
                PatchAll();
            }
        }
        
        public static void PatchAll()
        {
            harmonyTweaks.UnpatchSelf();
            methodsPatched = false;

            if (!ultrapainDifficulty)
                return;

            
            if(realUltrapainDifficulty && ConfigManager.discordRichPresenceToggle.value)
                harmonyTweaks.Patch(GetMethod<DiscordController>("SendActivity"), prefix: GetHarmonyMethod(GetMethod<DiscordController_SendActivity_Patch>("Prefix")));
            if (realUltrapainDifficulty && ConfigManager.steamRichPresenceToggle.value)
                harmonyTweaks.Patch(GetMethod<SteamController>("FetchSceneActivity"), postfix: GetHarmonyMethod(GetMethod<SteamController_FetchSceneActivity_Patch>("Postfix")));

            PatchAllEnemies();
            PatchAllPlayers();
            PatchAllMemes();
            methodsPatched = true;
        }

        public static string workingPath;
        public static string workingDir;

        public static AssetBundle bundle;
        public static AudioClip druidKnightFullAutoAud;
        public static AudioClip druidKnightFullerAutoAud;
        public static AudioClip druidKnightDeathAud;
        public static AudioClip enrageAudioCustom;
        public static GameObject fleshObamium;
        public static GameObject obamapticon;

        public void Awake()
        {
            instance = this;
            workingPath = Assembly.GetExecutingAssembly().Location;
            workingDir = Path.GetDirectoryName(workingPath);

            Logger.LogInfo($"Working path: {workingPath}, Working dir: {workingDir}");
            try
            {
                bundle = AssetBundle.LoadFromFile(Path.Combine(workingDir, "ultrapain"));
                druidKnightFullAutoAud = bundle.LoadAsset<AudioClip>("assets/ultrapain/druidknight/fullauto.wav");
                druidKnightFullerAutoAud = bundle.LoadAsset<AudioClip>("assets/ultrapain/druidknight/fullerauto.wav");
                druidKnightDeathAud = bundle.LoadAsset<AudioClip>("assets/ultrapain/druidknight/death.wav");
                enrageAudioCustom = bundle.LoadAsset<AudioClip>("assets/ultrapain/sfx/enraged.wav");
                fleshObamium = bundle.LoadAsset<GameObject>("assets/ultrapain/fleshprison/fleshobamium.prefab");
                obamapticon = bundle.LoadAsset<GameObject>("assets/ultrapain/panopticon/obamapticon.prefab");
            }
            catch (Exception e)
            {
                Logger.LogError($"Could not load the asset bundle:\n{e}");
            }

            // DEBUG
            /*string logPath = Path.Combine(Environment.CurrentDirectory, "log.txt");
            Logger.LogInfo($"Saving to {logPath}");
            List<string> assetPaths = new List<string>()
            {
                "fonts.bundle",
                "videos.bundle",
                "shaders.bundle",
                "particles.bundle",
                "materials.bundle",
                "animations.bundle",
                "prefabs.bundle",
                "physicsmaterials.bundle",
                "models.bundle",
                "textures.bundle",
            };

            //using (FileStream log = File.Open(logPath, FileMode.OpenOrCreate, FileAccess.Write))
            //{
                foreach(string assetPath in assetPaths)
                {
                    Logger.LogInfo($"Attempting to load {assetPath}");
                    AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(bundlePath, assetPath));
                    bundles.Add(bundle);
                    //foreach (string name in bundle.GetAllAssetNames())
                    //{
                    //    string line = $"[{bundle.name}][{name}]\n";
                    //    log.Write(Encoding.ASCII.GetBytes(line), 0, line.Length);
                    //}
                    bundle.LoadAllAssets();
                }
            //}
            */

            // Plugin startup logic 
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            harmonyTweaks = new Harmony(PLUGIN_GUID + "_tweaks");
            harmonyBase = new Harmony(PLUGIN_GUID + "_base");
            harmonyBase.Patch(GetMethod<DifficultySelectButton>("SetDifficulty"), postfix: GetHarmonyMethod(GetMethod<DifficultySelectPatch>("Postfix")));
            harmonyBase.Patch(GetMethod<DifficultyTitle>("Check"), postfix: GetHarmonyMethod(GetMethod<DifficultyTitle_Check_Patch>("Postfix")));
            harmonyBase.Patch(typeof(PrefsManager).GetConstructor(new Type[0]), postfix: GetHarmonyMethod(GetMethod<PrefsManager_Ctor>("Postfix")));
            harmonyBase.Patch(GetMethod<PrefsManager>("EnsureValid"), prefix: GetHarmonyMethod(GetMethod<PrefsManager_EnsureValid>("Prefix")));
            harmonyBase.Patch(GetMethod<Grenade>("Explode"), prefix: new HarmonyMethod(GetMethod<GrenadeExplosionOverride>("Prefix")), postfix: new HarmonyMethod(GetMethod<GrenadeExplosionOverride>("Postfix")));
            LoadPrefabs();
            ConfigManager.Initialize();

            SceneManager.activeSceneChanged += OnSceneChange;
        }
    }

    public static class Tools
    {
        private static Transform _target;
        private static Transform target { get
            {
                if(_target == null)
                    _target = MonoSingleton<PlayerTracker>.Instance.GetTarget();
                return _target;
            }
        }

        public static Vector3 PredictPlayerPosition(float speedMod, Collider enemyCol = null)
        {
            Vector3 projectedPlayerPos;

            if (MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude == 0f)
            {
                return target.position;
            }
            RaycastHit raycastHit;
            if (enemyCol != null && Physics.Raycast(target.position, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity(), out raycastHit, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude * 0.35f / speedMod, 4096, QueryTriggerInteraction.Collide) && raycastHit.collider == enemyCol)
            {
                projectedPlayerPos = target.position;
            }
            else if (Physics.Raycast(target.position, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity(), out raycastHit, MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity().magnitude * 0.35f / speedMod, LayerMaskDefaults.Get(LMD.EnvironmentAndBigEnemies), QueryTriggerInteraction.Collide))
            {
                projectedPlayerPos = raycastHit.point;
            }
            else
            {
                projectedPlayerPos = target.position + MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity() * 0.35f / speedMod;
                projectedPlayerPos = new Vector3(projectedPlayerPos.x, target.transform.position.y + (target.transform.position.y - projectedPlayerPos.y) * 0.5f, projectedPlayerPos.z);
            }

            return projectedPlayerPos;
        }
    }

    // Asset destroyer tracker
    /*[HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(UnityEngine.Object) })]
    public class TempClass1
    {
        static void Postfix(UnityEngine.Object __0)
        {
            if (__0 != null && __0 == Plugin.homingProjectile)
            {
                System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                Debug.LogError("Projectile destroyed");
                Debug.LogError(t.ToString());
                throw new Exception("Attempted to destroy proj");
            }
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(UnityEngine.Object), typeof(float) })]
    public class TempClass2
    {
        static void Postfix(UnityEngine.Object __0)
        {
            if (__0 != null && __0 == Plugin.homingProjectile)
            {
                System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                Debug.LogError("Projectile destroyed");
                Debug.LogError(t.ToString());
                throw new Exception("Attempted to destroy proj");
            }
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.DestroyImmediate), new Type[] { typeof(UnityEngine.Object) })]
    public class TempClass3
    {
        static void Postfix(UnityEngine.Object __0)
        {
            if (__0 != null && __0 == Plugin.homingProjectile)
            {
                System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                Debug.LogError("Projectile destroyed");
                Debug.LogError(t.ToString());
                throw new Exception("Attempted to destroy proj");
            }
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.DestroyImmediate), new Type[] { typeof(UnityEngine.Object), typeof(bool) })]
    public class TempClass4
    {
        static void Postfix(UnityEngine.Object __0)
        {
            if (__0 != null && __0 == Plugin.homingProjectile)
            {
                System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                Debug.LogError("Projectile destroyed");
                Debug.LogError(t.ToString());
                throw new Exception("Attempted to destroy proj");
            }
        }
    }*/
}
