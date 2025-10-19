using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using UnityEngine;
using VMCSE.Attacks.DevilSword;
using GlobalEnums;
using VMCSE.Weapons;
using VMCSE.Components;
using System;

namespace VMCSE.CrestManagement
{
    public class DevilCrest : CustomCrest
    {
        private bool wasSprinting = false;

        public void SetupWeapon()
        {
            PlayMakerFSM nailartFSM = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (nailartFSM == null) { VMCSE.Instance.LogError("VMCSE - Nail Arts not found"); return; }

            PlayMakerFSM crestAttackFSM = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (crestAttackFSM == null) { VMCSE.Instance.LogError("VMCSE - Crest Attacks not found"); return; }

            PlayMakerFSM sprintFSM = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintFSM == null) { VMCSE.Instance.LogError("VMCSE - Sprint not found."); return; }

            ReviveNailArts(nailartFSM);


            HeroController.instance.gameObject.AddComponent<DevilCrestHandler>();
        }

        private void ReviveNailArts(PlayMakerFSM nailartFSM)
        {
            FsmState TakeControlState = nailartFSM.GetState("Can Nail Art?");
            TakeControlState.AddMethod(_ =>
            {
                wasSprinting = HeroController.instance.cState.isSprinting;
            });

            FsmEventTarget nailartTarget = new FsmEventTarget() { target = FsmEventTarget.EventTarget.FSMComponent, fsmComponent = nailartFSM };

            FsmState AnticTypeState = nailartFSM.GetState("Antic Type");
            AnticTypeState.AddMethod(_ =>
            {
                if (HeroController.instance.playerData.CurrentCrestID == "Devil")
                {
                    nailartFSM.SendEvent("DEVIL");
                }
            });

            FsmState SprintCheckState = nailartFSM.AddState("Sprinting?");
            SprintCheckState.AddMethod(_ =>
            {
                if (wasSprinting || nailartFSM.GetFsmBoolIfExists("dashing") || GameManager.instance.inputHandler.inputActions.Dash.IsPressed)
                {
                    nailartFSM.SendEvent("DASHSLASH");
                }
            });


            FsmState CycloneCheckState = nailartFSM.AddState("Holding Up?");
            CycloneCheckState.AddMethod(_ =>
            {
                if (GameManager.instance.inputHandler.inputActions.Up.IsPressed || GameManager.instance.inputHandler.inputActions.Down.IsPressed)
                {
                    nailartFSM.SendEvent("CYCLONESLASH");
                }
            });
            

            FsmState GreatSlashChoiceState = nailartFSM.AddState("Great Slash Choice");
            GreatSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });
            FsmState CycloneSlashChoiceState = nailartFSM.AddState("Cyclone Slash Choice");
            CycloneSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });
            FsmState DashSlashChoiceState = nailartFSM.AddState("Dash Slash Choice");
            DashSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });

            //transitions
            AnticTypeState.AddTransition("DEVIL", SprintCheckState.name);
            SprintCheckState.AddTransition("DASHSLASH", DashSlashChoiceState.name);
            SprintCheckState.AddTransition("FINISHED", CycloneCheckState.name);
            CycloneCheckState.AddTransition("CYCLONESLASH", CycloneSlashChoiceState.name);
            CycloneCheckState.AddTransition("FINISHED", GreatSlashChoiceState.name);
            GreatSlashChoiceState.AddTransition("FINISHED", "Antic");
            CycloneSlashChoiceState.AddTransition("FINISHED", "Antic");
            DashSlashChoiceState.AddTransition("FINISHED", "Antic");
        }

        private void SendWeaponEvent(PlayMakerFSM playmakerFSM)
        {
            if (HeroController.instance == null) { return; }
            DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            playmakerFSM.SendEvent(handler.getEquippedWeapon().weaponEvent);
        }
    }
}
