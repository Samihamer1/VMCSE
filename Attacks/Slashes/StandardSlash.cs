using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using VMCSE.AnimationHandler;
using VMCSE.Components;

namespace VMCSE.Attacks.Slashes
{
    public class StandardSlash : MonoBehaviour
    {
        private bool hasInitialised = false;

        private NailSlash nailSlash;
        private AudioSource audioSource;
        private DamageEnemies damageEnemies;
        private tk2dSpriteAnimator animator;
        private tk2dSprite sprite;
        private PolygonCollider2D collider;

        private Color color = new Color(1, 1, 1, 1); //Base of nothing

        private StandardSlash? redSlash;

        //Start and Awake were causing me issues that I wasn't willing to fix.
        private void TryInit()
        {
            if (hasInitialised) { return; }
            hasInitialised = true;

            //devilslash = new GameObject("DevilSword Slash");
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            //devilslash.transform.parent = devilroot.transform;
            //devilslash.transform.localPosition = new Vector3(-1.1f, 0.4f, -0.001f);
            sprite = gameObject.AddComponent<tk2dSprite>();
            animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            collider = gameObject.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;
            collider.enabled = false;
            damageEnemies = Helper.AddDamageEnemies(gameObject);
            //an.library = AnimationManager.GetDevilSwordAnimator();

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = HeroController.instance.gameObject.Child("Attacks").Child("Scythe").Child("Slash").GetComponent<AudioSource>().clip;
            audioSource.outputAudioMixerGroup = HeroController.instance.gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;
            audioSource.playOnAwake = false;

            nailSlash = gameObject.AddComponent<NailSlash>();
            //s.scale = new Vector3(1.1f, 1, 1);
            nailSlash.animName = "";
            nailSlash.hc = HeroController.instance;
            nailSlash.activateOnSlash = new GameObject[0];
            nailSlash.enemyDamager = damageEnemies;

            //Color
            nailSlash.AttackStarting += ColorSlash;
            

            gameObject.layer = (int)PhysLayers.HERO_ATTACK;

            //tink
            //NailSlashTerrainThunk tink = gameObject.AddComponent<NailSlashTerrainThunk>();
            //tink.doRecoil = true;
            //tink.handleTink = true;

            //clash tink -- i tried to make this without copying from slash, but dead end. perhaps revisit later.
            GameObject attacks = HeroController.instance.gameObject.Child("Attacks");
            if (attacks == null) { return; }
            GameObject def = attacks.Child("Default");
            if (def == null ) { return; }
            GameObject slash = def.Child("Slash");
            if (slash == null) { return; }
            GameObject prefabclash = slash.Child("Clash Tink");
            if (prefabclash == null) { return; }

            GameObject clashtink = Instantiate(prefabclash);
            clashtink.transform.parent = gameObject.transform;
            clashtink.transform.localPosition = Vector3.zero;

            nailSlash.clashTinkPoly = clashtink.GetComponent<PolygonCollider2D>();
        }

        private void ColorSlash()
        {
            sprite.color = color;
        }

        public void SetScale(Vector3 scale)
        {
            TryInit();
            nailSlash.scale = scale;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetScale(scale);
        }

        public void SetAnimation(tk2dSpriteAnimation library, string animName)
        {
            TryInit();
            animator.library = library;
            nailSlash.animName = animName;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetAnimation(library, animName);
        }

        public void SetNailDamageMultiplier(float mult)
        {
            TryInit();
            damageEnemies.nailDamageMultiplier = mult;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetNailDamageMultiplier(mult * 0.33f); //Weaker
        }

        public void SetDamageDirection(float direction)
        {
            TryInit();

            damageEnemies.direction = direction;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetDamageDirection(direction);
        }

        public void SetDownAttack()
        {
            TryInit();
            HeroDownAttack downAttack = gameObject.GetComponent<HeroDownAttack>();
            if (downAttack == null)
            {
                downAttack = gameObject.AddComponent<HeroDownAttack>();
            }

            downAttack.hc = HeroController.instance;
            downAttack.attack = nailSlash;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetDownAttack();
        }

        public void SetColliderSize(string SlashPrefabName)
        {
            TryInit();

            // Load prefab by name
            GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(SlashPrefabName);
            if (prefab == null)
            {
                throw new ArgumentException("Prefab '" + SlashPrefabName + "' not found in VMCSE AssetBundle!");
            }

            collider.points = prefab.GetComponent<PolygonCollider2D>().points;

            //To account for the increased anim size
            Helper.ScalePolygonCollider(collider, (float)Math.Sqrt(AnimationManager.SPRITESCALE));

            Destroy(prefab);

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetColliderSize(SlashPrefabName);
        }

        public void SetColliderSize(Vector2[] points)
        {
            TryInit();

            collider.points = points;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetColliderSize(points);
        }

        public void SetStunDamage(float stunDamage)
        {
            damageEnemies.stunDamage = stunDamage;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetStunDamage(stunDamage * 0.2f); //Weaker
        }

        public void SetMagnitude(float magnitude)
        {
            damageEnemies.magnitudeMult = magnitude;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        } 

        public void SetAudioClip(AudioClip? clip)
        {
            if (clip == null) { return; }

            audioSource.clip = clip;

            //Changing for the RedSlash if possible
            if (redSlash == null) { return; }

            redSlash.SetAudioClip(clip);
        }

        /// <summary>
        /// Creates and activates the Red Slash mechanic. This clones the current status of the slash as a red variant.
        /// You can create it at any time. Changes made by other functions are still applied.
        /// </summary>
        public void CreateRedSlash(Vector3 redSlashOffset)
        {
            TryInit();

            GameObject redCopy = GameObject.Instantiate(gameObject);
            redCopy.transform.parent = gameObject.transform;
            redCopy.transform.localPosition = redSlashOffset;
            redCopy.name = AttackNames.REDSLASH;

            //Damage modifiers
            DamageEnemies redDamage = redCopy.GetComponent<DamageEnemies>();
            redDamage.silkGeneration = HitSilkGeneration.None;
            redDamage.nailDamageMultiplier *= 0.33f;
            redDamage.stunDamage *= 0.2f;
            redDamage.magnitudeMult = 0f;

            //NailSlash modifiers
            NailSlash redNailSlash = redCopy.GetComponent<NailSlash>();
            redNailSlash.hc = HeroController.instance;
            GameObject.Destroy(redCopy.GetComponent<NailSlashRecoil>());

            //We need the absolute values of the scale since the redslash is a child of the normal slash
            //if the child has negative scale, we dont want red slash to flip again
            Vector3 scale = redNailSlash.scale;
            redNailSlash.scale = new Vector3(Math.Abs(scale.x), Math.Abs(scale.y), Math.Abs(scale.z));

            //Clearing the color from redSlash
            redNailSlash.AttackStarting -= ColorSlash;

            redSlash = redCopy.GetComponent<StandardSlash>(); //There's a StandardSlash in redSlash since its a copy.

            animator.AnimationEventTriggeredEvent += RedSlash;
        }

        private void RedSlash(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
        {
            if (clip.GetFrame(frame).eventInfo != "RedSlash") { return; }

            if (redSlash == null) { return; }

            DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();

            if (handler == null) { VMCSE.Instance.LogError("Handler not found when attempting Red Slash of " + gameObject.name); return; }

            if (!handler.ConsumeChaserBlade()) { return; }

            tk2dSpriteAnimator redAnim = redSlash.GetComponent<tk2dSpriteAnimator>();
            redAnim.Sprite.color = ColorConstants.DanteRed;

            redSlash.GetComponent<NailSlash>().StartSlash();
        }

    }
}
