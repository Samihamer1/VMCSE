using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using VMCSE.Weapons;

namespace VMCSE.Attacks
{
    public abstract class BaseNailArt : BaseAttack
    {
        public NailArtType naType;
        public string PreinitialState
        {
            get
            {
                switch (naType)
                {
                    case NailArtType.CycloneSlash: return StateNames.CYCLONESLASHCHOICE;
                    case NailArtType.DashSlash: return StateNames.DASHSLASHCHOICE;
                    case NailArtType.GreatSlash: return StateNames.GREATSLASHCHOICE;
                    default: VMCSE.Instance.LogError("NA Type of " + name + " not defined"); return "";
                }
            }
        }
        
        public WeaponBase? weapon;

        public BaseNailArt() {
            PlayMakerFSM nailartFSM = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (nailartFSM == null) { VMCSE.Instance.LogError("VMCSE - Nail Arts not found"); return; }
            fsm = nailartFSM;
            
        }

        public void SetStateAsInit(string stateName)
        {
            FsmState PreInitialChoiceState = fsm.GetState(PreinitialState);
            if (PreInitialChoiceState == null)
            {
                VMCSE.Instance.LogError(name + " preinitial state '" + PreinitialState + "' not found.");
                return;
            }

            PreInitialChoiceState.AddTransition(weapon.GetWeaponEvent(), stateName);
        }


        public void SetNAType(NailArtType type)
        {
            this.naType = type;
        }
        public void SetWeapon(WeaponBase weapon)
        {
            this.weapon = weapon;
        }
    }

    public enum NailArtType
    {
        GreatSlash,
        DashSlash,
        CycloneSlash
    }
}
