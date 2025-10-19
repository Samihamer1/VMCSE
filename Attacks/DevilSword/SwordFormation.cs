using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class SwordFormation : BaseNailArt
    {
        public SwordFormation() : base()
        {
            name = "SwordFormation";
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState FormState = fsm.AddState("Sword Formation");
            FormState.AddMethod(_ =>
            {
                DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
                if (handler == null) { VMCSE.Instance.LogError("DevilCrestHandler not found!"); return; }
                handler.RefreshChaserBlades();
            });

            FormState.AddTransition("FINISHED", "Regain Full Control");

            SetStateAsInit(FormState.name);
        }
    }
}
