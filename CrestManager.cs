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


        public static DevilCrest devilCrest;
        public static HeroController.ConfigGroup devilConfigGroup;

        public static GameObject CloneSlashObject(GameObject slashToClone, GameObject slashRoot)
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

        public static GameObject CloneDownspikeObject(GameObject slashToClone, GameObject slashRoot)
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

        public static void AddDevilRoot()
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            devilroot = new GameObject();
            devilroot.name = "Devil";
            devilroot.transform.parent = attacks.transform;
            devilroot.transform.localPosition = new Vector3();

        }


        public static void AddCrest()
        {
            AddDevilRoot();


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
                keysheet = "VMCSE",
                namekey = "DEVILCRESTNAME",
                desckey = "DEVILCRESTDESC"
            };

            RegisterCrest(devilCrest);

            devilCrest.SetupWeapon();
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
            devilConfigGroup = crest.getConfigGroup();
            asList.Add(devilConfigGroup);
            HeroController.instance.configs = asList.ToArray(); 


            //Adding the ToolCrest to the ToolCrestList in ToolItemManager
            ToolCrestList currentList = ToolItemManager.Instance.crestList;
            currentList.Add(crestData);
            ToolItemManager.Instance.crestList = currentList;

            //Finishing touches
            crest.toolCrest = crestData;
        }
    }
}
