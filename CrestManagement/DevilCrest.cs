using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using SFCore.Utils;
using UnityEngine;
using VMCSE.Attacks.DevilSword;
using GlobalEnums;

namespace VMCSE.CrestManagement
{
    public class DevilCrest : CustomCrest
    {
        private bool DevilswordDashAttack = false;
        private bool wasSprinting = false;
        public override void SetupDashFSM()
        {
            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintfsm == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            FsmEvent DEVILEVENT = sprintfsm.CreateFsmEvent("DEVIL");

            FsmState startAttackState = sprintfsm.GetState("Start Attack");
            startAttackState.AddAction(new CheckIfCrestEquipped { Crest = toolCrest, trueEvent = DEVILEVENT });

            FsmState testSetState = sprintfsm.AddState("Set Devil");
            testSetState = sprintfsm.CopyState("Set Attack Single", "Set Devil");
            SetGameObject action = (SetGameObject)testSetState.Actions[3];
            action.gameObject = slashDashObject;


            //modification to on hit so we dont bounce
            testSetState.AddMethod(() =>
            {
                DevilswordDashAttack = true;
            });

            FsmState HitEnemyNoBounce = sprintfsm.AddState("Hit Enemy No Bounce");
            HitEnemyNoBounce.AddMethod(() =>
            {
                slashDashObject.GetComponent<DashStabNailAttack>().OnCancelAttack();
                //HeroController.instance.CancelAttack();
            });

            FsmState ReactionType = sprintfsm.GetState("Reaction Type");
            ReactionType.AddMethod(() => { if (DevilswordDashAttack) { DevilswordDashAttack = false;  sprintfsm.SendEvent("DEVILSWORD"); } });

            FsmState RegainControlNormal = sprintfsm.GetState("Regain Control Normal");
            RegainControlNormal.AddMethod(() => { DevilswordDashAttack = false; });

            HitEnemyNoBounce.AddTransition("FINISHED", "Regain Control Normal");
            //ReactionType.AddTransition("DEVILSWORD", HitEnemyNoBounce.name);

            sprintfsm.AddTransition("Start Attack", "DEVIL", "Set Devil");

        }

        public override void SetupCrestAttackFSM()
        {
            PlayMakerFSM crestAttackFSM = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (crestAttackFSM == null) { VMCSE.Instance.LogError("VMCSE - Crest Attacks not found");return; }

            DevilSwordDownslash downslashAttack = new DevilSwordDownslash(slashDownObject, crestAttackFSM);
            downslashAttack.CreateAttack();
        }

        public override void SetupNailArtFSM()
        {
            PlayMakerFSM nailartFSM = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (nailartFSM == null) { VMCSE.Instance.LogError("VMCSE - Nail Arts not found"); return; }

            ReviveNailArts(nailartFSM);

            CreateHighTime(nailartFSM);

            FsmState DashSlashChoiceState = nailartFSM.GetState("Dash Slash Choice");
            DashSlashChoiceState.AddMethod(() =>
            {
                bool devilswordEquipped = true;
                if (devilswordEquipped)
                {
                    nailartFSM.SendEvent("DEVILSWORD");
                }
            });

            GameObject lungeStopper = CrestManager.devilswordMillionStinger.Child("Lunge Stopper");
            FsmOwnerDefault lungeStopperOwnerDefault = new FsmOwnerDefault() { gameObject = lungeStopper, ownerOption = OwnerDefaultOption.SpecifyGameObject};

            FsmState AnticTypeState = nailartFSM.GetState("Antic Type");
            AnticTypeState.AddMethod(() =>
            {
                if (HeroController.instance.playerData.CurrentCrestID == "Devil")
                {
                    nailartFSM.SendEvent("DEVIL");
                }
            });

            FsmState MillionStabAnticState = nailartFSM.AddState("Millionstab Antic");
            MillionStabAnticState.CopyActionData(nailartFSM.GetState("Wanderer Antic"));

            FsmState MillionStabState = nailartFSM.AddState("Millionstab");
            MillionStabState.CopyActionData(nailartFSM.GetState("Wanderer Attack"));
            MillionStabState.GetAction<Trigger2dEvent>(9).gameObject = lungeStopperOwnerDefault;
            MillionStabState.GetAction<Trigger2dEvent>(10).gameObject = lungeStopperOwnerDefault;
            MillionStabState.RemoveAction(7); //activate gameobject
            MillionStabState.AddMethod(() => { CrestManager.devilswordMillionStinger.SetActive(true); });

            FsmState MillionStabDmgBrake = nailartFSM.AddState("Millionstab Dmg Brake");
            MillionStabDmgBrake.CopyActionData(nailartFSM.GetState("Wanderer DmgBrake"));


            //transitions
            DashSlashChoiceState.AddTransition("DEVILSWORD", MillionStabAnticState.name);
            MillionStabAnticState.AddTransition("FINISHED", MillionStabState.name);
            MillionStabState.AddTransition("FINISHED", "Set Finished");
            MillionStabState.AddTransition("ATTACK LANDED", MillionStabDmgBrake.name);
            MillionStabDmgBrake.AddTransition("FINISHED", "Set Finished");

        }

