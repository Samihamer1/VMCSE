using GlobalSettings;
using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using VMCSE.Components;

namespace VMCSE.Attacks.KingCerberus
{
    internal class CerberusDashStab : BaseDashStab
    {
        private GameObject dashStabObject;
        private PlayParticleEffects flameParticles;
        public CerberusDashStab(DevilCrestHandler crestHandler, GameObject slashObject) : base(crestHandler)
        {
            dashStabObject = slashObject;

            DashStabNailAttack dashstab = dashStabObject.GetComponent<DashStabNailAttack>();
            if (dashstab == null) { return; }
            dashstab.AttackStarting += ActivateFlame;
            dashstab.EndedDamage += DeactivateFlame;

        }

        private SpriteFlash GetSpriteFlash()
        {
            return HeroController.instance.gameObject.GetComponent<SpriteFlash>();
        }

        private NestedFadeGroupBase? GetLightGroup()
        {
            GameObject light = HeroController.instance.gameObject.Child("HeroLight");
            if (light == null) { return null; }

            GameObject imbue = light.Child("Imbued Hero Light");
            if (imbue == null) { return null; }
            return imbue.GetComponent<NestedFadeGroupBase>();
        }

        private void DeactivateFlame(bool obj)
        {
            if (flameParticles != null)
            {
                flameParticles.StopParticleSystems();
                //GameObject.Destroy(flameParticles);
            }

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToZero(0.2f);
        }

        private void ActivateFlame()
        {
            //Color
            NailImbuementConfig config = HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire];

            dashStabObject.GetComponent<DashStabNailAttack>().SetNailImbuement(config, NailElements.Fire);


            //audio
            AudioSource audio = HeroController.instance.gameObject.GetComponent<AudioSource>();
            if (audio == null) { return; }
            //audio.PlayOneShot(HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire].ExtraSlashAudio.Clip);

            //Effects
            SpriteFlash flash = GetSpriteFlash();
            if (flash != null)
            {
                //flash.flashFocusHeal();
            }

            flameParticles = config.HeroParticles.Spawn(HeroController.instance.transform.position);
            flameParticles.PlayParticleSystems();

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToOne(0.2f);
            HeroController.instance.NailImbuement.imbuedHeroLightRenderer.color = config.ExtraHeroLightColor;

        }

        public override void CreateAttack()
        {
            if (fsm == null) { VMCSE.Instance.LogError("Sprint FSM not found when creating CerberusDashStab"); return; }

            #region Start Attack state
            FsmState? SetAttackSingleState = fsm.GetState("Set Attack Single");
            if (SetAttackSingleState == null) { VMCSE.Instance.LogError("Set Attack Single state not found in SprintFSM. Incompatibility with other mod?"); return; }
            FsmGameObject objectVariable = fsm.GetGameObjectVariable("Dash Stab Crt");

            SetAttackSingleState.AddMethod(_ =>
            {
                if (!handler.IsDevilEquipped()) { return; }
                if (!handler.isWeaponEquipped(WeaponNames.KINGCERBERUS)) { return; }

                objectVariable.value = dashStabObject;
                //ActivateFlame();
                //objectVariable.value = redslashObject;
            });
            #endregion
        }
    }
}
