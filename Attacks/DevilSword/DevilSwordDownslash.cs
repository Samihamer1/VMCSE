using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Text;
using Silksong.FsmUtil;
using UnityEngine;
using VMCSE.Components;
using VMCSE.AnimationHandler;

namespace VMCSE.Attacks.DevilSword
{
    public class DevilSwordDownslash : BaseAttack
    {
        private GameObject downslashObject;
        private GameObject redslashObject;

        public DevilSwordDownslash(GameObject downslashObject)
        {
            this.downslashObject = downslashObject;
        }

        private void CreateRedSlash()
        {
            GameObject redslash = GameObject.Instantiate(downslashObject);
            redslash.transform.parent = downslashObject.transform;
            redslash.transform.localPosition = new Vector3(-0.6f, 0, 0);

            redslash.name = AttackNames.REDSLASH;
            redslash.GetComponent<DamageEnemies>().silkGeneration = HitSilkGeneration.None;
            redslash.GetComponent<DamageEnemies>().nailDamageMultiplier *= 0.3f;
            redslash.GetComponent<NailSlash>().animName = "DownSpike Red";
            redslash.GetComponent<NailSlash>().hc = HeroController.instance;

            redslashObject = redslash;
        }

        public override void CreateAttack()
        {
            CreateRedSlash();

            fsm = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (fsm == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            FsmOwnerDefault hornetOwnerDefault = Helper.GetHornetOwnerDefault();

            FsmEvent DEVILDOWNSLASH = fsm.CreateFsmEvent("DEVILSWORD DOWNSLASH");

            
            //Setup state

            FsmState DevilDownslashSetupState = fsm.AddState("Devilsword Downslash Setup");
            DevilDownslashSetupState.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControlNotVelocity();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                fsm.GetBoolVariable("Disabled Animation").Value = true;
            });

            //Antic State

            FsmState DevilDownslashAnticState = fsm.AddState("Devilsword Downslash Antic");
            DevilDownslashAnticState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("DownSpike Antic"));
            DevilDownslashAnticState.AddWatchAnimationAction("FINISHED");

            //Slash state

            FsmState DevilDownslashState = fsm.AddState("Devilsword Downslash");       
            DevilDownslashState.AddAnimationAction(AnimationManager.GetDevilSwordAnimator().GetClipByName("DownSpike"));
            DevilDownslashState.AddWatchAnimationAction("ANIM END");
            DevilDownslashState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, -35);
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.SetCState("downAttacking", true);
                downslashObject.GetComponent<NailSlash>().StartSlash();

                DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
                if (handler == null) { return; }

                if (!handler.ConsumeChaserBlade()) { return; }
                redslashObject.GetComponent<NailSlash>().StartSlash();
                redslashObject.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;
            });

            //Slash end state

            FsmState DevilDownslashEndState = fsm.AddState("Devilsword Downslash End");
            DevilDownslashEndState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.SetCState("downAttacking", false);
                HeroController.instance.FinishDownspike(true);
            });

            //Slash bounce state

            FsmState DevilDownslashBounceState = fsm.AddState("Devilsword Downslash Bounce");
            DevilDownslashBounceState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.SetCState("downAttacking", false);
                HeroController.instance.SetStartWithDownSpikeBounce();
            });

            //Transitions

            FsmState idleState = fsm.GetState("Idle");
            idleState.AddTransition(DEVILDOWNSLASH.name, DevilDownslashSetupState.name);
            DevilDownslashSetupState.AddTransition("FINISHED", DevilDownslashAnticState.name);
            DevilDownslashAnticState.AddTransition("FINISHED", DevilDownslashState.name);
            DevilDownslashState.AddTransition("ANIM END", DevilDownslashEndState.name);

            DevilDownslashState.AddTransition("ATTACK LANDED", DevilDownslashBounceState.name);
            DevilDownslashState.AddTransition("BOUNCE TINKED", DevilDownslashBounceState.name);
            DevilDownslashState.AddTransition("BOUNCE CANCEL", DevilDownslashBounceState.name);
            DevilDownslashState.AddTransition("LEAVING SCENE", DevilDownslashEndState.name);
            DevilDownslashEndState.AddTransition("FINISHED", "End");
            DevilDownslashBounceState.AddTransition("FINISHED", "End");
        }
    }
}
