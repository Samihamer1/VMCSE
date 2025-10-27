using GlobalSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using VMCSE.CanvasHandler;

namespace VMCSE.Components
{
    public class DevilTrigger : MonoBehaviour
    {
        private GameObject? TriggerRoot;
        private GameObject[] TriggerBarObjects = new GameObject[10];
        private float TRIGGERBARMAX = 20; //only for a single bar. all 10 add up to 200
        private float TRIGGERMAX = 200;
        private float TRIGGERGAIN = 50; //return to 6.25 after testing
        private float trigger = 0f;

        private bool InTrigger = false;

        private const float TRIGGERLOSSPERSECOND = 10;

        private GameObject? TriggerEffect;

        #region trigger ui

        public void CreateTriggerUI()
        {
            if (HudCanvas.instance.gameObject == null) { return; }
            if (TriggerRoot != null) { return; }

            TriggerRoot = new GameObject();
            DontDestroyOnLoad(TriggerRoot);
            TriggerRoot.transform.parent = HudCanvas.instance.transform;
            TriggerRoot.layer = HudCanvas.instance.gameObject.layer;
            TriggerRoot.transform.localPosition = new Vector3(-4.6f, -2.8f, 0);
            TriggerRoot.name = "TriggerRoot";

            CreateFullTriggerBar();

            TriggerRoot.transform.localScale = new Vector3(1.75f, 1.75f, 1);

            UpdateUI();

        }

        private void CreateFullTriggerBar()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject holdingObject = new GameObject("Trigger Bar " + i);
                holdingObject.transform.parent = TriggerRoot.transform;
                holdingObject.transform.localPosition = new Vector3(0.28f * i, 0, 0);


                GameObject bgFill = CreateTriggerBackground();
                GameObject bgGray = CreateTriggerBackground();
                GameObject fg = CreateTriggerForeground();

                bgFill.transform.parent = holdingObject.transform;
                bgFill.transform.localPosition = new Vector3(0, 0, 0f); 

                bgGray.transform.parent = holdingObject.transform;
                bgGray.name = "Gray";
                bgGray.transform.localPosition = new Vector3(0, 0, 0.01f);
                bgGray.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f);

                fg.transform.parent = holdingObject.transform;
                fg.transform.localPosition = new Vector3(0, 0, -0.01f); 

                TriggerBarObjects[i] = holdingObject;
            }
        }

        private GameObject CreateTriggerBackground()
        {
            GameObject obj = new GameObject("Background");
            TiledCanvasObject tile = obj.AddComponent<TiledCanvasObject>();

            Sprite? backgroundSprite = ResourceLoader.LoadAsset<Sprite>("DevilTriggerBGv2");
            tile.SetSprite(backgroundSprite);

            return obj;
        }

        private GameObject CreateTriggerForeground()
        {
            GameObject obj = new GameObject("Foreground");
            CanvasObject canvas = obj.AddComponent<CanvasObject>();

            Sprite? foregroundSprite = ResourceLoader.LoadAsset<Sprite>("DevilTriggerFGv2");

            canvas.SetSprite(foregroundSprite);

            return obj;
        }

        private void UpdateUI()
        {
            float storedTrigger = trigger;
            for (int i = 0; i < TriggerBarObjects.Length; i++)
            {
                GameObject triggerBar = TriggerBarObjects[i];

                GameObject fg = triggerBar.Child("Foreground");
                GameObject bg = triggerBar.Child("Background");

                SpriteRenderer fgRenderer = fg.GetComponent<SpriteRenderer>();
                SpriteRenderer bgRenderer = bg.GetComponent<SpriteRenderer>();

                fgRenderer.color = new Color(1, 1, 1);
                fg.transform.localPosition = new Vector3(0, 0, -0.01f);

                //opacity of the bg is linear with the value of the current bar
                float opacity = storedTrigger / TRIGGERBARMAX;
                opacity = Math.Clamp(opacity, 0, 1);

                //if we have enough trigger to fully fill this bar, apply the effects for this
                if (storedTrigger >= TRIGGERBARMAX)
                {
                    //fgRenderer.color = new Color(0.65f, 0.4f, 1);
                    fg.transform.localPosition = new Vector3(0, 0, -0.011f);

                }

                //opacity of the bg

                bgRenderer.color = new Color(1, 1, 1, opacity);


                storedTrigger -= TRIGGERBARMAX;
            }
        }

        #endregion

        #region trigger logic

        public void StartTrigger()
        {
            if (trigger != TRIGGERMAX) { return; }

            InTrigger = true;
        }

        private void EndTrigger()
        {
            InTrigger = false;
            trigger = 0;
        }

        private void AddToTrigger(float value)
        {
            if (value > 0 && InTrigger) { return; } //no trigger gain while in trigger

            trigger += value;

            trigger = Math.Clamp(trigger, 0, TRIGGERMAX);

            UpdateUI();
        }

        //Add to trigger
        public void HitLanded(HitInstance instance)
        {
            //One nail damage hit is equal to 5 trigger gain. It scales linearly off of this.
            //Trigger gain from a single blow maxes at 20.

            float nailDamage = HeroController.instance.playerData.nailDamage;

            float damageRatio = instance.DamageDealt / nailDamage;

            float triggerGain = TRIGGERGAIN * damageRatio;
            triggerGain = Math.Clamp(triggerGain, 0, TRIGGERBARMAX);

            AddToTrigger(triggerGain);
        }

        #endregion

        #region trigger effects
        
        private void CreateTriggerEffect()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { VMCSE.Instance.LogError("Hornet not found when creating trigger effect. Called too early?");  return;  }

            GameObject effects = hornet.Child("Effects");
            if (effects == null) { return; }

            GameObject canbindeffect = effects.Child("Can Bind Effect");
            if (canbindeffect == null) { return; }

            GameObject soulbursteffect = canbindeffect.Child("Soul Burst");

            TriggerEffect = Instantiate(soulbursteffect);
            TriggerEffect.transform.parent = effects.transform;
            TriggerEffect.transform.localPosition = new Vector3(0, 0, 0.001f);
            TriggerEffect.GetComponent<SpriteRenderer>().color = ColorConstants.DanteRed;
            DeactivateAfterDelay deactivate = TriggerEffect.AddComponent<DeactivateAfterDelay>();
            deactivate.time = 0.5f;
        }

        #endregion

        public void InitialiseTriggerEffects()
        {
            CreateTriggerEffect();
        }

        public bool IsInTrigger()
        {
            return InTrigger;
        }

        private void Update()
        {
            if (!InTrigger) { return; }

            //Trigger loss
            AddToTrigger(-TRIGGERLOSSPERSECOND * Time.deltaTime);

            if (trigger <= 0)
            {
                EndTrigger();
                return;
            }

            //Effect
            if (TriggerEffect == null) { return; }
            if (!TriggerEffect.activeSelf) { TriggerEffect.SetActive(true); } //the deactivateafterdelay will auto loop this
        }
    }
}
