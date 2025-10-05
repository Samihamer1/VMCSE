using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMCSE.Attacks
{
    public abstract class BaseAttack
    {
        public PlayMakerFSM fsm;

        public abstract void CreateAttack();
        
    }
}