        private void CreateHighTime(PlayMakerFSM nailartFSM)
        {
            /*//dealt damage fsm
            PlayMakerFSM damageDealtFSM = CrestManager.devilswordHighTime.LocateMyFSM("Damage Dealt");
            if (damageDealtFSM == null)
            {
                damageDealtFSM = CrestManager.devilswordHighTime.AddComponent<PlayMakerFSM>();
                damageDealtFSM.GetComponent<DamageEnemies>().dealtDamageFSM = damageDealtFSM;
                damageDealtFSM.GetComponent<DamageEnemies>().targetRecordedFSMEvent = "ATTACK LANDED";
            }

            FsmState WaitingState = damageDealtFSM.AddState("Waiting");
            FsmState SendEventState = damageDealtFSM.AddState("Send Event");
            SendEventState.AddMethod(() =>
            {
                nailartFSM.SendEvent("ATTACK LANDED");
            });

            //transitions
            WaitingState.AddTransition("ATTACK LANDED", SendEventState.name);
            SendEventState.AddTransition("FINISHED", WaitingState.name);
            damageDealtFSM.SetState(WaitingState.name);*/


            FsmState CycloneSlashChoiceState = nailartFSM.GetState("Cyclone Slash Choice");
            CycloneSlashChoiceState.AddMethod(() =>
            {
                bool devilswordEquipped = true;
                if (devilswordEquipped)
                {
                    nailartFSM.SendEvent("DEVILSWORD");
                }
            });

            GameObject lungeStopper = CrestManager.devilswordHighTime.Child("Lunge Stopper");
            FsmOwnerDefault lungeStopperOwnerDefault = new FsmOwnerDefault() { gameObject = lungeStopper, ownerOption = OwnerDefaultOption.SpecifyGameObject };



            FsmState HighTimeAnticState = nailartFSM.AddState("High Time Antic");
            HighTimeAnticState.CopyActionData(nailartFSM.GetState("Wanderer Antic"));

            FsmState HighTimeState = nailartFSM.AddState("High Time");
            HighTimeState.CopyActionData(nailartFSM.GetState("Wanderer Attack"));
            HighTimeState.GetAction<Trigger2dEvent>(9).gameObject = lungeStopperOwnerDefault;
            HighTimeState.GetAction<Trigger2dEvent>(10).gameObject = lungeStopperOwnerDefault;
            HighTimeState.RemoveAction(7); //activate gameobject
            HighTimeState.RemoveAction(5); //accelerate to y
            HighTimeState.AddMethod(() => { CrestManager.devilswordHighTime.SetActive(true); });
            HighTimeState.RemoveAction(2); //set velocity to -45f x
            HighTimeState.GetAction<SetVelocity2d>().y = 30f;
            HighTimeState.GetAction<DecelerateXY>().decelerationX = 0f;
            HighTimeState.GetAction<DecelerateXY>().decelerationY = 0.9f;
            HighTimeState.GetAction<PlayAudioEvent>().audioClip = nailartFSM.GetState("Warrior2 Leap").GetAction<PlayAudioEvent>().audioClip;

            FsmState HighTimeBounceState = nailartFSM.AddState("High Time Bounce");
            HighTimeBounceState.AddMethod(() =>
            {
                HeroController.instance.SetStartWithHarpoonBounce();
                HeroController.instance.RegainControl();
                HeroController.instance.AffectedByGravity(true);
                HeroController.instance.StartDownspikeInvulnerability();
                //HeroController.instance.DownspikeBounce(false);
                
            });

            //transitions
            CycloneSlashChoiceState.AddTransition("DEVILSWORD", HighTimeAnticState.name);
            HighTimeAnticState.AddTransition("FINISHED", HighTimeState.name);
            HighTimeState.AddTransition("FINISHED", "Set Finished");
            HighTimeState.AddTransition("ATTACK LANDED", HighTimeBounceState.name);
            HighTimeBounceState.AddTransition("FINISHED", "Regain Full Control");
        }

