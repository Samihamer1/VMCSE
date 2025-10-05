using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.Attacks.DevilSword;
using VMCSE.Weapons;

namespace VMCSE.Components
{
    public class DevilCrestHandler : MonoBehaviour
    {
        private List<WeaponBase> allWeapons = new List<WeaponBase>();

        private List<WeaponBase> equippedWeapons = new List<WeaponBase>();
        private WeaponBase equippedWeapon;
        private StyleMeter styleMeter;

        private bool airCharge = true; //used for reactor bounce

        public void Start()
        {
            CreateWeapons();
            CreateStyle();
        }

        private void CreateStyle()
        {
            styleMeter = HeroController.instance.gameObject.AddComponent<StyleMeter>();
        }

        private void CreateWeapons()
        {
            DevilSwordWeapon devilSword = new DevilSwordWeapon();
            allWeapons.Add(devilSword);

            EquipWeapon(devilSword);
        }

        private void EquipWeapon(WeaponBase weapon)
        {
            HeroControllerConfig config = CrestManager.devilCrest.heroControllerConfig;
            HeroController.ConfigGroup group = weapon.GetConfigGroup();
            group.Config = config;
            group.Setup();


            for (int i = 0; i < HeroController.instance.configs.Length; i++)
            {
                HeroController.ConfigGroup stored = HeroController.instance.configs[i];
                if (stored.Config.name != "Devil") { continue; }
                HeroController.instance.configs[i] = group;
            }

            HeroController.instance.SetConfigGroup(group, group);

            CrestManager.devilroot.SetActive(true);

            equippedWeapon = weapon;
        }

        public WeaponBase getEquippedWeapon()
        {
            return equippedWeapon;
        }

        public bool isWeaponEquipped(WeaponBase weapon)
        {
            return equippedWeapon == weapon;
        }

        public void Update()
        {
            if (InputHandler.Instance.inputActions.QuickCast.IsPressed)
            {
                if (InputHandler.Instance.inputActions.Up.IsPressed)
                {
                    equippedWeapon.UpSpell();
                }
                if (InputHandler.Instance.inputActions.Down.IsPressed)
                {
                    equippedWeapon.DownSpell();
                }
                if (InputHandler.Instance.inputActions.Left.IsPressed || InputHandler.Instance.inputActions.Right.IsPressed)
                {
                    equippedWeapon.HorizontalSpell();
                }

                //hit the bigmoney
            }

            if (HeroController.instance == null)
            {
                return;
            }

            if ((HeroController.instance.cState.onGround || HeroController.instance.cState.touchingWall) && !airCharge)
            {
                airCharge = true;
            }
        }

        public bool UseAirCharge()
        {
            if (airCharge)
            {
                airCharge = false;
                return true;
            }
            return false;
        }

        public void HitLanded(HitInstance instance)
        {
            styleMeter.HitLanded(instance);
        }

        public void GotHit()
        {
            styleMeter.LargeLoss();
        }
    }
}
