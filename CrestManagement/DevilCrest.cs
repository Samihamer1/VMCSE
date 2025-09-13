using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;


namespace VMCSE.CrestManagement
{
    public class DevilCrest : CustomCrest
    {
        //Just a copy for later.
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

            sprintfsm.AddTransition("Start Attack", "DEVIL", "Set Devil");
        }
    }
}
