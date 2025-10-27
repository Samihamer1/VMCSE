using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Silksong.FsmUtil;
using VMCSE.AnimationHandler;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class HighTime : BaseNailArt
    {
        private GameObject RegularSlash;
        private GameObject RedSlash;

        public HighTime() : base()
        {
            name = "HighTime";
        }

        private GameObject CreateSlashTemplate(string name, float multiplier)
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            GameObject slash = new GameObject(name);
            slash.transform.parent = CrestManager.devilroot.transform;
            slash.transform.localScale = new Vector3(-0.9f, 1f, 1f);
            slash.transform.localPosition = new Vector3(-1.4f, 0.5f, 0);

            tk2dSprite sprite = slash.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = slash.AddComponent<tk2dSpriteAnimator>();

            //animation
            tk2dSpriteAnimation? library = AnimationManager.GetDevilSwordAnimator();
            if (library != null)
            {
                slash.GetComponent<tk2dSpriteAnimator>().currentClip = library.GetClipByName("HighTimeEffect");
            }
            else
            {
                VMCSE.Instance.LogError("HighTimeEffect failed to load - Devilsword animator was null.");
            }

            DeactivateAfter2dtkAnimation deactivate = slash.AddComponent<DeactivateAfter2dtkAnimation>();
            deactivate.animators = new List<tk2dSpriteAnimator> { animator };

            animator.AnimationEventTriggeredEvent += ActivateRedSlash;

            //hitbox
            slash.CopyPolygonColliderFromPrefab("DevilSwordHighTime");

            //damage
            DamageEnemies dmg = slash.AddDamageEnemies();
            dmg.canTriggerBouncePod = true;
            dmg.direction = 90;
            dmg.allowedTinkFlags = ITinkResponder.TinkFlags.None;
            dmg.doesNotTinkThroughWalls = true;
            dmg.dealtDamageFSM = fsm;
            dmg.dealtDamageFSMEvent = "ATTACK LANDED";

            //tink
            NailSlashTerrainThunk thunk = slash.AddComponent<NailSlashTerrainThunk>();
            thunk.generateChild = true;
            thunk.handleTink = true;
            

            return slash;
        }

        private void ActivateRedSlash(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
        {
            //checking if valid red slash trigger
            if (animator.gameObject.name != AttackNames.HIGHTIME) { return; }
            if (clip.GetFrame(frame).eventInfo != "RedSlash") { return; }

            //checking for handler
            DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            if (!handler.ConsumeChaserBlade()) { return; }

            //do it
            RedSlash.SetActive(true);
            RedSlash.GetComponent<tk2dSprite>().color = new Color(1, 0.25f, 0.25f, 0.5f);
        }

        private void CreateSlashes()
        {
            RegularSlash = CreateSlashTemplate(AttackNames.HIGHTIME, 1.5f);
            
            //empowered slash
            RedSlash = CreateSlashTemplate(AttackNames.HIGHTIMERED, 0.5f);
            

        }

        public override void CreateAttack()
        {
            CreateSlashes();

            PlayMakerFSM nailartFSM = fsm;
            FsmOwnerDefault hornetOwnerDefault = Helper.GetHornetOwnerDefault();

           
            FsmState HighTimeAnticState = nailartFSM.AddState("High Time Antic");
            HighTimeAnticState.CopyActionData(nailartFSM.GetState("Wanderer Antic"));


            FsmState HighTimeState = nailartFSM.AddState("High Time");
            HighTimeState.AddMethod(_ => { 
                RegularSlash.SetActive(true); 
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 30f);
            });
            HighTimeState.AddAction(new DecelerateXY { gameObject = hornetOwnerDefault, decelerationX = 0f, decelerationY = 0.9f });
            HighTimeState.AddWatchAnimationAction("FINISHED");


            FsmState HighTimeBounceState = nailartFSM.AddState("High Time Bounce");
            HighTimeBounceState.AddMethod(_ =>
            {
                //HeroController.instance.gameObject.LocateMyFSM("Sprint").SetState("Regain Control Normal");
                HeroController.instance.SetStartWithHarpoonBounce();
                //HeroController.instance.RegainControl();
                //HeroController.instance.AffectedByGravity(true);
                HeroController.instance.StartDownspikeInvulnerability();
                //HeroController.instance.DownspikeBounce(false);

            });


            //transitions
            HighTimeAnticState.AddTransition("FINISHED", HighTimeState.name);
            HighTimeState.AddTransition("FINISHED", "Set Finished");
            HighTimeState.AddTransition("ATTACK LANDED", HighTimeBounceState.name);
            HighTimeBounceState.AddTransition("FINISHED", "Set Finished");

            //adding cancellation of attack when hit
            fsm.AddMethod("Cancel All", _ =>
            {
                RegularSlash.SetActive(false);
                RedSlash.SetActive(false);
            });

            SetStateAsInit(HighTimeAnticState.name);
        }
    }
}
