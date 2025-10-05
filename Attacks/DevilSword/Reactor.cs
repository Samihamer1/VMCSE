using HutongGames.PlayMaker;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class Reactor : BaseSpell
    {
        public Reactor(DevilCrestHandler handler) : base(handler)
        {
            this.EVENTNAME = "REACTOR";
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState ReactorCheckState = fsm.AddState("Reactor Check");
            ReactorCheckState.AddMethod(() =>
            {
                bool charge = handler.UseAirCharge();
                if (charge)
                {
                    fsm.SendEvent("USE");
                }
            });

            FsmState ReactorBounceState = fsm.AddState("Reactor Bounce");
            ReactorBounceState.AddMethod(() =>
            {
                HeroController.instance.DownspikeBounce(false);
            });

            //Transition
            ReactorCheckState.AddTransition("FINISHED", "Special End");
            ReactorCheckState.AddTransition("USE", ReactorBounceState.name);
            ReactorBounceState.AddTransition("FINISHED", "Special End");

            SetStateAsInit(ReactorCheckState.name);
        }
    }
}
