using System;
using UnityEngine;

namespace VMCSE.Components
{
    public class StyleMeter : MonoBehaviour
    {
        private GameObject? StyleRoot;
        private GameObject[] StyleForegrounds = new GameObject[7];
        private GameObject[] StyleBackgrounds = new GameObject[7];
        private float[] meterlevels = { 20, 30, 40, 55, 60, 70, 90 };
        private float meterloss = 4f;
        private float metermax;
        private float meter = 0;
        private float rankDecayTime = 2f;
        private float rankDecayTimer = 0;
        private int maxRank = 6;
        private int rank = 0;
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
            GameObject bg = AddPartialStyleUI("VMCSE.Resources.UI.Style.Style" + index + "BG.png");
            GameObject fg = AddPartialStyleUI("VMCSE.Resources.UI.Style.Style" + index + "FG.png");

            bg.gameObject.SetActive(false);
            fg.gameObject.SetActive(false);

            StyleBackgrounds[index-1] = bg;
            StyleForegrounds[index-1] = fg;
        }
        public void HitLanded(HitInstance instance)
        {
            CreateStyleUI();

            AddToMeter(15);
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
