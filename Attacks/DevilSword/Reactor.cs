using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using VMCSE.AnimationHandler;
using VMCSE.Components;
using VMCSE.Components.ObjectComponents;

namespace VMCSE.Attacks.DevilSword
{
    public class Reactor : BaseSpell
    {
        private GameObject swordRoot;
        private GameObject[] swords;

        public Reactor(DevilCrestHandler handler) : base(handler)
        {
            this.EVENTNAME = "REACTOR";
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState ReactorCheckState = fsm.AddState("Reactor Check");
            ReactorCheckState.AddMethod(_ =>
            {
                if (HeroController.instance.cState.wallClinging)
                {
                    fsm.SendEvent("FINISHED");
                }

                bool charge = handler.UseAirCharge();
                if (charge)
                {
                    fsm.SendEvent("USE");
                }
            });

            FsmState ReactorSummonState = fsm.AddState("Reactor Summon");
            ReactorSummonState.AddMethod(_ =>
            {
                DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
                if (handler == null) { VMCSE.Instance.LogError("DevilCrestHandler not found!"); return; }
                handler.RefreshChaserBlades();
            });

            FsmState ReactorBounceState = fsm.AddState("Reactor Bounce");
            ReactorBounceState.AddMethod(_ =>
            {
                try
                {
                    GameObject effect = CreateEffect();
                    GameManager.instance.StartCoroutine(AnimationManager.PlayAnimationThenDestroy(effect.GetComponent<tk2dSpriteAnimator>(), "ReactorEffect"));
                    effect.transform.position = HeroController.instance.transform.position - new Vector3(0, 0.75f);
                    effect.SetActive(true);


                    HeroController.instance.gameObject.LocateMyFSM("Sprint").SetState("Regain Control Normal");
                    HeroController.instance.DownspikeBounce(false);
                }
                catch (Exception ex)
                {
                    VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
                }


            });

            //Transition
            ReactorCheckState.AddTransition("FINISHED", "Special End");
            ReactorCheckState.AddTransition("USE", ReactorSummonState.name);
            ReactorSummonState.AddTransition("FINISHED", ReactorBounceState.name);
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