        private void ReviveNailArts(PlayMakerFSM nailartFSM)
        {
            FsmState TakeControlState = nailartFSM.GetState("Can Nail Art?");
            TakeControlState.AddMethod(() =>
            {
                wasSprinting = HeroController.instance.cState.isSprinting;
            });

            FsmEventTarget nailartTarget = new FsmEventTarget() { target = FsmEventTarget.EventTarget.FSMComponent, fsmComponent = nailartFSM };

            FsmState AnticTypeState = nailartFSM.GetState("Antic Type");
            AnticTypeState.AddMethod(() =>
            {
                if (HeroController.instance.playerData.CurrentCrestID == "Devil")
                {
                    nailartFSM.SendEvent("DEVIL");
                }
            });

            FsmState SprintCheckState = nailartFSM.AddState("Sprinting?");
            SprintCheckState.AddMethod(() =>
            {
                if (wasSprinting || nailartFSM.GetFsmBoolIfExists("dashing") || GameManager.instance.inputHandler.inputActions.Dash.IsPressed)
                {
                    nailartFSM.SendEvent("CYCLONESLASH");
                }
            });


            FsmState CycloneCheckState = nailartFSM.AddState("Holding Up?");
            CycloneCheckState.AddMethod(() =>
            {
                if (GameManager.instance.inputHandler.inputActions.Up.IsPressed || GameManager.instance.inputHandler.inputActions.Down.IsPressed)
                {
                    nailartFSM.SendEvent("DASHSLASH");
                }
            });
            

            FsmState GreatSlashChoiceState = nailartFSM.AddState("Great Slash Choice");
            FsmState CycloneSlashChoiceState = nailartFSM.AddState("Cyclone Slash Choice");
            FsmState DashSlashChoiceState = nailartFSM.AddState("Dash Slash Choice");

            //transitions
            AnticTypeState.AddTransition("DEVIL", SprintCheckState.name);
            SprintCheckState.AddTransition("CYCLONESLASH", CycloneSlashChoiceState.name);
            SprintCheckState.AddTransition("FINISHED", CycloneCheckState.name);
            CycloneCheckState.AddTransition("DASHSLASH", DashSlashChoiceState.name);
            CycloneCheckState.AddTransition("FINISHED", GreatSlashChoiceState.name);
            GreatSlashChoiceState.AddTransition("FINISHED", "Antic");
            CycloneSlashChoiceState.AddTransition("FINISHED", "Antic");
            DashSlashChoiceState.AddTransition("FINISHED", "Antic");
        }
    }
}
