using System;
using System.Collections.Generic;
using UnityEngine;

namespace VMCSE.Components
{
    public class StyleMeter : MonoBehaviour
    {
        private GameObject? StyleRoot;
        private GameObject[] StyleForegrounds = new GameObject[7];
        private GameObject[] StyleBackgrounds = new GameObject[7];
        private float[] meterlevels = { 20, 40, 55, 70, 75, 80, 90 };
        private Dictionary<string, float> recentlyHitAttacks = new Dictionary<string, float>();
        private int attackStackSize = 6; // how many differently named attacks can be currently stored for scaling style gain
        private float meterloss = 4f;
        private float metermax;
        private float meter = 0;
        private float rankDecayTime = 2f;
        private float rankDecayTimer = 0;
        private int maxRank = 6;
        private int rank = 0;

        private float BASESTYLEGAIN = 10;
        private int BASEHITLIMIT = 6;
        private void CreateStyleUI()
        {
            if (HudCanvas.instance.gameObject == null) { return; }
            if (StyleRoot != null) { return; }

            StyleRoot = new GameObject();
            DontDestroyOnLoad(StyleRoot);
            StyleRoot.transform.parent = HudCanvas.instance.transform;
            StyleRoot.layer = HudCanvas.instance.gameObject.layer;
            StyleRoot.transform.localPosition = new Vector3(28, -7, 0);
            StyleRoot.name = "StyleRoot";


            for (int i = 0; i < 7; i++)
            {
                AddFullStyleUI(i + 1);
            }

            StyleRoot.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
        }

        private GameObject AddPartialStyleUI(string path)
        {
            GameObject StyleRank = new GameObject();
            DontDestroyOnLoad(StyleRank);
            StyleRank.transform.parent = StyleRoot.transform;
            StyleRank.layer = HudCanvas.instance.gameObject.layer;
            StyleRank.transform.localPosition = new Vector3(0, 0, 0);

            StyleRank.AddComponent<SpriteRenderer>();

            try
            {
                StyleRankUI ui = StyleRank.AddComponent<StyleRankUI>();
                ui.SetSprite(path);
            }
            catch (Exception ex)
            {
                VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }


            return StyleRank;
        }

        private void AddFullStyleUI(int index)
        {
            string bgAsset = "Style" + index + "BG";
            string fgAsset = "Style" + index + "FG";

            GameObject bg = AddPartialStyleUI("VMCSE.Resources.UI.Style.Style" + index + "BG.png");
            GameObject fg = AddPartialStyleUI("VMCSE.Resources.UI.Style.Style" + index + "FG.png");

            bg.gameObject.SetActive(false);
            fg.gameObject.SetActive(false);

            StyleBackgrounds[index-1] = bg;
            StyleForegrounds[index-1] = fg;
        }

        //returns amount of style gained
        private float ProcessHitLanded(string nameOfAttack)
        {
            //Get non scaled style gain and hit limit
            float gainedStyle = BASESTYLEGAIN;
            float hitLimit = BASEHITLIMIT;
            if (AttackStyleGains.attackStyleGains.ContainsKey(nameOfAttack))
            {
                StyleGain stylegain = AttackStyleGains.attackStyleGains[nameOfAttack];
                gainedStyle = stylegain.GetStyleGain();
                hitLimit = stylegain.GetHitsTilNoGain();

                //in case of no limit, just add
                if (stylegain is StyleGainNoLimit)
                {
                    return gainedStyle;
                }
            }

            //check if attack is stored within the list
            float numberOfHits;
            bool found = recentlyHitAttacks.TryGetValue(nameOfAttack, out numberOfHits);

            if (found)
            {
                //increment stored hit amount and apply scaling
                recentlyHitAttacks[nameOfAttack] = Math.Clamp(numberOfHits+1, 1, hitLimit);

                float halfLimit = (float)Math.Round(hitLimit / 2);
                if (halfLimit < 1)
                {
                    halfLimit = 1; //no shenanigans
                }
                float scaleRatio = 1-((numberOfHits - halfLimit) / halfLimit);

                //scaling applies linearly for each hit over half, reducing it at max to 0 gain
                gainedStyle *= scaleRatio;
                return gainedStyle;
            }

            //if not found, and not at stack limit
            if (recentlyHitAttacks.Count < attackStackSize)
            {
                //add as usual, non scaled
                recentlyHitAttacks[nameOfAttack] = 1;
                return gainedStyle;
            }

            //if at stack limit, replace entry with largest number of hits with the new attack
            string largestKey = "";
            float storedLargestVal = 0;
            foreach (string key in recentlyHitAttacks.Keys)
            {
                float currentVal = recentlyHitAttacks[key];
                if (storedLargestVal < currentVal) { storedLargestVal = currentVal; largestKey = key; }
            }

            recentlyHitAttacks.Remove(largestKey);
            recentlyHitAttacks[nameOfAttack] = 1;
            return gainedStyle;
        }

        public void HitLanded(HitInstance instance)
        {
            CreateStyleUI();

            float styleGain = ProcessHitLanded(instance.Source.name);

            AddToMeter(styleGain);
        }

        public void LargeLoss()
        {
            rank -= 1;
            rank = Math.Clamp(rank, 0, maxRank);
            metermax = meterlevels[rank];
            if (meter > metermax * 0.3f)
            {
                meter = metermax * 0.3f;
            }

            //reset stored attack list
            recentlyHitAttacks.Clear();
        }

        private void AddToMeter(float amount)
        {
            meter += amount;
            rankDecayTimer = 0;

            //clamp meter in case of max rank
            meter = Math.Clamp(meter, 0, meterlevels[maxRank]);

            //Check for rank ups
            if (meter > metermax)
            {
                float spareMeter = meter - metermax;
                RankUp();
                AddToMeter(spareMeter);
            }
        }

        private void RankUp()
        {
            rank += 1;
            metermax = meterlevels[rank];
            meter = 0;
        }

        private void RankDown()
        {
            rank -= 1;
            metermax = meterlevels[rank];
            meter = metermax;
            rankDecayTime = 0;
        }

        public void FixedUpdate()
        {
            float loss = meterloss * Time.deltaTime;

            meter -= loss;

            if (meter < 0 && rank > 0)
            {
                if (rankDecayTimer > rankDecayTime)
                {
                    RankDown();
                }

                rankDecayTimer += Time.deltaTime;
            }

            meter = Math.Clamp(meter, 0, metermax);

            UpdateUI();
        }

        private void UpdateUI()
        {
            DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            if (StyleRoot == null) { return; }
            if (!handler.IsDevilEquipped()) { StyleRoot.SetActive(false); return; }
            StyleRoot.SetActive(true);

            for (int i = 0; i < StyleBackgrounds.Length; i++)
            {
                StyleBackgrounds[i].gameObject.SetActive(false);
                StyleForegrounds[i].gameObject.SetActive(false);
            }

            if (meter == 0 && rank == 0) { return; }

            StyleBackgrounds[rank].gameObject.SetActive(true);
            StyleForegrounds[rank].gameObject.SetActive(true);

            StyleBackgrounds[rank].GetComponent<StyleRankUI>().SetFillAmount(meter / metermax);
        }
    }
}
