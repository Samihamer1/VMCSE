using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE
{
    internal static class AnimationLibraryNames
    {
        public static readonly string SHAMAN = "Hornet CrestWeapon Shaman Anim";
        public static readonly string WANDERER = "Hornet CrestWeapon Dagger Anim";
        public static readonly string WITCH = "Hornet CrestWeapon Whip Anim";
        public static readonly string CLOAKLESS = "Hornet Cloakless Anim";
        public static readonly string DEFAULT = "Knight";
    }

    internal static class StateNames
    {
        public static readonly string GREATSLASHCHOICE = "Great Slash Choice";
        public static readonly string DASHSLASHCHOICE = "Dash Slash Choice";
        public static readonly string CYCLONESLASHCHOICE = "Cyclone Slash Choice";
    }

    internal static class WeaponNames
    {
        public static readonly string DEVILSWORD = "DevilSword";
        public static readonly string KINGCERBERUS = "KingCerberus";
    }

    internal static class EventNames
    {
        public static readonly string DEVILSWORD = "DEVILSWORD";
        public static readonly string KINGCERBERUS = "KINGCERBERUS";
        public static readonly string REACTOR = "REACTOR";
        public static readonly string DRIVE = "DRIVE";
        public static readonly string ROUNDTRIP = "ROUNDTRIP";
    }

    internal static class AttackNames
    {
        public static readonly string REDSLASH = "REDSLASH";
        public static readonly string DRIVESLASH1 = "DRIVE1";
        public static readonly string DRIVESLASH2 = "DRIVE2";
        public static readonly string HIGHTIME = "HIGH TIME";
        public static readonly string HIGHTIMERED = "HIGH TIME RED";
        public static readonly string MILLIONSTAB = "MILLIONSTAB";
        public static readonly string MILLIONSTABRED = "MILLIONSTABRED";
        public static readonly string ROUNDTRIP = "ROUND TRIP";
        public static readonly string ROUNDTRIPRED = "ROUND TRIP RED";
        public static readonly string ROUNDTRIPMINI = "ROUND TRIP MINI";
    }

    #region Style system constants

    internal class StyleGain
    {
        private float styleGain;
        private float hitsTilNoGain;
        public StyleGain(float styleGain, float hitsTilNoGain)
        {
            this.styleGain = styleGain;
            this.hitsTilNoGain = hitsTilNoGain;
        }

        public float GetStyleGain() { return styleGain; }
        public float GetHitsTilNoGain() { return hitsTilNoGain; }
    }

    internal class StyleGainNoLimit : StyleGain
    {
        //A variant of StyleGain that doesn't record the hit on the recently hit attacks list.
        public StyleGainNoLimit(float styleGain) : base(styleGain, 0)
        {
        }
    }

    internal static class AttackStyleGains
    {
        public static readonly Dictionary<string, StyleGain> attackStyleGains = new()
        {
            [AttackNames.DRIVESLASH1] = new StyleGain(8, 5),
            [AttackNames.DRIVESLASH2] = new StyleGain(14, 5),
            [AttackNames.HIGHTIME] = new StyleGain(30, 3),
            [AttackNames.MILLIONSTAB] = new StyleGain(3.5f, 12),
            [AttackNames.MILLIONSTAB] = new StyleGain(4, 12),
            [AttackNames.ROUNDTRIP] = new StyleGain(3, 10),
            [AttackNames.ROUNDTRIPRED] = new StyleGain(2.5f, 10),
            [AttackNames.REDSLASH] = new StyleGainNoLimit(4f),
            [AttackNames.ROUNDTRIPMINI] = new StyleGainNoLimit(2f)
        };
    }

    #endregion

    internal static class ColorConstants
    {
        public static readonly Color DanteRed = new Color(1, 0.25f, 0.25f, 0.5f);
        public static readonly Color CerberusBlue = new Color(0.75f, 0.85f, 1, 1);
    }
}
