using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.DevilSword;
using VMCSE.Attacks.KingCerberus;
using VMCSE.Attacks.Slashes;

namespace VMCSE.Weapons
{
    internal class KingCerberusWeapon : WeaponBase
    {
        public KingCerberusWeapon()
        {
            try
            {
                CreateWeaponObjects();
            }
            catch (Exception ex)
            {
                VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }


            name = WeaponNames.KINGCERBERUS;
            weaponEvent = EventNames.KINGCERBERUS;
            dashSlash = new KingSlayer();
            //cycloneSlash = new HighTime();
            //greatSlash = new SwordFormation();
            //specialDownSlash = new DevilSwordDownslash(objectInfo.slashDownObject);
            specialDashStab = new CerberusDashStab(handler, objectInfo.slashdashstabObject);

            //downSpell = new Reactor(handler);
            //horizontalSpell = new Drive(handler);
            //upSpell = new RoundTrip(handler);

            try
            {
                Initialise();
            }
            catch (Exception ex)
            {
                VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }

            configGroup = objectInfo.getConfigGroup();

            HeroControllerConfig configRef = config.GetConfig();
            configRef.heroAnimOverrideLib = AnimationManager.GetKingCerberusAnimator();
            configRef.attackCooldownTime = 0.4f;
            configRef.attackDuration = 0.35f;
            configRef.attackRecoveryTime = 0.15f;

            configRef.dashStabTime = 0.175f;
            configRef.dashStabSpeed = -40f;
            configRef.dashStabBounceJumpSpeed = 18f;

            configRef.downSlashType = HeroControllerConfig.DownSlashTypes.Slash;
        }

        private void CreateWeaponObjects()
        {
            objectInfo = new CrestManagement.CrestObjectInfo();
            GameObject devilroot = CrestManager.devilroot;

            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");
            GameObject hunter = attacks.Child("Default");

            AudioClip slashAudio = HeroController.instance.gameObject.Child("Attacks").Child("Witch").Child("Slash").GetComponent<AudioSource>().clip;

            //Regular slash
            GameObject devilslash = new GameObject("KingCerberus Slash");
            devilslash.transform.parent = devilroot.transform;
            devilslash.transform.localPosition = new Vector3(-1.1f, 0.4f, -0.001f);
            StandardSlash slash1 = devilslash.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(-1.1f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "SlashEffect");
            slash1.SetNailDamageMultiplier(1f);
            slash1.SetColliderSize("DevilSwordSlash");
            slash1.SetAudioClip(slashAudio);
            slash1.CreateRedSlash(new Vector3(0.3f, 0, 0));
            slash1.SetColor(ColorConstants.CerberusBlue);

            objectInfo.slashObject = devilslash;

            //Altslash
            GameObject devilslashalt = new GameObject("KingCerberus SlashAlt");
            devilslashalt.transform.parent = devilroot.transform;
            devilslashalt.transform.localPosition = new Vector3(-1f, 0.4f, -0.001f);
            StandardSlash slash2 = devilslashalt.AddComponent<StandardSlash>();
            slash2.SetScale(new Vector3(-1.1f, 1f, 1));
            slash2.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "SlashAltEffect");
            slash2.SetNailDamageMultiplier(1f);
            slash2.SetColliderSize("DevilSwordSlashAlt");
            slash2.SetAudioClip(slashAudio);
            slash2.CreateRedSlash(new Vector3(0.3f, 0, 0));
            slash2.SetColor(ColorConstants.CerberusBlue);

            objectInfo.slashAltObject = devilslashalt;

            //Slash up
            GameObject cerberusSlashUp = new GameObject("KingCerberus SlashUp");
            cerberusSlashUp.transform.parent = devilroot.transform;
            cerberusSlashUp.transform.localPosition = new Vector3(0, 1.6f, -0.001f);
            StandardSlash slash3 = cerberusSlashUp.AddComponent<StandardSlash>();
            slash3.SetScale(new Vector3(1f, 1f, 1));
            slash3.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "SlashUpEffect");
            slash3.SetNailDamageMultiplier(1f);
            slash3.SetDamageDirection(90f);
            slash3.SetColliderSize("DevilSwordSlashAlt");
            slash3.SetAudioClip(slashAudio);
            slash3.CreateRedSlash(new Vector3(0f, 0.3f, 0));
            slash3.SetColor(ColorConstants.CerberusBlue);

            objectInfo.slashUpObject = cerberusSlashUp;

            //Slash up
            GameObject cerberusSlashDown = new GameObject("KingCerberus SlashDown");
            cerberusSlashDown.transform.parent = devilroot.transform;
            cerberusSlashDown.transform.localPosition = new Vector3(0, -1.5f, -0.001f);
            StandardSlash slash4 = cerberusSlashDown.AddComponent<StandardSlash>();
            slash4.SetScale(new Vector3(1.15f, 1.25f, 1));
            slash4.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "SlashDownEffect");
            slash4.SetNailDamageMultiplier(1f);
            slash4.SetDamageDirection(270f);
            slash4.SetDownAttack();
            slash4.SetColliderSize("DevilSwordSlashAlt");
            slash4.SetAudioClip(slashAudio);
            slash4.CreateRedSlash(new Vector3(0f, -0.3f, 0));
            slash4.SetColor(ColorConstants.CerberusBlue);

            objectInfo.slashDownObject = cerberusSlashDown;


            //I SPENT LIKE AN HOUR FAILING TO CREATE A DASH STAB FROM SCRATCH
            //IF YOU COME BACK TO THIS, FORGET THE EXISTING FSM STATES
            //FOR THE LOVE OF GOD JUST MAKE YOUR OWN
            GameObject cerberusDashStab = UnityEngine.Object.Instantiate(hunter.Child("Dash Stab"));
            cerberusDashStab.transform.parent = devilroot.transform;
            cerberusDashStab.transform.localPosition = new Vector3(-0.3f, -1.3f, -0.001f);
            DashStabNailAttack dashstab = cerberusDashStab.GetComponent<DashStabNailAttack>();
            dashstab.scale.x = -1.2f;
            dashstab.heroController = HeroController.instance;
            dashstab.hc = HeroController.instance;
            dashstab.enabled = true;
            cerberusDashStab.GetComponent<tk2dSpriteAnimator>().library = AnimationManager.GetKingCerberusAnimator();
            cerberusDashStab.GetComponent<tk2dSpriteAnimator>().defaultClipId = AnimationManager.GetKingCerberusAnimator().GetClipIdByName("DashStabEffect");
            cerberusDashStab.GetComponent<tk2dSpriteAnimator>().currentClip = AnimationManager.GetKingCerberusAnimator().GetClipByName("DashStabEffect");
            DamageEnemies damager = cerberusDashStab.GetComponent<DamageEnemies>();
            damager.magnitudeMult = 1.5f;
            damager.nailDamageMultiplier = 1.1f / 1.5f; //Accounting for flintslate
            PolygonCollider2D collider = cerberusDashStab.GetComponent<PolygonCollider2D>();
            collider.points = new Vector2[] //I don't like to do this but if i spend another minute on this attack rn i will die
            {
                new Vector2(-0.63f, 1.5f),
                new Vector2(-2.5f, -0.1f),
                new Vector2(-1.85f, -2.1f),
                new Vector2(0.6f, -2.1f),
                new Vector2(1.3f, 0.1f)
            }; 


            objectInfo.slashdashstabObject = cerberusDashStab;

            ////Wall slash (temporarily just regular
            //objectInfo.slashWallObject = devilslash;

        }
    }
}
