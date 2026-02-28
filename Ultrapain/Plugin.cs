using BepInEx;
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
        public const string PLUGIN_VERSION = "1.1.3";

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
                currentDifficultyInfoText.ForceMeshUpdate();
                currentDifficultyInfoText.Rebuild(CanvasUpdate.PreRender);
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
        private static MethodInfo DoGetMethod<T>(string name)
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

            //New Condition-At-Runtime enemy patches
            harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<LegacyChanges_AddBrutalStacking>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<LegacyChanges_RemoveViolenceFeatures>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("GetSpeed"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<ZombieProjectile_GetSpeed_Patch>("Prefix")));


            if (ConfigManager.friendlyFireDamageOverrideToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Explosion_Collide_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Explosion_Collide_FF>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<PhysicalShockwave>("CheckCollision"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<PhysicalShockwave_CheckCollision_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<PhysicalShockwave_CheckCollision_FF>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<VirtueInsignia>("OnTriggerEnter"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<VirtueInsignia_OnTriggerEnter_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<VirtueInsignia_OnTriggerEnter_FF>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwingCheck2>("CheckCollision"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_CheckCollision_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_CheckCollision_FF>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Projectile>("Collided"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Projectile_Collided_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Projectile_Collided_FF>("Postfix")));

                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_DeliverDamage_FF>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Flammable>("Burn"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Flammable_Burn_FF>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<FireZone>("OnTriggerStay"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<StreetCleaner_Fire_FF>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StreetCleaner_Fire_FF>("Postfix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("UpdateModifiers"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_UpdateModifiers>("Postfix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<StatueBoss>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StatueBoss_Start_Patch>("Postfix")));
            if (ConfigManager.cerberusDashToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<StatueBoss>("Tackle"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StatueBoss_Tackle_Patch>("Postfix")));
            if (ConfigManager.cerberusParryable.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<StatueBoss>("StopTracking"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StatueBoss_StopTracking_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<StatueBoss>("Stomp"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StatueBoss_Stomp_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<StatueBoss>("StopDash"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<StatueBoss_StopDash_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Statue>("GetHurt"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Statue_GetHurt_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Shoot"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Shoot_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("PlaySound"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_PlaySound_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Update"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Update>("Postfix")));
            if(ConfigManager.droneHomeToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Death"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Death_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("GetHurt"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_GetHurt_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<Ferryman>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<FerrymanStart>("Postfix")));
            if(ConfigManager.ferrymanComboToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Ferryman>("StopMoving"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<FerrymanStopMoving>("Postfix")));

            if(ConfigManager.filthExplodeToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwingCheck2>("CheckCollision"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_CheckCollision_Patch2>("Prefix")));

            if(ConfigManager.fleshPrisonSpinAttackToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("HomingProjectileAttack"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<FleshPrisonShoot>("Postfix")));

            if (ConfigManager.hideousMassInsigniaToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Projectile>("Explode"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Projectile_Explode_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Mass>("ShootExplosive"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<HideousMassHoming>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<HideousMassHoming>("Prefix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<SpiderBody>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MaliciousFace_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<SpiderBody>("ChargeBeam"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MaliciousFace_ChargeBeam>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<SpiderBody>("BeamChargeEnd"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MaliciousFace_BeamChargeEnd>("Prefix")));
            if (ConfigManager.maliciousFaceHomingProjectileToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<SpiderBody>("ShootProj"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MaliciousFace_ShootProj_Patch>("Postfix")));
            }
            if (ConfigManager.maliciousFaceRadianceOnEnrage.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<SpiderBody>("Enrage"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MaliciousFace_Enrage_Patch>("Postfix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<Mindflayer>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Mindflayer_Start_Patch>("Postfix")));
            if (ConfigManager.mindflayerShootTweakToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Mindflayer>("ShootProjectiles"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Mindflayer_ShootProjectiles_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_DeliverDamage_MF>("Prefix")));
            }
            if (ConfigManager.mindflayerTeleportComboToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwingCheck2>("CheckCollision"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_CheckCollision_Patch>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_CheckCollision_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Mindflayer>("MeleeTeleport"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Mindflayer_MeleeTeleport_Patch>("Prefix")));
                //harmonyTweaks.Patch(Plugin.DoGetMethod<SwingCheck2>("DamageStop"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwingCheck2_DamageStop_Patch>("Postfix")));
            }

            if (ConfigManager.minosPrimeRandomTeleportToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("ProjectileCharge"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrimeCharge>("Postfix")));
            if (ConfigManager.minosPrimeTeleportTrail.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Teleport"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrimeCharge>("TeleportPostfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_Start>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Dropkick"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_Dropkick>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Combo"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_Combo>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("StopAction"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_StopAction>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Ascend"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_Ascend>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("Death"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_Death>("Prefix")));
            if (ConfigManager.minosPrimeCrushAttackToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("RiderKick"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_RiderKick>("Prefix")));
            if (ConfigManager.minosPrimeComboExplosiveEndToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<MinosPrime>("ProjectileCharge"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<MinosPrime_ProjectileCharge>("Prefix")));

            if (ConfigManager.schismSpreadAttackToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("ShootProjectile"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<ZombieProjectile_ShootProjectile_Patch>("Postfix")));

            if (ConfigManager.soliderShootTweakToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Solider_Start_Patch>("Postfix")));
            }
            if(ConfigManager.soliderCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("SpawnProjectile"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Solider_SpawnProjectile_Patch>("Postfix")));
            if (ConfigManager.soliderShootGrenadeToggle.value || ConfigManager.soliderShootTweakToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("ThrowProjectile"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Solider_ThrowProjectile_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Grenade>("Explode"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Explode_Patch>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Explode_Patch>("Prefix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<Stalker>("SandExplode"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Stalker_SandExplode_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<SandificationZone>("Enter"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SandificationZone_Enter_Patch>("Postfix")));

            if (ConfigManager.strayCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("SpawnProjectile"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Swing>("Postfix")));
            if (ConfigManager.strayShootToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<ZombieProjectile_Start_Patch1>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("ThrowProjectile"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<ZombieProjectile_ThrowProjectile_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("SwingEnd"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwingEnd>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<ZombieProjectiles>("DamageEnd"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DamageEnd>("Prefix")));
            }

            if(ConfigManager.streetCleanerCoinsIgnoreWeakPointToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Streetcleaner>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<StreetCleaner_Start_Patch>("Postfix")));
            if(ConfigManager.streetCleanerPredictiveDodgeToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<BulletCheck>("OnTriggerEnter"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<BulletCheck_OnTriggerEnter_Patch>("Postfix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<SwordsMachine>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_Start>("Postfix")));
            if (ConfigManager.swordsMachineNoLightKnockbackToggle.value || ConfigManager.swordsMachineSecondPhaseMode.value != ConfigManager.SwordsMachineSecondPhase.None)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwordsMachine>("Knockdown"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_Knockdown_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwordsMachine>("Down"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_Down_Patch>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_Down_Patch>("Prefix")));
                //harmonyTweaks.Patch(Plugin.DoGetMethod<SwordsMachine>("SetSpeed"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_SetSpeed_Patch>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<SwordsMachine>("EndFirstPhase"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_EndFirstPhase_Patch>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SwordsMachine_EndFirstPhase_Patch>("Prefix")));
            }
            if (ConfigManager.swordsMachineExplosiveSwordToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<ThrownSword>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<ThrownSword_Start_Patch>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<ThrownSword>("OnTriggerEnter"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<ThrownSword_OnTriggerEnter_Patch>("Postfix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<Turret>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<TurretStart>("Postfix")));
            if(ConfigManager.turretBurstFireToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Turret>("Shoot"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<TurretShoot>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Turret>("StartAiming"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<TurretAim>("Postfix")));
            }

            harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2CommonExplosion>("Postfix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2FirstStart>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2FirstUpdate>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("ShootWeapon"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2FirstShootWeapon>("Prefix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondStart>("Postfix")));
            //if(ConfigManager.v2SecondStartEnraged.value)
            //    harmonyTweaks.Patch(Plugin.DoGetMethod<BossHealthBar>("OnEnable"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondEnrage>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondUpdate>("Prefix")));
            //harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("AltShootWeapon"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2AltShootWeapon>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("SwitchWeapon"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondSwitchWeapon>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("ShootWeapon"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondShootWeapon>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondShootWeapon>("Postfix")));
            if(ConfigManager.v2SecondFastCoinToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<V2>("ThrowCoins"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2SecondFastCoin>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Cannonball>("OnTriggerEnter"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2RocketLauncher>("CannonBallTriggerPrefix")));
            
            if (ConfigManager.v2FirstSharpshooterToggle.value || ConfigManager.v2SecondSharpshooterToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyRevolver>("PrepareAltFire"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2CommonRevolverPrepareAltFire>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Projectile>("Collided"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2CommonRevolverBullet>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyRevolver>("AltFire"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<V2CommonRevolverAltShoot>("Prefix")));
            }
            
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Virtue_Start_Patch>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("SpawnDroneInsignia"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Virtue_SpawnInsignia_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Death"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Virtue_Death_Patch>("Prefix")));
           

            if (ConfigManager.sisyInstJumpShockwave.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Sisyphus>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SisyphusInstructionist_Start>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Sisyphus>("Update"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SisyphusInstructionist_Update>("Postfix")));
            }
            
            if(ConfigManager.sisyInstBoulderShockwave.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Sisyphus>("SetupExplosion"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SisyphusInstructionist_SetupExplosion>("Postfix")));
            if(ConfigManager.sisyInstStrongerExplosion.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Sisyphus>("StompExplosion"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SisyphusInstructionist_StompExplosion>("Prefix")));
            
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanTail>("Awake"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<LeviathanTail_Start>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanTail>("BigSplash"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<LeviathanTail_BigSplash>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanTail>("SwingEnd"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<LeviathanTail_SwingEnd>("Prefix")));
            
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanHead>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Leviathan_Start>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanHead>("ProjectileBurst"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Leviathan_ProjectileBurst>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanHead>("ProjectileBurstStart"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Leviathan_ProjectileBurstStart>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<LeviathanHead>("FixedUpdate"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Leviathan_FixedUpdate>("Prefix")));
            /*
            if (ConfigManager.somethingWickedSpear.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Wicked>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SomethingWicked_Start>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Wicked>("GetHit"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<SomethingWicked_GetHit>("Postfix")));
            }
            if(ConfigManager.somethingWickedSpawnOn43.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<ObjectActivator>("Activate"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<ObjectActivator_Activate>("Prefix")));
            }*/


            if (ConfigManager.panopticonFullPhase.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_Start>("Postfix")));
            if (ConfigManager.panopticonAxisBeam.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("SpawnInsignia"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_SpawnInsignia>("Prefix")));
            if (ConfigManager.panopticonSpinAttackToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("HomingProjectileAttack"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_HomingProjectileAttack>("Postfix")));
            if (ConfigManager.panopticonBlackholeProj.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("SpawnBlackHole"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_SpawnBlackHole>("Postfix")));
            if (ConfigManager.panopticonBalanceEyes.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("SpawnFleshDrones"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_SpawnFleshDrones>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_SpawnFleshDrones>("Postfix")));
            if (ConfigManager.panopticonBlueProjToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("Update"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Panopticon_BlueProjectile>("Transpiler")));


            if (ConfigManager.idolExplosionToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Idol>("Death"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Idol_Death_Patch>("Postfix")));
            if (ConfigManager.providenceRandomPattern.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Shoot"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Providence_CrossPatch>("Prefix")));

            // ADDME

            //harmonyTweaks.Patch(Plugin.DoGetMethod<GabrielSecond>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<GabrielSecond_Start>("Postfix")));
            //harmonyTweaks.Patch(Plugin.DoGetMethod<GabrielSecond>("BasicCombo"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<GabrielSecond_BasicCombo>("Postfix")));
            //harmonyTweaks.Patch(Plugin.DoGetMethod<GabrielSecond>("FastCombo"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<GabrielSecond_FastCombo>("Postfix")));
            //harmonyTweaks.Patch(Plugin.DoGetMethod<GabrielSecond>("CombineSwords"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<GabrielSecond_CombineSwords>("Postfix")));
            //harmonyTweaks.Patch(Plugin.DoGetMethod<GabrielSecond>("ThrowCombo"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<GabrielSecond_ThrowCombo>("Postfix")));
            
        }

        private static void PatchAllPlayers()
        {
            /*if (ConfigManager.crossmodSupport_MD.value == true)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Start"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<CrossmodSupport_MD_SplendorConductor>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_DeliverDamage_Crossmod>("Prefix")));
            }*/
            if (!ConfigManager.playerTweakToggle.value)
                return;
            harmonyTweaks.Patch(Plugin.DoGetMethod<Punch>("TryParryProjectile"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Punch_TryParryProjectile_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Grenade>("Explode"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Explode_Patch1>("Prefix")));
            Type[] types = [typeof(Collider)];
            harmonyTweaks.Patch(typeof(Grenade).GetMethod("Collision", types), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Collision_Patch>("Prefix")));
            if (ConfigManager.rocketBoostToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Explosion_Collide_Patch>("Prefix")));

            if (ConfigManager.rocketGrabbingToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<HookArm>("FixedUpdate"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<HookArm_FixedUpdate_Patch>("Prefix")));
            
            if (ConfigManager.orbStrikeToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Coin>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Coin_Start>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Punch>("BlastCheck"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Punch_BlastCheck>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Punch_BlastCheck>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Explosion_Collide>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Coin>("DelayedReflectRevolver"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Coin_DelayedReflectRevolver>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Coin>("ReflectRevolver"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Coin_ReflectRevolver>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Coin_ReflectRevolver>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Grenade>("Explode"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Explode>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Grenade_Explode>("Postfix")));
                
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnemyIdentifier>("DeliverDamage"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_DeliverDamage>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<EnemyIdentifier_DeliverDamage>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<RevolverBeam>("ExecuteHits"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<RevolverBeam_ExecuteHits>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<RevolverBeam_ExecuteHits>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<RevolverBeam>("HitSomething"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<RevolverBeam_HitSomething>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<RevolverBeam_HitSomething>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<RevolverBeam>("Start"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<RevolverBeam_Start>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Cannonball>("Explode"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Cannonball_Explode>("Prefix")));

                harmonyTweaks.Patch(Plugin.DoGetMethod<Explosion>("Collide"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Explosion_CollideOrbital>("Prefix")));
            }
            
            if(ConfigManager.chargedRevRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Revolver>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Revolver_Update>("Prefix")));
            if(ConfigManager.coinRegSpeedMulti.value != 1 || ConfigManager.sharpshooterRegSpeedMulti.value != 1
                || ConfigManager.railcannonRegSpeedMulti.value != 1 || ConfigManager.rocketFreezeRegSpeedMulti.value != 1
                || ConfigManager.rocketCannonballRegSpeedMulti.value != 1 || ConfigManager.nailgunAmmoRegSpeedMulti.value != 1
                || ConfigManager.sawAmmoRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(Plugin.DoGetMethod<WeaponCharges>("Charge"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<WeaponCharges_Charge>("Prefix")));
            if(ConfigManager.nailgunHeatsinkRegSpeedMulti.value != 1 || ConfigManager.sawHeatsinkRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Nailgun>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<NailGun_Update>("Prefix")));
            if(ConfigManager.staminaRegSpeedMulti.value != 1)
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("Update"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_Update>("Prefix")));
            
            if(ConfigManager.playerHpDeltaToggle.value || ConfigManager.maxPlayerHp.value != 100 || ConfigManager.playerHpSupercharge.value != 200 || ConfigManager.whiplashHardDamageCap.value != 50 || ConfigManager.whiplashHardDamageSpeed.value != 1)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("GetHealth"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_GetHealth>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("SuperCharge"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_SuperCharge>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("Respawn"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_Respawn>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_Start>("Postfix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("GetHurt"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_GetHurt>("Transpiler")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<HookArm>("FixedUpdate"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<HookArm_FixedUpdate>("Transpiler")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("ForceAntiHP"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_ForceAntiHP>("Transpiler")));
            }

            // ADDME
            harmonyTweaks.Patch(Plugin.DoGetMethod<Revolver>("Shoot"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Revolver_Shoot>("Transpiler")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Shotgun>("Shoot"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Shotgun_Shoot>("Transpiler")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Shotgun_Shoot>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Shotgun_Shoot>("Postfix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Shotgun>("ShootSinks"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Shotgun_ShootSinks>("Transpiler")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Nailgun>("Shoot"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Nailgun_Shoot>("Transpiler")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<Nailgun>("SuperSaw"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<Nailgun_SuperSaw>("Transpiler")));
            
            if (ConfigManager.hardDamagePercent.normalizedValue != 1)
                harmonyTweaks.Patch(Plugin.DoGetMethod<NewMovement>("GetHurt"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_GetHurt>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<NewMovement_GetHurt>("Postfix")));

            harmonyTweaks.Patch(Plugin.DoGetMethod<HealthBar>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<HealthBar_Start>("Postfix")));
			harmonyTweaks.Patch(Plugin.DoGetMethod<HealthBar>("Update"), transpiler: GetHarmonyMethod(Plugin.DoGetMethod<HealthBar_Update>("Transpiler")));
			foreach (HealthBarTracker hb in HealthBarTracker.instances)
            {
                if (hb != null)
                    hb.SetSliderRange();
            }
            
            harmonyTweaks.Patch(Plugin.DoGetMethod<Harpoon>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Harpoon_Start>("Postfix")));
            if(ConfigManager.screwDriverHomeToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Harpoon>("Punched"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Harpoon_Punched>("Postfix")));
            if(ConfigManager.screwDriverSplitToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<Harpoon>("OnTriggerEnter"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Harpoon_OnTriggerEnter_Patch>("Prefix")));
        }

        private static void PatchAllMemes()
        {
            if (ConfigManager.enrageSfxToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<EnrageEffect>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<EnrageEffect_Start>("Postfix")));
            
            if(ConfigManager.funnyDruidKnightSFXToggle.value)
            {
                harmonyTweaks.Patch(Plugin.DoGetMethod<Mandalore>("FullBurst"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<DruidKnight_FullBurst>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DruidKnight_FullBurst>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Mandalore>("FullerBurst"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DruidKnight_FullerBurst>("Prefix")));
                harmonyTweaks.Patch(Plugin.DoGetMethod<Drone>("Explode"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Explode>("Prefix")), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Drone_Explode>("Postfix")));
            }

            if (ConfigManager.fleshObamiumToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<FleshObamium_Start>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<FleshObamium_Start>("Prefix")));
            if (ConfigManager.obamapticonToggle.value)
                harmonyTweaks.Patch(Plugin.DoGetMethod<FleshPrison>("Start"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<Obamapticon_Start>("Postfix")), prefix: GetHarmonyMethod(Plugin.DoGetMethod<Obamapticon_Start>("Prefix")));
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

            harmonyTweaks.Patch(Plugin.DoGetMethod<DiscordController>("SendActivity"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DiscordController_SendActivity_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<DiscordController>("FetchSceneActivity"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DiscordController_FetchSceneActivity_Patch>("Prefix")));
            harmonyTweaks.Patch(Plugin.DoGetMethod<SteamController>("FetchSceneActivity"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<SteamController_FetchSceneActivity_Patch>("Prefix")));

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
            Logger.LogInfo("ULTRAPAIN: REFUELED is very early in proper development! Beware of bugs!");

            harmonyTweaks = new Harmony(PLUGIN_GUID + "_tweaks");
            harmonyBase = new Harmony(PLUGIN_GUID + "_base");

            harmonyBase.Patch(typeof(GameProgressSaver).GetMethod("GetProgress"), prefix: GetHarmonyMethod(typeof(GameProgressSaver_GetProgress_Fix).GetMethod("Prefix")));
            harmonyBase.Patch(typeof(GameProgressSaver).GetMethod("GetEncoreProgress"), prefix: GetHarmonyMethod(typeof(GameProgressSaver_GetEncoreProgress_Fix).GetMethod("Prefix")));
            harmonyBase.Patch(typeof(GameProgressSaver).GetMethod("GetPrime"), prefix: GetHarmonyMethod(typeof(GameProgressSaver_GetPrime_Fix).GetMethod("Prefix")));

            harmonyBase.Patch(Plugin.DoGetMethod<DifficultySelectButton>("SetDifficulty"), postfix: GetHarmonyMethod(Plugin.DoGetMethod<DifficultySelectPatch>("Postfix")));
            harmonyBase.Patch(Plugin.DoGetMethod<DifficultyTitle>("Check"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<DifficultyTitle_Check_Patch>("Prefix")));
            harmonyBase.Patch(Plugin.DoGetMethod<LevelSelectPanel>("CheckScore"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<LevelSelectPanel_Fix>("Prefix")));
            harmonyBase.Patch(typeof(PrefsManager).GetConstructor(new Type[0]), postfix: GetHarmonyMethod(Plugin.DoGetMethod<PrefsManager_Ctor>("Postfix")));
            harmonyBase.Patch(Plugin.DoGetMethod<PrefsManager>("EnsureValid"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<PrefsManager_EnsureValid>("Prefix")));
            harmonyBase.Patch(Plugin.DoGetMethod<EndlessHighScore>("OnEnable"), prefix: GetHarmonyMethod(Plugin.DoGetMethod<EndlessHighScore_Fix>("Prefix")));
            harmonyBase.Patch(Plugin.DoGetMethod<Grenade>("Explode"), prefix: new HarmonyMethod(Plugin.DoGetMethod<GrenadeExplosionOverride>("Prefix")), postfix: new HarmonyMethod(Plugin.DoGetMethod<GrenadeExplosionOverride>("Postfix")));
            harmonyBase.Patch(typeof(RankData).GetConstructor([typeof(StatsManager)]), prefix: GetHarmonyMethod(Plugin.DoGetMethod<RankData_Fix>("Prefix")));

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
