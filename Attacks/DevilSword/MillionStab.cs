using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Components;
using VMCSE.Weapons;

namespace VMCSE.Attacks.DevilSword
{
    public class MillionStab : BaseNailArt
    {
        private GameObject millionStab;
        private GameObject millionStabRed;
        public MillionStab() : base()
        {
            name = "MillionStab";
        }

        private GameObject CreateStinger(string name, float damage)
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            //Million stinger
            GameObject millionStinger = UnityEngine.Object.Instantiate(attacks.Child("Charge Slash Wanderer"));
            millionStinger.transform.parent = CrestManager.devilroot.transform;
            millionStinger.transform.localScale = new Vector3(0.75f, 0.7f, 1f);
            millionStinger.transform.localPosition = new Vector3(-1.2f, 0.6f, 0);
            millionStinger.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.4f;
            millionStinger.name = name;

            return millionStinger;
        }        

        private GameObject CreateRedStinger()
        {
            GameObject stinger = CreateStinger(AttackNames.REDSLASH, 0.1f);
            stinger.transform.parent = millionStab.transform;
            stinger.transform.localScale = new Vector3(1f, 0.7f, 1f);
            stinger.transform.localPosition = new Vector3(0,0, -0.01f);

            stinger.Child("Charge Slash Hornet Voice").SetActive(false);

            stinger.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;
            stinger.GetComponent<tk2dSpriteAnimator>().currentClip = AnimationManager.GetDevilSwordAnimator().GetClipByName("MillionStabFast");

            stinger.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.3f;
            stinger.GetComponent<DamageEnemies>().stepsPerHit *= 2;
            stinger.GetComponent<DamageEnemies>().minimalHitEffects = true;
            stinger.GetComponent<DamageEnemies>().multiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Minimal;
            stinger.GetComponent<DamageEnemies>().SetSilkGenerationNone();

            return stinger;
        }

        public override void CreateAttack()
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            millionStab = CreateStinger(AttackNames.MILLIONSTAB, 0.35f);
            millionStabRed = CreateRedStinger();

            PlayMakerFSM nailartFSM = fsm;


            GameObject lungeStopper = millionStab.Child("Lunge Stopper");
            FsmOwnerDefault lungeStopperOwnerDefault = new FsmOwnerDefault() { gameObject = lungeStopper, ownerOption = OwnerDefaultOption.SpecifyGameObject };

            FsmState AnticTypeState = nailartFSM.GetState("Antic Type");
            AnticTypeState.AddMethod(_ =>
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
            MillionStabState.AddMethod(_ => {
                millionStab.SetActive(true);
                millionStab.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.35f;

                //checking for handler
                DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
                if (handler == null) { return; }
                if (!handler.ConsumeChaserBlade()) { return; }

                millionStabRed.SetActive(true);
                millionStabRed.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;

            });

            FsmState MillionStabDmgBrake = nailartFSM.AddState("Millionstab Dmg Brake");
            MillionStabDmgBrake.CopyActionData(nailartFSM.GetState("Wanderer DmgBrake"));

            //adding cancellation of attack when hit
            fsm.AddMethod("Cancel All", _ =>
            {
                millionStab.SetActive(false);
            });


            //transitions
            MillionStabAnticState.AddTransition("FINISHED", MillionStabState.name);
            MillionStabState.AddTransition("FINISHED", "Set Finished");
            MillionStabState.AddTransition("ATTACK LANDED", MillionStabDmgBrake.name);
            MillionStabDmgBrake.AddTransition("FINISHED", "Set Finished");

            SetStateAsInit(MillionStabAnticState.name);
        }

    }
}
