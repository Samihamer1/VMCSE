using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.Localization;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.Slashes;
using VMCSE.CrestManagement;
using static ToolCrest;

namespace VMCSE
{
    public static class CrestManager
    {
        public static GameObject devilroot;
        public static GameObject devilslash;
        public static GameObject devilslashalt;
        public static GameObject devilupslash;
        public static GameObject devildownslash;
        public static GameObject devildashstab;
        public static GameObject devilwallslash;
        public static GameObject devilswordMillionStinger;
        public static GameObject devilswordHighTime;

        public static DevilCrest devilCrest;

        private static GameObject CloneSlashObject(GameObject slashToClone, GameObject slashRoot)
        {
            GameObject clonedSlash = UnityEngine.Object.Instantiate(slashToClone);
            clonedSlash.transform.parent = slashRoot.transform;
            clonedSlash.transform.localPosition = new Vector3(0, 0, -0.001f);
            NailSlash slash = clonedSlash.GetComponent<NailSlash>();
            NailSlashRecoil nailSlashRecoil = clonedSlash.GetComponent<NailSlashRecoil>();
            if (nailSlashRecoil != null)
            {
                nailSlashRecoil.heroCtrl = HeroController.instance;
            }

            slash.hc = HeroController.instance;
            slash.enabled = true;
            return clonedSlash;
        }

        private static GameObject CloneDownspikeObject(GameObject slashToClone, GameObject slashRoot)
        {
            GameObject clonedSlash = UnityEngine.Object.Instantiate(slashToClone);
            clonedSlash.transform.parent = slashRoot.transform;
            clonedSlash.transform.localPosition = new Vector3(0, 0, -0.001f);
            Downspike slash = clonedSlash.GetComponent<Downspike>();
            slash.heroCtrl = HeroController.instance;
            //Helper.SetPrivateField(slash, nameof(slash.hc), HeroController.instance);
            slash.enabled = true;
            return clonedSlash;
        }

        public static void AddAttacks()
        {
            //Just cloning and a variety of slashes for testing.

            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");
            GameObject hunter = attacks.Child("Default");
            GameObject reaper = attacks.Child("Scythe");
            GameObject witch = attacks.Child("Witch");
            GameObject beast = attacks.Child("Warrior");
            GameObject wanderer = attacks.Child("Wanderer");

            devilroot = new GameObject();
            devilroot.name = "Devil";
            devilroot.transform.parent = attacks.transform;
            devilroot.transform.localPosition = new Vector3();


            devilwallslash = CloneSlashObject(reaper.Child("WallSlash"), devilroot);

            //special case for our dash stab
            devildashstab = UnityEngine.Object.Instantiate(hunter.Child("Dash Stab"));
            devildashstab.transform.parent = devilroot.transform;
            devildashstab.transform.localPosition = new Vector3(0, 0, -0.001f);
            DashStabNailAttack dashstab = devildashstab.GetComponent<DashStabNailAttack>();
            dashstab.scale.x = -1.4f;
            dashstab.heroController = HeroController.instance;
            dashstab.hc = HeroController.instance;
            dashstab.enabled = true;
            DamageEnemies damager = devildashstab.GetComponent<DamageEnemies>();
            damager.magnitudeMult = 2.25f;
            damager.nailDamageMultiplier = 1.5f;
            damager.doesNotTink = true;


            //repackage sprites to fix artefact
            //Regular slash
            devilslash = CloneSlashObject(hunter.Child("Slash"), devilroot);
            StandardSlash devilSlash1 = new StandardSlash
            {
                SlashObject = devilslash,
                SlashAnimatorObject = AnimationManager.DevilSwordAnimator,
                SlashPrefabName = "DevilSwordSlash",
                AnimationName = "SlashEffect",
                LocalPosition = new Vector3(-1.1f, 0.4f, -0.001f),
                LocalScale = new Vector3(1.1f, 1f, 1)
            };
            devilSlash1.SetupSlash();

            //Altslash
            devilslashalt = CloneSlashObject(hunter.Child("Slash"), devilroot);
            StandardSlash devilSlash2 = new StandardSlash
            {
                SlashObject = devilslashalt,
                SlashAnimatorObject = AnimationManager.DevilSwordAnimator,
                SlashPrefabName = "DevilSwordSlashAlt",
                AnimationName = "SlashAltEffect",
                LocalPosition = new Vector3(-0.6f, 0.4f, -0.001f),
                LocalScale = new Vector3(1.1f, 0.85f, 1)
            };
            devilSlash2.SetupSlash();

            //Slashup
            devilupslash = CloneSlashObject(hunter.Child("UpSlash"), devilroot);
            StandardSlash devilSlash3 = new StandardSlash
            {
                SlashObject = devilupslash,
                SlashAnimatorObject = AnimationManager.DevilSwordAnimator,
                SlashPrefabName = "DevilSwordSlashUp",
                AnimationName = "SlashUpEffect",
                LocalPosition = new Vector3(-1f, 1.1f, -0.001f),
                LocalScale = new Vector3(1.1f, 1f, 1)
            };
            devilSlash3.SetupSlash();

            //Downspike
            devildownslash = CloneSlashObject(wanderer.Child("DownSlash"), devilroot);
            StandardSlash devildownspike = new StandardSlash
            {
                SlashObject = devildownslash,
                SlashAnimatorObject = AnimationManager.DevilSwordAnimator,
                SlashPrefabName = "DevilSwordDownspike",
                AnimationName = "DownSpikeEffect",
                LocalPosition = new Vector3(),
                LocalScale = new Vector3(1.1f, 1f, 1)
            };
            devildownspike.SetupSlash();

            //Million stinger
            GameObject millionStinger = UnityEngine.Object.Instantiate(attacks.Child("Charge Slash Wanderer"));
            millionStinger.transform.parent = devilroot.transform;
            millionStinger.transform.localScale = new Vector3(1, 0.7f, 1f);
            millionStinger.transform.localPosition = new Vector3(-1.2f, 0.6f, 0);
            millionStinger.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.4f;
            devilswordMillionStinger = millionStinger;

            //High time
            GameObject highTime = UnityEngine.Object.Instantiate(attacks.Child("Charge Slash Wanderer"));
            highTime.transform.parent = devilroot.transform;
            highTime.transform.localScale = new Vector3(-0.9f, 1f, 1f);
            highTime.transform.localPosition = new Vector3(-1f, 0.5f, 0);
            highTime.GetComponent<DamageEnemies>().multiHitter = false;
            //highTime.GetComponent<DamageEnemies>().doesNotTink = true;
            highTime.GetComponent<DamageEnemies>().nailDamageMultiplier = 1.75f;
            highTime.GetComponent<DamageEnemies>().canTriggerBouncePod = true;
            highTime.GetComponent<DamageEnemies>().direction = 90;
            highTime.Child("Charge Slash Hornet Voice").SetActive(false);
            Helper.SetPrivateField<tk2dSpriteAnimationClip>(highTime.GetComponent<tk2dSpriteAnimator>(), "currentClip", AnimationManager.DevilSwordAnimator.GetComponent<tk2dSpriteAnimator>().GetClipByName("HighTimeEffect"));
            highTime.CopyPolygonColliderFromPrefab("DevilSwordHighTime");
            highTime.Child("Lunge Stopper").CopyPolygonColliderFromPrefab("DevilSwordHighTime");
            devilswordHighTime = highTime;

        }


