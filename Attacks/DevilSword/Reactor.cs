using HutongGames.PlayMaker;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class Reactor : BaseSpell
    {
        public Reactor(DevilCrestHandler handler) : base(handler)
        {
            this.EVENTNAME = "REACTOR";
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState ReactorCheckState = fsm.AddState("Reactor Check");
            ReactorCheckState.AddMethod(() =>
            {
                bool charge = handler.UseAirCharge();
                if (charge)
                {
                    fsm.SendEvent("USE");
                }
            });

            FsmState ReactorBounceState = fsm.AddState("Reactor Bounce");
            ReactorBounceState.AddMethod(() =>
            {
                GameObject effect = CreateEffect();
                GameManager.instance.StartCoroutine(AnimationManager.PlayAnimationThenDestroy(effect.GetComponent<tk2dSpriteAnimator>(), "ReactorEffect"));
                effect.transform.position = HeroController.instance.transform.position - new Vector3(0, 0.75f);
                effect.SetActive(true);

                //HeroController.instance.CancelDash();
                //HeroController.instance.RelinquishControl();
                //HeroController.instance.AffectedByGravity(false);
                //HeroController.instance.SetDoFullJump();
                //HeroController.instance.Jump();
                HeroController.instance.gameObject.LocateMyFSM("Sprint").SetState("Regain Control Normal");
                HeroController.instance.DownspikeBounce(false);
                //HeroController.instance.cState.onGround = false;
                //HeroController.instance.ShroomBounce();
                //HeroController.instance.ResetAnimationDownspikeBounce();

            });

            //Transition
            ReactorCheckState.AddTransition("FINISHED", "Special End");
            ReactorCheckState.AddTransition("USE", ReactorBounceState.name);
            ReactorBounceState.AddTransition("FINISHED", "Special End");

            SetStateAsInit(ReactorCheckState.name);
        }

        private GameObject CreateEffect()
        {
            GameObject effect = new GameObject();
            effect.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = effect.AddComponent<tk2dSpriteAnimator>();
            animator.Library = AnimationManager.DevilSwordAnimator.GetComponent<tk2dSpriteAnimator>().Library;

            return effect;
        }
    }
}
