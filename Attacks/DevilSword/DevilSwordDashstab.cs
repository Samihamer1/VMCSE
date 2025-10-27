using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.Slashes;
using VMCSE.Components;

namespace VMCSE.Attacks.DevilSword
{
    public class DevilSwordDashstab : BaseAttack
    {
        private GameObject dashstabObject;
        private GameObject? redslashObject;
        private GameObject? GiantBurstObject;
        
        public DevilSwordDashstab(GameObject dashStabObj)
        {
            this.dashstabObject = dashStabObj;            
        }

        private void RedSlash(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
        {
            if (clip.GetFrame(frame).eventInfo != "RedSlash") { return; }
            DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
            if (handler == null) { VMCSE.Instance.LogError("DevilCrestHandler not found. Can't create DevilSword Dashstab"); return; }
            
            if (!handler.ConsumeChaserBlade()) { return; }
            //activate slash
            redslashObject.GetComponent<NailSlash>().StartSlash();
            redslashObject.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;
        }

        private void CreateRedSlash()
        {
            //Regular slash
            GameObject redCopy = new GameObject(AttackNames.REDSLASH);
            redCopy.transform.parent = dashstabObject.transform;
            redCopy.transform.localPosition = new Vector3(0, 0.3f, 0);
            StandardSlash slash1 = redCopy.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(-1.2f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetDevilSwordAnimator(), "DashStabEffect");
            slash1.SetNailDamageMultiplier(0.33f);
            slash1.SetColliderSize(dashstabObject.GetComponent<PolygonCollider2D>().points);
            slash1.SetStunDamage(0.2f);
            slash1.SetMagnitude(0f);
            

            dashstabObject.GetComponent<DashStabNailAttack>().AttackStarting += RedSlash;

            redslashObject = redCopy;
        }

        private void RedSlash()
        {
            redslashObject.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;
        }

        private void CreateEffects()
        {
            GameObject SpecialAttacks = HeroController.instance.gameObject.Child("Special Attacks");
            if (SpecialAttacks == null) { VMCSE.Instance.LogError("Special Attacks gameobject not found. Incompatibility with other mods?"); return;  }

            GameObject SilkChargeBurst = SpecialAttacks.Child("Silk Charge DashBurst");
            if (SilkChargeBurst == null) { return; }

            GiantBurstObject = GameObject.Instantiate(SilkChargeBurst);
            GiantBurstObject.name = "Red Charge Burst";
            GiantBurstObject.transform.parent = SpecialAttacks.transform;
            GiantBurstObject.transform.localPosition = new Vector3(-0.38f, 0, 0);
            GiantBurstObject.GetComponentInChildren<SpriteRenderer>().color = ColorConstants.DanteRed;
        }

        public override void CreateAttack()
        {

            CreateRedSlash();
            CreateEffects();

            dashstabObject.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggeredEvent += RedSlash;

            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintfsm == null) { VMCSE.Instance.LogError("Sprint FSM not found."); return; }

            DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
            if (handler == null) { VMCSE.Instance.LogError("DevilCrestHandler not found. Can't create DevilSword Dashstab"); return; };


            #region Start Attack state
            FsmState? SetAttackSingleState = sprintfsm.GetState("Set Attack Single");
            if (SetAttackSingleState == null) { VMCSE.Instance.LogError("Set Attack Single state not found in SprintFSM. Incompatibility with other mod?"); return; }
            FsmGameObject objectVariable = sprintfsm.GetGameObjectVariable("Dash Stab Crt");

            SetAttackSingleState.AddMethod(_ =>
            {
                if (!handler.IsDevilEquipped()) { return; } 
                if (!handler.isWeaponEquipped(WeaponNames.DEVILSWORD)) { return; }

                objectVariable.value = dashstabObject;
                
                //objectVariable.value = redslashObject;
            });
            #endregion

            #region Attack Dash state
            FsmState? attackDashState = sprintfsm.GetState("Attack Dash");
            if (attackDashState == null) { VMCSE.Instance.LogError("Attack Dash state not found in SprintFSM. Incompatibility with other mod?"); return; }

            attackDashState.AddMethod(_ =>
            {
                //When in trigger, use the cool red burst effect
                if (!handler.IsDevilEquipped()) { return; }
                if (!handler.isWeaponEquipped(WeaponNames.DEVILSWORD)) { return; }
                if (!handler.GetTrigger().IsInTrigger()) { return; }

                if (GiantBurstObject == null) { return; }
                GiantBurstObject.SetActive(true);
            });
            #endregion
        }
    }
}
