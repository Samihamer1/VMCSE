using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Text;
using SFCore.Utils;
using UnityEngine;

namespace VMCSE.Attacks.DevilSword
{
    public class DevilSwordDownslash : BaseAttack
    {
        private GameObject downslashObject;

        public DevilSwordDownslash(GameObject downslashObject)
        {
            this.downslashObject = downslashObject;
        }

        public override void CreateAttack()
        {
            fsm = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (fsm == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            FsmOwnerDefault hornetOwnerDefault = Helper.GetHornetOwnerDefault();

            FsmEvent DEVILDOWNSLASH = fsm.CreateFsmEvent("DEVILSWORD DOWNSLASH");

            
            //Setup state

            FsmState DevilDownslashSetupState = fsm.AddState("Devilsword Downslash Setup");
            DevilDownslashSetupState.AddMethod(() =>
            {

                HeroController.instance.RelinquishControlNotVelocity();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                fsm.GetFsmBoolVariable("Disabled Animation").Value = true;
            });

            //Antic State

            FsmState DevilDownslashAnticState = fsm.AddState("Devilsword Downslash Antic");
            DevilDownslashAnticState.AddAnimationAction("DevilSword", "DownSpike Antic");
            DevilDownslashAnticState.AddWatchAnimationAction("FINISHED");

            //Slash state

            FsmState DevilDownslashState = fsm.AddState("Devilsword Downslash");
            DevilDownslashState.AddMethod(() =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, -35);
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.SetCState("downAttacking", true);
                downslashObject.GetComponent<NailSlash>().StartSlash();
            });
            DevilDownslashState.AddAnimationAction("DevilSword", "DownSpike");
            DevilDownslashState.AddWatchAnimationAction("ANIM END");

            //Slash end state

            FsmState DevilDownslashEndState = fsm.AddState("Devilsword Downslash End");
            DevilDownslashEndState.AddMethod(() =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.SetCState("downAttacking", false);
                HeroController.instance.FinishDownspike(true);
            });

            //Slash bounce state

            FsmState DevilDownslashBounceState = fsm.AddState("Devilsword Downslash Bounce");
            DevilDownslashBounceState.AddMethod(() =>
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
