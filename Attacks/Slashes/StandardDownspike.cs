using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.Attacks.Slashes
{
    public class StandardDownspike
    {
        public GameObject SlashObject { get; set; }
        public string SlashPrefabName { get; set; }
        public string AnimationName { get; set; }
        public  GameObject SlashAnimatorObject { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 LocalScale { get; set; }

        public void SetupSlash()
        {
            // Load prefab by name
            GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(SlashPrefabName);
            if (prefab == null)
            {
                throw new ArgumentException("Prefab '" + SlashPrefabName + "' not found in VMCSE AssetBundle!");
            }

            SlashObject.transform.localPosition = LocalPosition;
            SlashObject.GetComponent<Downspike>().scale = LocalScale;
            SlashObject.GetComponent<Downspike>().animName = AnimationName;
            SlashObject.GetComponent<Downspike>().Awake();
            SlashObject.GetComponent<tk2dSpriteAnimator>().Library = SlashAnimatorObject.GetComponent<tk2dSpriteAnimator>().Library;
            SlashObject.GetComponent<PolygonCollider2D>().points = prefab.GetComponent<PolygonCollider2D>().points;
        }
    }
}
