using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMCSE.Attacks
{
    public abstract class BaseAttack
    {
        public PlayMakerFSM fsm;
        public string name;

        public abstract void CreateAttack();
        
    }
}
