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
        public DevilSwordWeapon() {
            try
            {
                CreateWeaponObjects();
            }
            catch (Exception ex)
            {
                VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }
            

            name = WeaponNames.DEVILSWORD;
            weaponEvent = EventNames.DEVILSWORD;
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

            //Config
            HeroControllerConfig configRef = config.GetConfig();
            configRef.heroAnimOverrideLib = AnimationManager.GetDevilSwordAnimator();
            configRef.attackCooldownTime = 0.45f;
            configRef.attackDuration = 0.35f;
            configRef.attackRecoveryTime = 0.25f;
            configRef.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
            configRef.downSlashEvent = "DEVILSWORD DOWNSLASH";

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

            

            //special case for our dash stab
            GameObject devildashstab = UnityEngine.Object.Instantiate(hunter.Child("Dash Stab"));
            devildashstab.transform.parent = devilroot.transform;
            devildashstab.transform.localPosition = new Vector3(0, 0, -0.001f);
            DashStabNailAttack dashstab = devildashstab.GetComponent<DashStabNailAttack>();
            dashstab.scale.x = -1.2f;
            dashstab.heroController = HeroController.instance;
            dashstab.hc = HeroController.instance;
            dashstab.enabled = true;
            devildashstab.GetComponent<tk2dSpriteAnimator>().library = AnimationManager.GetDevilSwordAnimator();
            devildashstab.GetComponent<tk2dSpriteAnimator>().defaultClipId = AnimationManager.GetDevilSwordAnimator().GetClipIdByName("DashStabEffect");
            devildashstab.GetComponent<tk2dSpriteAnimator>().currentClip = AnimationManager.GetDevilSwordAnimator().GetClipByName("DashStabEffect");
            DamageEnemies damager = devildashstab.GetComponent<DamageEnemies>();
            damager.magnitudeMult = 2.25f;
            damager.nailDamageMultiplier = 1.25f;
            //damager.doesNotTink = true;

            objectInfo.slashdashstabObject = devildashstab;

            //Regular slash
            GameObject devilslash = new GameObject("DevilSword Slash");
            devilslash.transform.parent = devilroot.transform;
            devilslash.transform.localPosition = new Vector3(-1.1f, 0.4f, -0.001f);
            StandardSlash slash1 = devilslash.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(1.1f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "SlashEffect");
            slash1.SetNailDamageMultiplier(1f);
            slash1.SetColliderSize("DevilSwordSlash");
            slash1.CreateRedSlash(new Vector3(-0.3f, 0, 0));

            objectInfo.slashObject = devilslash;

            //Altslash
            GameObject devilslashalt = new GameObject("DevilSword SlashAlt");
            devilslashalt.transform.parent = devilroot.transform;
            devilslashalt.transform.localPosition = new Vector3(-0.6f, 0.4f, -0.001f);
            StandardSlash slash2 = devilslashalt.AddComponent<StandardSlash>();
            slash2.SetScale(new Vector3(1.1f, 0.85f, 1));
            slash2.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "SlashAltEffect");
            slash2.SetNailDamageMultiplier(1f);
            slash2.SetColliderSize("DevilSwordSlashAlt");
            slash2.CreateRedSlash(new Vector3(-0.3f, 0, 0));

            objectInfo.slashAltObject = devilslashalt;

            //Slashup
            GameObject devilupslash = new GameObject("DevilSword UpSlash");
            devilupslash.transform.parent = devilroot.transform;
            devilupslash.transform.localPosition = new Vector3(-1f, 1.1f, -0.001f);
            StandardSlash slash3 = devilupslash.AddComponent<StandardSlash>();
            slash3.SetScale(new Vector3(1.1f, 1f, 1));
            slash3.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "SlashUpEffect");
            slash3.SetNailDamageMultiplier(1f);
            slash3.SetColliderSize("DevilSwordSlashUp");
            slash3.SetDamageDirection(90f);
            slash3.CreateRedSlash(new Vector3(-0.15f, 0.15f, 0));

            objectInfo.slashUpObject = devilupslash;

            //Downspike
            GameObject devildownslash = new GameObject("DevilSword DownSlash");
            devildownslash.transform.parent = devilroot.transform;
            devildownslash.transform.localPosition = new Vector3(0,0, -0.001f);
            StandardSlash slash4 = devildownslash.AddComponent<StandardSlash>();
            slash4.SetScale(new Vector3(1.1f, 1f, 1));
            slash4.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "DownSpikeEffect");
            slash4.SetNailDamageMultiplier(1f);
            slash4.SetColliderSize("DevilSwordDownspike");
            slash4.SetDamageDirection(270f);
            slash4.SetDownAttack();

            objectInfo.slashDownObject = devildownslash;

            //Wall slash
            GameObject devilwallslash = new GameObject("DevilSword WallSlash");
            devilwallslash.transform.parent = devilroot.transform;
            devilwallslash.transform.localPosition = new Vector3(1.1f, 0.4f, -0.001f);
            StandardSlash slash5 = devilwallslash.AddComponent<StandardSlash>();
            slash5.SetScale(new Vector3(-1.1f, 1, 1));
            slash5.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "SlashEffect");
            slash5.SetNailDamageMultiplier(1f);
            slash5.SetColliderSize("DevilSwordSlash");
            slash5.CreateRedSlash(new Vector3(-0.3f, 0, 0));

            objectInfo.slashWallObject = devilwallslash;

        }
    }
}
