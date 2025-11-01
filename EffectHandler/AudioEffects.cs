using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.EffectHandler.Audio;

namespace VMCSE.EffectHandler
{
    /// <summary>
    /// AudioEffects stores all the classes deriving from BaseAudioEffect.
    /// It is created and used through the EffectManager, and should not be accessed otherwise.
    /// </summary>
    internal class AudioEffects
    {
        #region AudioEffect names
        internal static class Names
        {
            public static readonly string WITCHSLASH = "Witch Slash";
            public static readonly string SILKSPEARZAP = "Silkspear Zap";
        }
        #endregion

        #region AudioEffects and their keys
        private Dictionary<string,Type> audioEffectDictionary = new Dictionary<string,Type>()
        {
            [Names.SILKSPEARZAP] = typeof(SilkspearZapAudioEffect),
            [Names.WITCHSLASH] = typeof(WitchSlashAudioEffect),

        };
        #endregion

        private Dictionary<string, BaseAudioEffect> storedAudioEffects = new Dictionary<string, BaseAudioEffect>();

        public AudioEffects() {
            Initialise();
        }

        private void Initialise()
        {
            foreach (string key in audioEffectDictionary.Keys)
            {
                Type t = audioEffectDictionary[key];
                try
                {
                    var effect = Activator.CreateInstance(t);

                    storedAudioEffects.Add(key, (BaseAudioEffect)effect);
                }
                catch (Exception e)
                {
                    VMCSE.Instance.LogError($"Audio Effect '{key}' unable to be created. Is the type correct?");
                    continue;
                }

            }
        }

        /// <summary>
        /// Attempts to get the AudioClip stored under the given 'clipName'.
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns>The AudioClip if found, null if not</returns>
        public AudioClip? GetAudioClip(string clipName)
        {
            if (storedAudioEffects.ContainsKey(clipName))
            {
                AudioClip? clip = storedAudioEffects[clipName].GetAudioClip();
                if (clip == null)
                {
                    VMCSE.Instance.LogError($"AudioEffect {clipName} found, but stored clip is null.");
                }
                return clip;
            }
            VMCSE.Instance.LogError($"AudioEffect {clipName} not found.");
            return null;
        }
    }
}
