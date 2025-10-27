using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.CanvasHandler
{
    internal class CanvasObject : MonoBehaviour
    {
        public SpriteRenderer? renderer;

        public void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.layer = HudCanvas.instance.gameObject.layer;

            renderer = gameObject.AddComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite? sprite)
        {
            if (renderer == null) { return; }
            renderer.sprite = sprite;
        }
    }
}