        public static void AddCrest()
        {
            AddAttacks();


            ToolCrestList currentList = ToolItemManager.Instance.crestList;
            ToolCrest reaper = currentList.GetByName("Reaper");
            Sprite crestGlow = reaper.CrestGlow;
            Sprite crestSilhouette = reaper.CrestSilhouette;
            Sprite crestSprite = reaper.CrestSprite;
            HeroControllerConfig reaperConfig = reaper.HeroConfig;


            //double test

            HeroControllerConfig devilConfig = UnityEngine.GameObject.Instantiate(reaperConfig);
            devilConfig.name = "Devil";
            devilConfig.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
            devilConfig.downSlashEvent = "DEVILSWORD DOWNSLASH";
            //Crest Attacks has a state Idle to add to
            //Reference reaper to make it :)
            devilConfig.heroAnimOverrideLib = AnimationManager.DevilSwordAnimator.GetComponent<tk2dSpriteAnimator>().Library;
            

            devilCrest = new DevilCrest()
            {
                name = "Devil",
                crestGlowSprite = crestGlow,
                crestSilhouetteSprite = crestSilhouette,
                crestSprite = crestSprite,
                heroControllerConfig = devilConfig,
                isUnlocked = true,
                activeRootObject = devilroot,
                slashDashObject = devildashstab,
                slashDownObject = devildownslash,
                slashObject = devilslash,
                slashUpObject = devilupslash,
                slashWallObject = devilwallslash,
                slashAltObject = devilslashalt,
                keysheet = "VMCSE",
                namekey = "DEVILCRESTNAME",
                desckey = "DEVILCRESTDESC"
            };

            RegisterCrest(devilCrest);
        }

        public static void RegisterCrest(CustomCrest crest)
        {
            ToolCrest crestData = new ToolCrest();
            crestData.name = crest.name;
            crestData.crestSprite = crest.crestSprite;
            crestData.crestGlow = crest.crestGlowSprite;
            crestData.crestSilhouette = crest.crestSilhouetteSprite;
            crestData.heroConfig = crest.heroControllerConfig;


            LocalisedString namestring = new LocalisedString()
            {
                Key = crest.namekey,
                Sheet = crest.keysheet
            };
            LocalisedString descstring = new LocalisedString()
            {
                Key = crest.desckey,
                Sheet = crest.keysheet
            };
            crestData.displayName = namestring;
            crestData.description = descstring;

            //For now, i'm ignoring slots.
            crestData.slots = new SlotInfo[0];

            crestData.SaveData = new ToolCrestsData.Data
            {
                IsUnlocked = crest.isUnlocked,
                Slots = new List<ToolCrestsData.SlotData>(),
                DisplayNewIndicator = true
            };

            //Add config group to the herocontrollers list
            HeroController.ConfigGroup[] configgroups = HeroController.instance.configs;

            crest.SetupConfigGroup();

            List<HeroController.ConfigGroup> asList = configgroups.ToList();
            asList.Add(crest.getConfigGroup());
            HeroController.instance.configs = asList.ToArray();


            //Adding the ToolCrest to the ToolCrestList in ToolItemManager
            ToolCrestList currentList = ToolItemManager.Instance.crestList;
            currentList.Add(crestData);
            ToolItemManager.Instance.crestList = currentList;

            //Finishing touches
            crest.toolCrest = crestData;
            crest.SetupDashFSM();
            crest.SetupCrestAttackFSM();
            crest.SetupNailArtFSM();
        }
    }
}
