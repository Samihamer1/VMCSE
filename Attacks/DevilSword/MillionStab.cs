using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using UnityEngine;
using VMCSE.Weapons;

namespace VMCSE.Attacks.DevilSword
{
    public class MillionStab : BaseNailArt
    {
        public MillionStab() : base()
        {
            name = "MillionStab";
        }

        public override void CreateAttack()
        {
            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");

            //Million stinger
            GameObject millionStinger = UnityEngine.Object.Instantiate(attacks.Child("Charge Slash Wanderer"));
            millionStinger.transform.parent = CrestManager.devilroot.transform;
            millionStinger.transform.localScale = new Vector3(1, 0.7f, 1f);
            millionStinger.transform.localPosition = new Vector3(-1.2f, 0.6f, 0);
            millionStinger.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.4f;

            PlayMakerFSM nailartFSM = fsm;


            GameObject lungeStopper = millionStinger.Child("Lunge Stopper");
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
            MillionStabState.AddMethod(_ => { millionStinger.SetActive(true); });

            FsmState MillionStabDmgBrake = nailartFSM.AddState("Millionstab Dmg Brake");
            MillionStabDmgBrake.CopyActionData(nailartFSM.GetState("Wanderer DmgBrake"));


            //transitions
            MillionStabAnticState.AddTransition("FINISHED", MillionStabState.name);
            MillionStabState.AddTransition("FINISHED", "Set Finished");
            MillionStabState.AddTransition("ATTACK LANDED", MillionStabDmgBrake.name);
            MillionStabDmgBrake.AddTransition("FINISHED", "Set Finished");

            SetStateAsInit(MillionStabAnticState.name);
        }

    }
}
