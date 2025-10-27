using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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

        private GameObject? redSlash;

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


            nailSlash = gameObject.AddComponent<NailSlash>();
            //s.scale = new Vector3(1.1f, 1, 1);
            nailSlash.animName = "";
            nailSlash.hc = HeroController.instance;
            nailSlash.activateOnSlash = new GameObject[0];
            nailSlash.enemyDamager = damageEnemies;
            

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

        public void SetScale(Vector3 scale)
        {
            TryInit();
            nailSlash.scale = scale;
        }

        public void SetAnimation(tk2dSpriteAnimation library, string animName)
        {
            TryInit();
            animator.library = library;
            nailSlash.animName = animName;
        }

        public void SetNailDamageMultiplier(float mult)
        {
            TryInit();
            damageEnemies.nailDamageMultiplier = mult;
        }

        public void SetDamageDirection(float direction)
        {
            TryInit();

            damageEnemies.direction = direction;
        }

        public void SetDownAttack()
        {
            TryInit();

            HeroDownAttack downAttack = gameObject.AddComponent<HeroDownAttack>();
            downAttack.hc = HeroController.instance;
            downAttack.attack = nailSlash;
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
        }

        public void SetColliderSize(Vector2[] points)
        {
            TryInit();

            collider.points = points;
        }

        public void SetStunDamage(float stunDamage)
        {
            damageEnemies.stunDamage = stunDamage;
        }

        public void SetMagnitude(float magnitude)
        {
            damageEnemies.magnitudeMult = magnitude;
        }

        /// <summary>
        /// Creates and activates the Red Slash mechanic. This clones the current staus of the slash as a red variant.
        /// Use ONLY after slash has been fully setup with other functions. 
        /// </summary>
        public void CreateRedSlash(Vector3 redSlashOffset)
        {
            TryInit();

            GameObject redCopy = GameObject.Instantiate(gameObject);
            redCopy.transform.parent = gameObject.transform;
            redCopy.transform.localPosition = redSlashOffset;
            redCopy.name = AttackNames.REDSLASH;
            redCopy.GetComponent<DamageEnemies>().silkGeneration = HitSilkGeneration.None;
            redCopy.GetComponent<DamageEnemies>().nailDamageMultiplier *= 0.33f;
            redCopy.GetComponent<DamageEnemies>().stunDamage *= 0.2f;
            redCopy.GetComponent<NailSlash>().hc = HeroController.instance;
            GameObject.Destroy(redCopy.GetComponent<NailSlashRecoil>());

            redSlash = redCopy;

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

        //public void SetupSlash()
        //{
        //    // Load prefab by name
        //    GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(SlashPrefabName);
        //    if (prefab == null)
        //    {
        //        throw new ArgumentException("Prefab '" + SlashPrefabName + "' not found in VMCSE AssetBundle!");
        //    }

        //    SlashObject.transform.localPosition = LocalPosition;
        //    SlashObject.GetComponent<NailSlash>().scale = LocalScale;
        //    SlashObject.GetComponent<NailSlash>().animName = AnimationName;
        //    SlashObject.GetComponent<NailSlash>().Awake();
        //    SlashObject.GetComponent<tk2dSpriteAnimator>().Library = SlashAnimator;



        //    PolygonCollider2D collider = SlashObject.GetComponent<PolygonCollider2D>();
        //    collider.points = prefab.GetComponent<PolygonCollider2D>().points;

        //    //To account for the increased anim size
        //    Helper.ScalePolygonCollider(collider, (float)Math.Sqrt(AnimationManager.SPRITESCALE));

        //    if (!CreateRedSlash) { return; }

        //    GameObject redCopy = GameObject.Instantiate(SlashObject);
        //    redCopy.transform.parent = SlashObject.transform;
        //    redCopy.transform.localPosition = RedSlashOffset;
        //    redCopy.name = AttackNames.REDSLASH;
        //    redCopy.GetComponent<DamageEnemies>().silkGeneration = HitSilkGeneration.None;
        //    redCopy.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.33f;
        //    redCopy.GetComponent<NailSlash>().hc = HeroController.instance;
        //    GameObject.Destroy(redCopy.GetComponent<NailSlashRecoil>());
        //}
    }
}
