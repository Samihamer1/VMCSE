using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Components;
using VMCSE.Components.ObjectComponents;

namespace VMCSE.Attacks.DevilSword
{
    public class RoundTrip : BaseSpell
    {
        private GameObject activeSword;
        public RoundTrip(DevilCrestHandler handler) : base(handler)
        {
            EVENTNAME = EventNames.ROUNDTRIP;
            ICON = ResourceLoader.LoadAsset<Sprite>("RoundTripIcon");
            ICONGLOW = ResourceLoader.LoadAsset<Sprite>("RoundTripIconGlow");
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmOwnerDefault hornetOwnerDefault = Helper.GetHornetOwnerDefault();

            FsmState ThrowAnticState = fsm.AddState("Throw Antic");
            ThrowAnticState.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.GetComponent<SpriteFlash>().flashFocusHeal();
            });

            ThrowAnticState.AddAnimationAction("DevilSword", "RoundTrip Antic");
            ThrowAnticState.AddWatchAnimationActionTrigger("FINISHED");

            FsmState ThrowSwordState = fsm.AddState("Throw Round Trip");
            ThrowSwordState.AddMethod(_ =>
            {
                CreateSwordObject();

                if (handler.ConsumeChaserBlade())
                {
                    activeSword.name = AttackNames.ROUNDTRIPRED;
                    activeSword.GetComponent<tk2dSprite>().color = new Color(1, 0.25f, 0.25f, 0.5f);
                    activeSword.GetComponent<DamageEnemies>().stepsPerHit = 6;
                    activeSword.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.1f;
                }
            });

            FsmState ThrowRecoilState = fsm.AddState("Throw Recoil");
            ThrowRecoilState.AddMethod(_ =>
            {
                HeroController.instance.Recoil(!HeroController.instance.cState.facingRight, false);
            });

            FsmState ThrowEndDelayState = fsm.AddState("Throw End Delay");
            ThrowEndDelayState.AddAction(new DecelerateV2 { gameObject = hornetOwnerDefault, deceleration = 0.4f, brakeOnExit = false });
            ThrowEndDelayState.AddWatchAnimationAction("FINISHED");

            //transitions
            ThrowAnticState.AddTransition("FINISHED", ThrowSwordState.name);
            ThrowSwordState.AddTransition("FINISHED", ThrowRecoilState.name);
            ThrowRecoilState.AddTransition("FINISHED", ThrowEndDelayState.name);
            ThrowEndDelayState.AddTransition("FINISHED", "Special End");
            SetStateAsInit(ThrowAnticState.name);
        }

        public override bool OnManualCooldown()
        {
            if (activeSword == null)
            {
                return false;
            }
            return true;
        }

        private void CreateSwordObject()
        {
            GameObject sword = new GameObject(AttackNames.ROUNDTRIP);
            sword.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = sword.AddComponent<tk2dSpriteAnimator>();
            animator.library = AnimationManager.GetDevilSwordAnimator();
            animator.Play("RoundTripEffect");
            BoxCollider2D collider = sword.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            DamageEnemies dmg = sword.AddComponent<DamageEnemies>();
            dmg.useNailDamage = true;
            dmg.nailDamageMultiplier = 0.15f;
            dmg.multiHitter = true;
            dmg.multiHitFirstEffects = EnemyHitEffectsProfile.EffectsTypes.Minimal;
            dmg.multiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Minimal;
            dmg.magnitudeMult = 0f;
            dmg.stepsPerHit = 8;
            dmg.damageMultPerHit = new float[0];
            dmg.sourceIsHero = true;
            dmg.isNailAttack = true;
            dmg.attackType = AttackTypes.Nail;
            dmg.corpseDirection = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.corpseMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.currencyMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.slashEffectOverrides = new GameObject[0];
            dmg.DealtDamage = new UnityEngine.Events.UnityEvent();
            dmg.damageFSMEvent = "";
            dmg.dealtDamageFSMEvent = "";
            dmg.stunDamage = 0.1f;
            dmg.isHeroDamage = true;

            Rigidbody2D rigidbody = sword.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.SetVelocity(0, 0);

            sword.transform.position = HeroController.instance.transform.position;

            sword.AddComponent<RoundTripSword>();

            activeSword = sword;

            if (!handler.GetTrigger().IsInTrigger()) { return; }

            //extra orbiting swords
            CreateOrbitingSwords();

            
        }

        private void CreateOrbitingSwords()
        {
            GameObject sword1 = GameObject.Instantiate(activeSword);
            GameObject sword2 = GameObject.Instantiate(activeSword);

            OrbitingRoundTripSword sword1orbit = CreateOrbitingSword(sword1);
            OrbitingRoundTripSword sword2orbit = CreateOrbitingSword(sword2);
            sword2orbit.SetTime(0.5f); //so it is on the other side of orbit
        }

        private OrbitingRoundTripSword CreateOrbitingSword(GameObject sword1)
        {
            sword1.name = AttackNames.ROUNDTRIPMINI;
            sword1.transform.parent = activeSword.transform;
            GameObject.Destroy(sword1.GetComponent<RoundTripSword>());
            sword1.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.1f;
            sword1.GetComponent<DamageEnemies>().stepsPerHit = 16;
            OrbitingRoundTripSword orbit = sword1.AddComponent<OrbitingRoundTripSword>();
            sword1.SetActive(true);
            sword1.GetComponent<tk2dSpriteAnimator>().Play("RoundTripEffect");
            sword1.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;

            return orbit;
        }

    }
}
