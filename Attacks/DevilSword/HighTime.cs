using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Silksong.FsmUtil;
using VMCSE.AnimationHandler;

namespace VMCSE.Attacks.DevilSword
{
    public class HighTime : BaseNailArt
    {
        public HighTime() : base()
        {
            name = "HighTime";
        }

        public override void CreateAttack()
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            //High time
            GameObject highTime = UnityEngine.Object.Instantiate(attacks.Child("Charge Slash Wanderer"));
            highTime.transform.parent = CrestManager.devilroot.transform;
            highTime.transform.localScale = new Vector3(-0.9f, 1f, 1f);
            highTime.transform.localPosition = new Vector3(-1f, 0.5f, 0);
            highTime.GetComponent<DamageEnemies>().multiHitter = false;
            highTime.GetComponent<DamageEnemies>().doesNotTink = true;
            highTime.GetComponent<DamageEnemies>().allowedTinkFlags = ITinkResponder.TinkFlags.Projectile;
            highTime.GetComponent<DamageEnemies>().nailDamageMultiplier = 1.75f;
            highTime.GetComponent<DamageEnemies>().canTriggerBouncePod = true;
            highTime.GetComponent<DamageEnemies>().direction = 90;
            highTime.Child("Charge Slash Hornet Voice").SetActive(false);

            tk2dSpriteAnimation? library = AnimationManager.GetDevilSwordAnimator();
            if (library != null)
            {
                highTime.GetComponent<tk2dSpriteAnimator>().currentClip = library.GetClipByName("HighTimeEffect");
            } else
            {
                VMCSE.Instance.LogError("HighTimeEffect failed to load - Devilsword animator was null.");
            }

                highTime.CopyPolygonColliderFromPrefab("DevilSwordHighTime");
            highTime.Child("Lunge Stopper").CopyPolygonColliderFromPrefab("DevilSwordHighTime");

            PlayMakerFSM nailartFSM = fsm;


            GameObject lungeStopper = highTime.Child("Lunge Stopper");
            FsmOwnerDefault lungeStopperOwnerDefault = new FsmOwnerDefault() { gameObject = lungeStopper, ownerOption = OwnerDefaultOption.SpecifyGameObject };

            FsmState HighTimeAnticState = nailartFSM.AddState("High Time Antic");
            HighTimeAnticState.CopyActionData(nailartFSM.GetState("Wanderer Antic"));

            FsmState HighTimeState = nailartFSM.AddState("High Time");
            HighTimeState.CopyActionData(nailartFSM.GetState("Wanderer Attack"));
            HighTimeState.GetAction<Trigger2dEvent>(9).gameObject = lungeStopperOwnerDefault;
            HighTimeState.GetAction<Trigger2dEvent>(10).gameObject = lungeStopperOwnerDefault;
            HighTimeState.RemoveAction(7); //activate gameobject
            HighTimeState.RemoveAction(5); //accelerate to y
            HighTimeState.AddMethod(_ => { highTime.SetActive(true); });
            HighTimeState.RemoveAction(2); //set velocity to -45f x
            HighTimeState.GetAction<SetVelocity2d>().y = 30f;
            HighTimeState.GetAction<DecelerateXY>().decelerationX = 0f;
            HighTimeState.GetAction<DecelerateXY>().decelerationY = 0.9f;
            HighTimeState.GetAction<PlayAudioEvent>().audioClip = nailartFSM.GetState("Warrior2 Leap").GetAction<PlayAudioEvent>().audioClip;

            FsmState HighTimeBounceState = nailartFSM.AddState("High Time Bounce");
            HighTimeBounceState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.LocateMyFSM("Sprint").SetState("Regain Control Normal");
                HeroController.instance.SetStartWithHarpoonBounce();
                HeroController.instance.RegainControl();
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.StartDownspikeInvulnerability();
                //HeroController.instance.DownspikeBounce(false);

            });

            //transitions
            HighTimeAnticState.AddTransition("FINISHED", HighTimeState.name);
            HighTimeState.AddTransition("FINISHED", "Set Finished");
            HighTimeState.AddTransition("ATTACK LANDED", HighTimeBounceState.name);
            HighTimeBounceState.AddTransition("FINISHED", "Regain Full Control");

            SetStateAsInit(HighTimeAnticState.name);
        }
    }
}
