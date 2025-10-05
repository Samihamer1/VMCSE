using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VMCSE.Components
{
    public class StyleRankUI : MonoBehaviour
    {
        private Vector2 origSize;
        public void Start()
        {
            //renderer = gameObject.AddComponent<SpriteRenderer>();            
        }

        private float getScaledUnit(float toScale)
        {
            return (toScale / 2) / 100;
        }

        public void SetSprite(string path)
        {
            Texture2D texture = ResourceLoader.LoadTexture2D(path);

            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero, 100.0f);

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            gameObject.transform.localPosition = new Vector2(-getScaledUnit(texture.width), -getScaledUnit(texture.height));

            renderer.sprite = sprite;
            origSize = renderer.size;

            renderer.tileMode = SpriteTileMode.Continuous;
            renderer.drawMode = SpriteDrawMode.Tiled;
        }

        public void SetFillAmount(float fraction)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(origSize.x, origSize.y * fraction);
        }
    }
}
