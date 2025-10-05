using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VMCSE
{
    internal static class ResourceLoader
    {

        public static AssetBundle bundle;

        public static void Init()
        {
            LoadBundle();
        }

        private static void LoadBundle()
        {
            string resourceName = "VMCSE.Resources.vmcsebundle.bundle";

            using Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                VMCSE.Instance.LogError("Bundle not found");
                return;
            }

            // Read stream into byte[]
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            byte[] bundleData = ms.ToArray();

            // Load AssetBundle from memory
             bundle = AssetBundle.LoadFromMemory(bundleData);
            if (bundle == null)
            {
                VMCSE.Instance.LogError("Failed to load bundle");
                return;
            }
        }
        public static Texture2D LoadTexture2D(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            MemoryStream memoryStream = new((int)stream.Length);
            stream.CopyTo(memoryStream);
            stream.Close();
            var bytes = memoryStream.ToArray();
            memoryStream.Close();

            var texture2D = new Texture2D(1, 1);
            _ = texture2D.LoadImage(bytes);
            texture2D.anisoLevel = 0;

            return texture2D;
        }

        public static Sprite LoadSprite(string path)
        {
            Texture2D texture = LoadTexture2D(path);
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.one / 2, 100.0f);
        }
    }
}
