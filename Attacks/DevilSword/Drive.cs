using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.Slashes;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class Drive : BaseSpell
    {
        //we want a 1 second cooldown or something on this spell. its too much.
        //good luck o7
        private GameObject driveObject;
        private GameObject driveAltObject;
        private GameObject driveFinalObject;
        private tk2dSpriteAnimation shamanLibrary;
        private float chargeTimer = 0f;

        private const float COOLDOWNTIME = 2f;
        public Drive(DevilCrestHandler handler) : base(handler)
        {
            EVENTNAME = EventNames.DRIVE;
            ICON = ResourceLoader.LoadAsset<Sprite>("DriveIcon");
            ICONGLOW = ResourceLoader.LoadAsset<Sprite>("DriveIconGlow");
        }

        private void RedColor()
        {
            driveObject.GetComponent<tk2dSprite>().color = new Color(1, 0.2f, 0.2f, 0.9f);
            driveAltObject.GetComponent<tk2dSprite>().color = new Color(1, 0.2f, 0.2f, 0.9f);
            driveFinalObject.GetComponent<tk2dSprite>().color = new Color(1, 0.2f, 0.2f, 0.9f);
        }

        private void ModifySlash(GameObject slash)
        {
            slash.GetComponent<NailSlash>().hc = HeroController.instance;
            slash.GetComponent<DamageEnemies>().silkGeneration = HitSilkGeneration.None;
            slash.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.45f;
            slash.GetComponent<NailSlashTravel>().travelDistance = new Vector2(-9, 0);
            slash.Child("Particle Slash Trail").GetComponent<ParticleSystem>().startColor = new Color(1, 0.2f, 0.2f);
            slash.SetActive(true);

            slash.GetComponent<NailSlash>().AttackStarting += RedColor;
        }

        private void SetupAttacks()
        {

            GameObject hornet = HeroController.instance.gameObject;
            GameObject specialattacks = hornet.Child("Special Attacks");
            GameObject attacks = hornet.Child("Attacks");
            GameObject shaman = attacks.Child("Shaman");
            driveObject = CrestManager.CloneSlashObject(shaman.Child("Slash"), specialattacks);
            ModifySlash(driveObject);
            driveObject.name = AttackNames.DRIVESLASH1;
            driveAltObject = CrestManager.CloneSlashObject(shaman.Child("AltSlash"), specialattacks);
            ModifySlash(driveAltObject);
            driveAltObject.name = AttackNames.DRIVESLASH2;

            driveFinalObject = CrestManager.CloneSlashObject(shaman.Child("Slash"), specialattacks);            
            ModifySlash(driveFinalObject);
            driveFinalObject.name = AttackNames.REDSLASH;
            driveFinalObject.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.25f;
        }

        private IEnumerator ChargeUp()
        {
            while (InputHandler.Instance.inputActions.QuickCast.IsPressed)
            {
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= 0.25f)
                {
                    fsm.SendEvent("CHARGE");
                    yield break;
                }
                yield return new Wait();
            }
            fsm.SendEvent("CANCELCHARGE");
        }

        private IEnumerator DoubleDrive()
        {
            driveObject.GetComponent<NailSlash>().StartSlash();
            yield return new WaitForSeconds(0.1f);
            if (fsm.ActiveStateName != "Drive End Delay") { yield break; }
            driveAltObject.GetComponent<NailSlash>().StartSlash();
        }

        private IEnumerator TripleDrive()
        {
            yield return DoubleDrive();
            yield return new WaitForSeconds(0.1f);
            if (fsm.ActiveStateName != "Drive End Delay") { yield break; }
            driveFinalObject.GetComponent<NailSlash>().StartSlash();
        }

        public override void CreateAttack()
        {
            SetupAttacks();

            if (fsm == null) { return; }

            FsmOwnerDefault hornetOwnerDefault = Helper.GetHornetOwnerDefault();

            FsmState DriveAnticState = fsm.AddState("Drive Antic");
            DriveAnticState.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.GetComponent<SpriteFlash>().flashFocusHeal();
                chargeTimer = 0f;
            });
            //DriveAnticState.AddAction(new DecelerateV2 { gameObject = hornetOwnerDefault, brakeOnExit = false, deceleration = 0.85f });
            DriveAnticState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("DriveSlash"));
            DriveAnticState.AddWatchAnimationActionTrigger("FINISHED");
            

            FsmState DriveChargeState = fsm.AddState("Drive Charge");
            //DriveChargeState.AddAnimationAction("DevilSword", "Drive Antic");
            DriveChargeState.AddWatchAnimationAction("FINISHED");

            FsmState DriveReleaseState = fsm.AddState("Drive Release");
            DriveReleaseState.AddMethod(_ =>
            {
                StartCooldownTimer(COOLDOWNTIME);

                if (handler.GetTrigger().IsInTrigger())
                {
                    GameManager.instance.StartCoroutine(DoubleDrive());
                    return;
                }

                driveObject.GetComponent<NailSlash>().StartSlash();
                
            });
            DriveReleaseState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("DriveSlashFast"));

            FsmState DriveCheckHoldState = fsm.AddState("Drive Check Hold");
            DriveCheckHoldState.AddMethod(_ =>
            {
                GameManager.instance.StartCoroutine(ChargeUp());
            });
            DriveCheckHoldState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("Drive Antic"));

            FsmState DriveReleaseLargeState = fsm.AddState("Drive Release Large");
            DriveReleaseLargeState.AddMethod(_ =>
            {
                StartCooldownTimer(COOLDOWNTIME);
                if (handler.GetTrigger().IsInTrigger())
                {
                    GameManager.instance.StartCoroutine(TripleDrive());
                    return;
                }
                GameManager.instance.StartCoroutine(DoubleDrive());
                
            });
            DriveReleaseLargeState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("DriveSlashFast"));

            FsmState DriveRecoilLargeState = fsm.AddState("Drive Recoil Large");
            DriveRecoilLargeState.AddMethod(_ =>
            {
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.Recoil(!HeroController.instance.cState.facingRight, false);
            });

            FsmState DriveRecoilState = fsm.AddState("Drive Recoil");
            DriveRecoilState.AddMethod(_ =>
            {
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.Recoil(!HeroController.instance.cState.facingRight, false);
            });

            FsmState DriveEndDelayState = fsm.AddState("Drive End Delay");
            //DriveRecoilState.AddAnimationAction("Hornet CrestWeapon Shaman Anim", "Slash_Charged");
            DriveEndDelayState.AddAction(new DecelerateV2 { gameObject = hornetOwnerDefault, deceleration = 0.4f, brakeOnExit = false });
            DriveEndDelayState.AddWatchAnimationAction("FINISHED");

            //cancel cleanup
            FsmState CancelAllState = fsm.GetState("Cancel All");
            CancelAllState.AddMethod(_ =>
            {
                driveObject.GetComponent<NailSlash>().CancelAttack();
                driveAltObject.GetComponent<NailSlash>().CancelAttack();
                driveFinalObject.GetComponent<NailSlash>().CancelAttack();
            });

            //transitions
            DriveAnticState.AddTransition("FINISHED", DriveCheckHoldState.name);
            DriveCheckHoldState.AddTransition("CHARGE", DriveChargeState.name);
            DriveCheckHoldState.AddTransition("CANCELCHARGE", DriveReleaseState.name);
            DriveReleaseState.AddTransition("FINISHED", DriveRecoilState.name);
            DriveChargeState.AddTransition("FINISHED", DriveReleaseLargeState.name);
            DriveReleaseLargeState.AddTransition("FINISHED", DriveRecoilLargeState.name);
            DriveRecoilLargeState.AddTransition("FINISHED", DriveEndDelayState.name);
            DriveRecoilState.AddTransition("FINISHED", DriveEndDelayState.name);
            DriveEndDelayState.AddTransition("FINISHED", "Special End");

            SetStateAsInit(DriveAnticState.name);
        }

        public override bool OnManualCooldown()
        {
            return false;
        }
    }
}
