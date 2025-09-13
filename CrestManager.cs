using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private static GameObject CloneSlashObject(GameObject slashToClone, GameObject slashRoot)
        {
            GameObject clonedSlash = UnityEngine.Object.Instantiate(slashToClone);
            clonedSlash.transform.parent = slashRoot.transform;
            clonedSlash.transform.localPosition = new Vector3(0, 0, -0.001f);
            NailSlash slash = clonedSlash.GetComponent<NailSlash>();
            Helper.SetPrivateField(slash, nameof(slash.hc), HeroController.instance);
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

            devilslash = CloneSlashObject(witch.Child("Slash"), devilroot);
            devilslashalt = CloneSlashObject(beast.Child("AltSlash"), devilroot);
            devilupslash = CloneSlashObject(hunter.Child("UpSlash"), devilroot);

            devildownslash = CloneSlashObject(wanderer.Child("DownSlash"), devilroot);
            HeroDownAttack downattack = devildownslash.GetComponent<HeroDownAttack>();
            DamageEnemies damageenemies = devildownslash.GetComponent<DamageEnemies>();
            NailSlash downslash = devildownslash.GetComponent<NailSlash>();
            Helper.SetPrivateField(downslash, nameof(downslash.enemyDamager), damageenemies);
            Helper.SetPrivateField(downattack, nameof(downattack.attack), downslash);

            devilwallslash = CloneSlashObject(reaper.Child("WallSlash"), devilroot);

            //special case for our dash stab
            devildashstab = UnityEngine.Object.Instantiate(hunter.Child("Dash Stab"));
            devildashstab.transform.parent = devilroot.transform;
            devildashstab.transform.localPosition = new Vector3(0, 0, -0.001f);
            DashStabNailAttack dashstab = devildashstab.GetComponent<DashStabNailAttack>();
            Helper.SetPrivateField(dashstab, nameof(dashstab.hc), HeroController.instance);
            dashstab.enabled = true;
        }


        public static void AddCrest()
        {
            AddAttacks();


            ToolCrestList currentList = Helper.GetPrivateField<ToolCrestList>(ToolItemManager.Instance, nameof(ToolItemManager.crestList));
            ToolCrest reaper = currentList.GetByName("Reaper");
            Sprite crestGlow = reaper.CrestGlow;
            Sprite crestSilhouette = reaper.CrestSilhouette;
            Sprite crestSprite = reaper.CrestSprite;
            HeroControllerConfig reaperConfig = reaper.HeroConfig;

            HeroControllerConfig testConfig = UnityEngine.GameObject.Instantiate(reaperConfig);
            testConfig.name = "Test";
            Helper.SetPrivateField(testConfig, nameof(testConfig.downSlashType), HeroControllerConfig.DownSlashTypes.Slash);


            //Creating the crest, mostly stealing from reaper for proof of concept.
            CustomTestCrest TestCrest = new CustomTestCrest()
            {
                name = "Test",
                crestGlowSprite = crestGlow,
                crestSilhouetteSprite = crestSilhouette,
                crestSprite = crestSprite,
                heroControllerConfig = testConfig,
                isUnlocked = true,
                activeRootObject = devilroot,
                slashDashObject = devildashstab,
                slashDownObject = devildownslash,
                slashObject = devilslash,
                slashUpObject = devilupslash,
                slashWallObject = devilwallslash,
                slashAltObject = devilslashalt
            };

            RegisterCrest(TestCrest);

            //double test

            HeroControllerConfig devilConfig = UnityEngine.GameObject.Instantiate(reaperConfig);
            devilConfig.name = "Devil";
            Helper.SetPrivateField(devilConfig, nameof(devilConfig.downSlashType), HeroControllerConfig.DownSlashTypes.Slash);

            DevilCrest devilCrest = new DevilCrest()
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
                slashObject = devilslashalt,
                slashUpObject = devilupslash,
                slashWallObject = devilwallslash,
                slashAltObject = devilslash
            };

            RegisterCrest(devilCrest);
        }

        public static void RegisterCrest(CustomCrest crest)
        {
            ToolCrest crestData = new ToolCrest();
            crestData.name = crest.name;
            Helper.SetPrivateField(crestData, nameof(ToolCrest.crestSprite), crest.crestSprite);
            Helper.SetPrivateField(crestData, nameof(ToolCrest.crestGlow), crest.crestGlowSprite);
            Helper.SetPrivateField(crestData, nameof(ToolCrest.crestSilhouette), crest.crestSilhouetteSprite);
            Helper.SetPrivateField(crestData, nameof(ToolCrest.heroConfig), crest.heroControllerConfig);

            //For now, i'm ignoring slots.
            Helper.SetPrivateField(crestData, nameof(ToolCrest.slots), new SlotInfo[0]);

            crestData.SaveData = new ToolCrestsData.Data
            {
                IsUnlocked = crest.isUnlocked,
                Slots = new List<ToolCrestsData.SlotData>(),
                DisplayNewIndicator = true
            };


            //Add config group to the herocontrollers list
            HeroController.ConfigGroup[] configgroups = Helper.GetPrivateField<HeroController.ConfigGroup[]>(HeroController.instance, nameof(HeroController.instance.configs));

            crest.SetupConfigGroup();

            List<HeroController.ConfigGroup> asList = configgroups.ToList();
            asList.Add(crest.getConfigGroup());
            Helper.SetPrivateField(HeroController.instance, nameof(HeroController.instance.configs), asList.ToArray());


            //Adding the ToolCrest to the ToolCrestList in ToolItemManager
            ToolCrestList currentList = Helper.GetPrivateField<ToolCrestList>(ToolItemManager.Instance, nameof(ToolItemManager.crestList));
            currentList.Add(crestData);
            Helper.SetPrivateField(ToolItemManager.Instance, nameof(ToolItemManager.crestList), currentList);

            //Finishing touches
            crest.toolCrest = crestData;
            crest.SetupDashFSM();

        }
    }
}
