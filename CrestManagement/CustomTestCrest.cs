using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace VMCSE.CrestManagement
{
    public class CustomTestCrest : CustomCrest
    {
        public override void SetupDashFSM()
        {
            //This is specifically to copy Hunter's dash attack. It'll be different for the others, I just haven't tested those yet.
            //Of course, it can also be custom.
            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintfsm == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            FsmEvent TESTEVENT = sprintfsm.CreateFsmEvent("TEST");

            FsmState startAttackState = sprintfsm.GetState("Start Attack");
            startAttackState.AddAction(new CheckIfCrestEquipped { Crest = toolCrest, trueEvent = TESTEVENT });

            FsmState testSetState = sprintfsm.AddState("Set Test");
            testSetState = sprintfsm.CopyState("Set Attack Single", "Set Test");
            SetGameObject action = (SetGameObject)testSetState.Actions[3];
            action.gameObject = slashDashObject;

            sprintfsm.AddTransition("Start Attack", "TEST", "Set Test");
        }
    }
}
