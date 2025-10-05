using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolGameObjectActivator;

namespace VMCSE.Attacks.DevilSword
{
    public class DevilSwordDashstab : BaseAttack
    {
        private GameObject dashstabObject;

        public DevilSwordDashstab(GameObject downslashObject)
        {
            this.dashstabObject = downslashObject;
        }
        public override void CreateAttack()
        {
            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintfsm == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            FsmEvent DEVILEVENT = sprintfsm.CreateFsmEvent("DEVIL");

            FsmState startAttackState = sprintfsm.GetState("Start Attack");
            startAttackState.AddMethod(() =>
            {
                if (HeroController.instance.playerData.CurrentCrestID == "Devil")
                {
                    sprintfsm.SendEvent("DEVIL");
                }
            });

            FsmState testSetState = sprintfsm.AddState("Set Devil");
            testSetState = sprintfsm.CopyState("Set Attack Single", "Set Devil");
            SetGameObject action = (SetGameObject)testSetState.Actions[3];
            action.gameObject = dashstabObject;


           

            sprintfsm.AddTransition("Start Attack", "DEVIL", "Set Devil");
        }
    }
}
