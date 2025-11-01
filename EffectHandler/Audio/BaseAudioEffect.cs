using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.EffectHandler.Audio
{
    internal abstract class BaseAudioEffect
    {
        internal AudioClip? storedClip;

        /// <summary>
        /// To be overridden.
        /// Attempts to find the necessary AudioClip to store.
        /// </summary>
        /// <returns>The AudioClip to be stored</returns>
        public abstract AudioClip? TryInitialiseClip();

        public BaseAudioEffect()
        {
            storedClip = TryInitialiseClip();
        }

        public AudioClip? GetAudioClip()
        {
            return storedClip;
        }
    }
}
