using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using VMCSE.Components;

namespace VMCSE.Attacks
{
    public abstract class BaseSpell : BaseAttack
    {
        public DevilCrestHandler handler;
        public string EVENTNAME;
        public BaseSpell(DevilCrestHandler handler) {
            if (HeroController.instance == null) { return; }
            PlayMakerFSM spellfsm = HeroController.instance.gameObject.LocateMyFSM("Silk Specials");
            fsm = spellfsm;
            

            this.handler = handler;
        }

        public void SetStateAsInit(string statename)
        {
            fsm.AddGlobalTransition(EVENTNAME, statename);
        }
    }
}
