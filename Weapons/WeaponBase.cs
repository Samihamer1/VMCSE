using System;
using System.Collections.Generic;
using System.Text;
using VMCSE.Attacks;
using VMCSE.Components;
using VMCSE.CrestManagement;

namespace VMCSE.Weapons
{
    public class WeaponBase
    {
        public string? name;
        public string? weaponEvent { get; set; }

        public BaseNailArt? cycloneSlash { get; set; }
        public BaseNailArt? greatSlash { get; set; }
        public BaseNailArt? dashSlash { get; set; }
        public BaseSpell? upSpell { get; set; }
        public BaseSpell? downSpell { get; set; }
        public BaseSpell? horizontalSpell { get; set; }

        public BaseAttack? specialDownSlash { get; set; }
        public BaseAttack? specialDashStab { get; set; }
        public CrestObjectInfo? objectInfo;
        internal HeroController.ConfigGroup configGroup;
        public DevilCrestHandler handler;

        internal WeaponConfig config;

        public WeaponBase()
        {
            if (HeroController.instance == null) { return; }
            handler = HeroController.instance.GetComponent<DevilCrestHandler>();


            ToolCrestList currentList = ToolItemManager.Instance.crestList;
            ToolCrest reaper = currentList.GetByName("Reaper");
            HeroControllerConfig reaperConfig = reaper.HeroConfig;


            //double test

            config = new WeaponConfig(UnityEngine.GameObject.Instantiate(reaperConfig));
        }

        public bool isEquipped()
        {
            return handler.isWeaponEquipped(this);
        }

        public void Initialise()
        {

            InitNailArt(cycloneSlash, NailArtType.CycloneSlash);
            InitNailArt(greatSlash, NailArtType.GreatSlash);
            InitNailArt(dashSlash, NailArtType.DashSlash);

            InitSpell(upSpell);
            InitSpell(downSpell);
            InitSpell(horizontalSpell);

            InitAttack(specialDownSlash);
            InitAttack(specialDashStab);
        }

        private void InitSpell(BaseSpell? spell)
        {
            if (spell == null) { return; }
            spell.CreateAttack();
        }

        private void Spell(BaseSpell? spell)
        {
            if (HeroController.instance == null) { return; }
            if (spell == null) { return; }
            PlayMakerFSM spellfsm = HeroController.instance.gameObject.LocateMyFSM("Silk Specials");

            if (!spell.OnCooldown() && !spell.OnManualCooldown())
            {
                //You could hold me at gunpoint, and I wouldn't be able to explain why this line of code is required.
                //Just know that if you use SendEvent like a normal person, it doesn't recognise the global transition.
                //But only the SECOND time the game loads.
                spellfsm.ChangeState(spell.GetGlobalEvent());
            }
        }

        public void UpSpell()
        {
            Spell(upSpell);
        }

        public void HorizontalSpell()
        {
            Spell(horizontalSpell);
        }

        public void DownSpell()
        {
            Spell(downSpell);
        }

        public HeroController.ConfigGroup GetConfigGroup()
        {
            return configGroup;
        }

        private void InitNailArt(BaseNailArt? nailArt, NailArtType type)
        {
            if (nailArt == null) { return; }
            nailArt.SetWeapon(this);
            nailArt.SetNAType(type);
            nailArt.CreateAttack();
        }

        private void InitAttack(BaseAttack? attack)
        {
            if (attack == null) { return; }
            attack.CreateAttack();
        }

        public string GetWeaponEvent()
        {
            if (weaponEvent == null)
            {
                VMCSE.Instance.LogError("Weapon event of " + name + " not found");
                return "";
            }

            return weaponEvent;
        }

        public string? GetName()
        {
            return name;
        }
    }
}
