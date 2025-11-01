using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.EffectHandler
{
    /// <summary>
    /// Differing from the ResourceLoader, EffectManager handles miscellaneous storage and activation of 'effects'.
    /// This could include GameObjects, AudioClips, Particles, ect.
    /// It does not load any external files. This behaviour is always delegated to ResourceLoader.
    /// </summary>
    internal static class EffectManager
    {
        private static AudioEffects? AudioEffects;

        /// <summary>
        /// To be called whenever the Hero has fully loaded. Not to be used before this.
        /// </summary>
        public static void Initialise()
        {
            AudioEffects = new AudioEffects();
        }

        /// <summary>
        /// Attempts to get the AudioClip stored under the given 'clipName'.
        /// clipName should be a value from the AudioEffects.Names static class.
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns>The AudioClip if found, null if not</returns>
        public static AudioClip? GetAudioClip(string clipName) {
            if (AudioEffects == null) { VMCSE.Instance.LogError($"AudioEffects is null. Cannot request AudioEffect {clipName}"); return null; }
            return AudioEffects.GetAudioClip(clipName);
        }
    }
}
