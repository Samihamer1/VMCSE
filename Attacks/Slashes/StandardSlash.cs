using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;

namespace VMCSE.Attacks.Slashes
{
    public class StandardSlash
    {
        public GameObject SlashObject { get; set; }
        public string SlashPrefabName { get; set; }
        public string AnimationName { get; set; }
        public  tk2dSpriteAnimation? SlashAnimator { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 LocalScale { get; set; }
        public bool CreateRedSlash { get; set; }
        public Vector3 RedSlashOffset { get; set; }

        public void SetupSlash()
        {
            // Load prefab by name
            GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(SlashPrefabName);
            if (prefab == null)
            {
                throw new ArgumentException("Prefab '" + SlashPrefabName + "' not found in VMCSE AssetBundle!");
            }

            SlashObject.transform.localPosition = LocalPosition;
            SlashObject.GetComponent<NailSlash>().scale = LocalScale;
            SlashObject.GetComponent<NailSlash>().animName = AnimationName;
            SlashObject.GetComponent<NailSlash>().Awake();
            SlashObject.GetComponent<tk2dSpriteAnimator>().Library = SlashAnimator;



            PolygonCollider2D collider = SlashObject.GetComponent<PolygonCollider2D>();
            collider.points = prefab.GetComponent<PolygonCollider2D>().points;

            //To account for the increased anim size
            Helper.ScalePolygonCollider(collider, (float)Math.Sqrt(AnimationManager.SPRITESCALE));

            if (!CreateRedSlash) { return; }

            GameObject redCopy = GameObject.Instantiate(SlashObject);
            redCopy.transform.parent = SlashObject.transform;
            redCopy.transform.localPosition = RedSlashOffset;
            redCopy.name = "RedSlash";
            redCopy.GetComponent<DamageEnemies>().silkGeneration = HitSilkGeneration.None;
            redCopy.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.33f;
            redCopy.GetComponent<NailSlash>().hc = HeroController.instance;
            GameObject.Destroy(redCopy.GetComponent<NailSlashRecoil>());
        }
    }
}
