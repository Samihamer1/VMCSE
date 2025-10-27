using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using VMCSE.Attacks;
using VMCSE.Weapons;

namespace VMCSE.Components
{
    internal class CanvasUI : MonoBehaviour
    {
        private Sprite? NeutralCircleSprite;
        private Sprite? NeutralCircleSpriteGlow;
        private Sprite? DirectionalCircleSprite;
        private Sprite? DirectionalCircleSpriteGlow;
        private Sprite? DoubleDirectionalCircleSprite;
        private Sprite? DoubleDirectionalCircleSpriteGlow;

        private Sprite BLANKSPRITE;

        private GameObject? SkillIconN; //neutral
        private GameObject? SkillIconH; //horizontal
        private GameObject? SkillIconU; //up
        private GameObject? SkillIconD; //down

        private DevilCrestHandler handler;

        private Color WHITE = Color.white;
        private Color GRAY = new Color(0.32f, 0.32f, 0.32f, 1f);

        private void Start()
        {
            handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            GetSprites();
            CreateSkillIcons();
            CreateTrigger();
        }

        private void CreateTrigger()
        {
            handler.GetTrigger().CreateTriggerUI();
        }

        private void GetSprites()
        {
            GameObject toolIcons = gameObject.Child("Tool Icons");

            GameObject presetIconNeutral = toolIcons.Child("Tool Icon N");
            RadialHudSwitchModeSettings hudSettingsNeutral = presetIconNeutral.GetComponent<RadialHudSwitchModeSettings>();
            NeutralCircleSprite = hudSettingsNeutral.regularMode;
            NeutralCircleSpriteGlow = hudSettingsNeutral.glowRegular;

            GameObject presetIconDirectional = toolIcons.Child("Tool Icon U");
            RadialHudSwitchModeSettings hudSettings = presetIconDirectional.GetComponent<RadialHudSwitchModeSettings>();
            DirectionalCircleSprite = hudSettings.regularMode;
            DirectionalCircleSpriteGlow = hudSettings.glowRegular;

            DoubleDirectionalCircleSprite = ResourceLoader.LoadAsset<Sprite>("HorizontalSkillCircle");
            DoubleDirectionalCircleSpriteGlow = ResourceLoader.LoadAsset<Sprite>("HorizontalSkillCircleGlow");

            BLANKSPRITE = new Sprite();
        }

        private void CreateSkillIcons()
        {
            GameObject toolIcons = gameObject.Child("Tool Icons");

            SkillIconN = CreateSkillIcon();
            SetSkillIconSprites(SkillIconN, BLANKSPRITE, NeutralCircleSpriteGlow);

            SkillIconD = CreateSkillIcon();
            SkillIconD.Child("Background").transform.SetRotation2D(180);

            SkillIconU = CreateSkillIcon();

            SkillIconH = CreateSkillIcon();
            SkillIconH.Child("Background").transform.SetRotation2D(90);

        }

        private void SetSkillIconSprites(GameObject skillIcon, Sprite foreground, Sprite background)
        {
            GameObject bg = skillIcon.Child("Background");
            GameObject fg = skillIcon.Child("Foreground");

            if (fg == null || bg == null) { return; }

            bg.GetComponent<SpriteRenderer>().sprite = background;
            fg.GetComponent<SpriteRenderer>().sprite = foreground;
        }

        private void SetSkillColor(GameObject skillIcon, Color color)
        {
            GameObject bg = skillIcon.Child("Background");
            GameObject fg = skillIcon.Child("Foreground");

            if (fg == null || bg == null) { return; }

            bg.GetComponent<SpriteRenderer>().color = color;
            fg.GetComponent<SpriteRenderer>().color = color;
        }

        private GameObject CreateSkillIcon()
        {
            GameObject toolIcons = gameObject.Child("Tool Icons");

            GameObject SkillIcon = new GameObject();
            UnityEngine.GameObject.DontDestroyOnLoad(SkillIcon);
            SkillIcon.transform.parent = toolIcons.transform;
            SkillIcon.transform.localScale = Vector3.one;
            SkillIcon.layer = HudCanvas.instance.gameObject.layer;

            GameObject SkillBackground = new GameObject("Background");
            SkillBackground.transform.parent = SkillIcon.transform;
            SkillBackground.layer = HudCanvas.instance.gameObject.layer;
            SkillBackground.transform.localScale = new Vector3(1,1);
            SkillBackground.transform.localPosition = new Vector3(0, 0);
            SpriteRenderer backgroundRenderer = SkillBackground.AddComponent<SpriteRenderer>();

            GameObject SkillForeground = new GameObject("Foreground");
            SkillForeground.transform.parent = SkillIcon.transform;
            SkillForeground.layer = HudCanvas.instance.gameObject.layer;
            SkillForeground.transform.localScale = new Vector3(1,1);
            SkillForeground.transform.localPosition = new Vector3(0, 0);
            SpriteRenderer foregroundRenderer = SkillForeground.AddComponent<SpriteRenderer>();

            return SkillIcon;
        }

        private void UpdateIcon(GameObject? SkillIcon, BaseSpell? spell, Sprite background, Sprite backgroundGlow)
        {
            if (SkillIcon == null) return;

            Sprite fg = BLANKSPRITE;
            Sprite bg = BLANKSPRITE;

            if (spell != null)
            {
                fg = spell.ICONGLOW;
                bg = backgroundGlow;

                SetSkillColor(SkillIcon, WHITE);
                
                if (spell.OnCooldown() || spell.OnManualCooldown())
                {
                    SetSkillColor(SkillIcon, GRAY);
                    fg = spell.ICON;
                    bg = background;
                }
            }

            SetSkillIconSprites(SkillIcon, fg, bg);
        }

        private void SetAllActiveTo(bool val)
        {
            SkillIconD.SetActive(val);
            SkillIconH.SetActive(val);
            SkillIconN.SetActive(val);
            SkillIconU.SetActive(val);
        }

        private void UpdateAllIcons()
        {
            if (handler == null) { return; }

            WeaponBase weapon = handler.getEquippedWeapon();

            SetAllActiveTo(true);

            if (! (HeroController.instance.playerData.CurrentCrestID == "Devil"))
            {
                SetAllActiveTo(false);
            }

            UpdateIcon(SkillIconH, weapon.horizontalSpell, DoubleDirectionalCircleSprite, DoubleDirectionalCircleSpriteGlow);
            UpdateIcon(SkillIconD, weapon.downSpell, DirectionalCircleSprite, DirectionalCircleSpriteGlow);
            UpdateIcon(SkillIconU, weapon.upSpell, DirectionalCircleSprite, DirectionalCircleSpriteGlow);
        }

        private void Update()
        {
            UpdateAllIcons();
        }
    }
}
