using System;
using System.Collections;
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
        GameObject redslash;
        public DevilSwordWeapon() {
            CreateWeaponObjects();

            weaponEvent = "DEVILSWORD";
            dashSlash = new MillionStab();
            cycloneSlash = new HighTime();
            greatSlash = new SwordFormation();
            specialDownSlash = new DevilSwordDownslash(objectInfo.slashDownObject);
            specialDashStab = new DevilSwordDashstab(objectInfo.slashdashstabObject);

            downSpell = new Reactor(handler);
            horizontalSpell = new Drive(handler);
            upSpell = new RoundTrip(handler);

            try
            {
                Initialise();
            }
            catch (Exception ex)
            {
                VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }

            configGroup = objectInfo.getConfigGroup();
            
            
        }

        private void RedSlash(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
        {
            if (clip.GetFrame(frame).eventInfo != "RedSlash") { return; }

            GameObject RedSlash = animator.gameObject.Child("RedSlash");

            if (RedSlash == null) { return; }

            if (!handler.ConsumeChaserBlade()) { return; }

            tk2dSpriteAnimator redAnim = RedSlash.GetComponent<tk2dSpriteAnimator>();
            redAnim.Sprite.color = new Color(1, 0.25f, 0.25f, 0.5f);
            
            RedSlash.GetComponent<NailSlash>().StartSlash();
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
                SlashAnimator = AnimationManager.GetDevilSwordAnimator(),
                SlashPrefabName = "DevilSwordSlash",
                AnimationName = "SlashEffect",
                LocalPosition = new Vector3(-1.1f, 0.4f, -0.001f),
                LocalScale = new Vector3(1.1f, 1f, 1),
                CreateRedSlash = true,
                RedSlashOffset = new Vector3(-0.3f, 0, 0)
            };
            devilSlash1.SetupSlash();
            objectInfo.slashObject = devilslash;
            
            devilslash.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggeredEvent += RedSlash;

            //Altslash
            GameObject devilslashalt = CrestManager.CloneSlashObject(hunter.Child("Slash"), devilroot);
            StandardSlash devilSlash2 = new StandardSlash
            {
                SlashObject = devilslashalt,
                SlashAnimator = AnimationManager.GetDevilSwordAnimator(),
                SlashPrefabName = "DevilSwordSlashAlt",
                AnimationName = "SlashAltEffect",
                LocalPosition = new Vector3(-0.6f, 0.4f, -0.001f),
                LocalScale = new Vector3(1.1f, 0.85f, 1),
                CreateRedSlash = true,
                RedSlashOffset = new Vector3(-0.3f, 0, 0)
            };
            devilSlash2.SetupSlash();
            objectInfo.slashAltObject = devilslashalt;

            devilslashalt.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggeredEvent += RedSlash;

            //Slashup
            GameObject devilupslash = CrestManager.CloneSlashObject(hunter.Child("UpSlash"), devilroot);
            StandardSlash devilSlash3 = new StandardSlash
            {
                SlashObject = devilupslash,
                SlashAnimator = AnimationManager.GetDevilSwordAnimator(),
                SlashPrefabName = "DevilSwordSlashUp",
                AnimationName = "SlashUpEffect",
                LocalPosition = new Vector3(-1f, 1.1f, -0.001f),
                LocalScale = new Vector3(1.1f, 1f, 1),
                CreateRedSlash = true,
                RedSlashOffset = new Vector3(-0.15f, 0.15f, 0)
            };
            devilSlash3.SetupSlash();
            objectInfo.slashUpObject = devilupslash;

            devilupslash.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggeredEvent += RedSlash;

            //Downspike
            GameObject devildownslash = CrestManager.CloneSlashObject(wanderer.Child("DownSlash"), devilroot);
            StandardSlash devildownspike = new StandardSlash
            {
                SlashObject = devildownslash,
                SlashAnimator = AnimationManager.GetDevilSwordAnimator(),
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
