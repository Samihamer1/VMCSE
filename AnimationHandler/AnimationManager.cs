using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VMCSE.AnimationHandler
{
    public static class AnimationManager
    {
        private static bool init = false;
        public const float SPRITESCALE = 3.5f;
        public static GameObject DevilSwordAnimator;
        public static void InitAnimations()
        {
            if (init) return;
            init = true;

            #region DevilSword animations

            DevilSwordAnimator = CreateAnimationObject("DevilSword");

            //DevilSword animations (for DevilSwordDante)

            //SlashEffect
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.SlashEffect.spritesheet.png", "SlashEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 217, 190);
            SetFrameToTrigger(DevilSwordAnimator, "SlashEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTrigger(DevilSwordAnimator, "SlashEffect", 3);

            //SlashAltEffect
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.SlashAltEffect.spritesheet.png", "SlashAltEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 266, 237);
            SetFrameToTrigger(DevilSwordAnimator, "SlashAltEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTrigger(DevilSwordAnimator, "SlashAltEffect", 3);

            //SlashUpEffect
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.SlashUpEffect.spritesheet.png", "SlashUpEffect", 30, tk2dSpriteAnimationClip.WrapMode.Once, 4, 215, 241);
            SetFrameToTrigger(DevilSwordAnimator, "SlashUpEffect", 1); // To activate the damage frames within NailSlash
            SetFrameToTrigger(DevilSwordAnimator, "SlashUpEffect", 3);

            //DownspikeEffect
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.DownspikeEffect.spritesheet.png", "DownSpikeEffect", 20, tk2dSpriteAnimationClip.WrapMode.Once, 4, 1, 1);
            SetFrameToTrigger(DevilSwordAnimator, "DownSpikeEffect", 0); // To activate the damage frames within NailSlash
            SetFrameToTrigger(DevilSwordAnimator, "DownSpikeEffect", 3);

            //Downspike
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.Downspike.spritesheet.png", "DownSpike", 16, tk2dSpriteAnimationClip.WrapMode.Once, 2, 270, 250);

            //Downspike Antic
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.DownspikeAntic.spritesheet.png", "DownSpike Antic", 18, tk2dSpriteAnimationClip.WrapMode.Once, 3, 270, 250);

            //Dashstab Antic
            //LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.Downspike.spritesheet.png", "DownSpike", 16, tk2dSpriteAnimationClip.WrapMode.Once, 2, 270, 250);

            //Dashstab
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.Dashstab.spritesheet.png", "Dashstab", 18, tk2dSpriteAnimationClip.WrapMode.Once, 3, 313, 192);

            //HighTime Effect
            LoadAnimationTo(DevilSwordAnimator, "VMCSE.Resources.DevilSword.HighTimeEffect.spritesheet.png", "HighTimeEffect", 24, tk2dSpriteAnimationClip.WrapMode.Once, 4, 322, 304);

            #endregion
        }

        private static void SetFrameToTrigger(GameObject animatorObject, string animationName, int frame) {
            animatorObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName(animationName).frames[frame].triggerEvent = true;
        }

        private static GameObject CreateAnimationObject(string name)
        {
            GameObject obj = new GameObject();
            obj.name = name;
            obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimation animation = obj.AddComponent<tk2dSpriteAnimation>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animation;
            animation.clips = new tk2dSpriteAnimationClip[0];
            UnityEngine.GameObject.DontDestroyOnLoad(obj);
            UnityEngine.GameObject.DontDestroyOnLoad(animator);
            return obj;
        }

        private static void LoadAnimationTo(GameObject animator, string path, string name, int fps, tk2dSpriteAnimationClip.WrapMode wrapmode, int length, int xbound, int ybound)
        {
            List<tk2dSpriteAnimationClip> list = animator.GetComponent<tk2dSpriteAnimator>().Library.clips.ToList<tk2dSpriteAnimationClip>();

            Texture2D texture1 = ResourceLoader.LoadTexture2D(path);
            

            string[] names = new string[length];
            Rect[] rects = new Rect[length];
            Vector2[] anchors = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                names[i] = name + (i + 1).ToString();
                rects[i] = new Rect(i * xbound, i * ybound, xbound, ybound);
                anchors[i] = new Vector2(xbound/2, ybound/2);
            }

            tk2dSpriteCollectionData spriteCollectiondata = Tk2dHelper.CreateTk2dSpriteCollection(texture1, names, rects, anchors, new GameObject());
            spriteCollectiondata.material = HeroController.instance.GetComponent<MeshRenderer>().material;

            tk2dSpriteAnimationFrame[] list1 = new tk2dSpriteAnimationFrame[length];

            for (int i = 0; i < length; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = spriteCollectiondata;
                frame.spriteId = i;
                

                list1[i] = frame;
            }

            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = name;
            clip.fps = fps;
            clip.frames = list1;
            clip.wrapMode = wrapmode;


            clip.SetCollection(spriteCollectiondata);

            list.Add(clip);
            tk2dSpriteAnimation animation = animator.GetComponent<tk2dSpriteAnimator>().Library;
            animation.clips = list.ToArray();            
            Helper.SetPrivateField<bool>(animation, "isValid", false); //to refresh the animation lookup
            animation.ValidateLookup();
        }
    }
}
