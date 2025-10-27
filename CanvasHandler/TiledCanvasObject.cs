using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.CanvasHandler
{
    internal class TiledCanvasObject : CanvasObject
    {
        private Vector2 originalSpriteSize;
        private new void Awake()
        {
            base.Awake();

            if (renderer == null) { return; }

            renderer.tileMode = SpriteTileMode.Continuous;
            renderer.drawMode = SpriteDrawMode.Tiled;
        }

        public new void SetSprite(Sprite? sprite)
        {
            if (renderer == null) { return; }
            renderer.sprite = sprite;

            originalSpriteSize = renderer.size;
        }

        public void SetFillAmount(float fillX, float fillY)
        {
            if (renderer == null) { return; }
            if (originalSpriteSize == null) { return; }

            renderer.size = new Vector2(originalSpriteSize.x * fillX, originalSpriteSize.y * fillY);
        }
    }
}
