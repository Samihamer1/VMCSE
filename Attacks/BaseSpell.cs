using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.Components;

namespace VMCSE.Attacks
{
    public abstract class BaseSpell : BaseAttack
    {
        public DevilCrestHandler handler;
        public string EVENTNAME;
        private float cooldown = 0;
        private float lastUsedTime = 0;
        public Sprite ICON;
        public Sprite ICONGLOW;

        public BaseSpell(DevilCrestHandler handler) {
            if (HeroController.instance == null) { return; }
            PlayMakerFSM spellfsm = HeroController.instance.gameObject.LocateMyFSM("Silk Specials");
            fsm = spellfsm;
            

            this.handler = handler;
        }

        public abstract bool OnManualCooldown();

        public void StartCooldownTimer(float cooldown)
        {
            lastUsedTime = Time.time;
            this.cooldown = cooldown;
        }

        public bool OnCooldown()
        {
            float currentTime = Time.time;

            if (currentTime-lastUsedTime >= cooldown)
            {
                return false;
            }
            return true;
        }

        public void SetStateAsInit(string statename)
        {
            fsm.AddGlobalTransition(EVENTNAME, statename);            
        }
    }
}
