using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.AnimationHandler
{
    public class VMCAnimation
    {
        public string animName;
        public string filePath;
        public int fps;
        public tk2dSpriteAnimationClip.WrapMode wrapMode;
        public int numberOfFrames;
        public int spriteXBound;
        public int spriteYBound;
    }
}
