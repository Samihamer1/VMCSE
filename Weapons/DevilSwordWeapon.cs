using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.DevilSword;
using VMCSE.Attacks.Slashes;

namespace VMCSE.Weapons
{
    internal class DevilSwordWeapon : WeaponBase
    {
        public DevilSwordWeapon() {
            CreateWeaponObjects();

            weaponEvent = "DEVILSWORD";
            dashSlash = new MillionStab();
            cycloneSlash = new HighTime();
            specialDownSlash = new DevilSwordDownslash(objectInfo.slashDownObject);
            specialDashStab = new DevilSwordDashstab(objectInfo.slashdashstabObject);

            downSpell = new Reactor(handler);

            Initialise();

            configGroup = objectInfo.getConfigGroup();
            
            
        }

        private void CreateWeaponObjects()
        {
            objectInfo = new CrestManagement.CrestObjectInfo();
            GameObject devilroot = CrestManager.devilroot;

            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");
            GameObject hunter = attacks.Child("Default");
            GameObject reaper = attacks.Child("Scythe");
            GameObject witch = attacks.Child("Witch");
            GameObject beast = attacks.Child("Warrior");
            GameObject wanderer = attacks.Child("Wanderer");

            objectInfo.slashWallObject = CrestManager.CloneSlashObject(reaper.Child("WallSlash"), devilroot);

            //special case for our dash stab
            GameObject devildashstab = UnityEngine.Object.Instantiate(hunter.Child("Dash Stab"));
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

            objectInfo.slashdashstabObject = devildashstab;


            //repackage sprites to fix artefact
            //Regular slash
            GameObject devilslash = CrestManager.CloneSlashObject(hunter.Child("Slash"), devilroot);
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
            objectInfo.slashObject = devilslash;

            //Altslash
            GameObject devilslashalt = CrestManager.CloneSlashObject(hunter.Child("Slash"), devilroot);
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
            objectInfo.slashAltObject = devilslashalt;

            //Slashup
            GameObject devilupslash = CrestManager.CloneSlashObject(hunter.Child("UpSlash"), devilroot);
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
            objectInfo.slashUpObject = devilupslash;

            //Downspike
            GameObject devildownslash = CrestManager.CloneSlashObject(wanderer.Child("DownSlash"), devilroot);
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
            objectInfo.slashDownObject = devildownslash;
        }
    }
}
