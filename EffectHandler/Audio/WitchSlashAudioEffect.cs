using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.EffectHandler.Audio
{
    internal class WitchSlashAudioEffect : BaseAudioEffect
    {
        public override AudioClip? TryInitialiseClip()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            GameObject attacks = hornet.Child("Attacks");
            if (attacks == null) { return null; }

            GameObject witch = attacks.Child("Witch");
            if (witch == null) { return null; }

            GameObject slash = witch.Child("Slash");
            if (slash == null) { return null; }

            return slash.GetComponent<AudioSource>().clip;            
        }
    }
}
