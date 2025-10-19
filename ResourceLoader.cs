using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VMCSE
{
    internal static class ResourceLoader
    {

        public static AssetBundle bundle;

        #region png paths

        private static readonly Dictionary<string, string> imagePaths = new()
        {
            ["Style1BG"] = "VMCSE.Resources.UI.Style.Style1BG.png",
            ["Style1FG"] = "VMCSE.Resources.UI.Style.Style1FG.png",
            ["Style2BG"] = "VMCSE.Resources.UI.Style.Style2BG.png",
            ["Style2FG"] = "VMCSE.Resources.UI.Style.Style2FG.png",
            ["Style3BG"] = "VMCSE.Resources.UI.Style.Style3BG.png",
            ["Style3FG"] = "VMCSE.Resources.UI.Style.Style3FG.png",
            ["Style4BG"] = "VMCSE.Resources.UI.Style.Style4BG.png",
            ["Style4FG"] = "VMCSE.Resources.UI.Style.Style4FG.png",
            ["Style5BG"] = "VMCSE.Resources.UI.Style.Style5BG.png",
            ["Style5FG"] = "VMCSE.Resources.UI.Style.Style5FG.png",
            ["Style6BG"] = "VMCSE.Resources.UI.Style.Style6BG.png",
            ["Style6FG"] = "VMCSE.Resources.UI.Style.Style6FG.png",
            ["Style7BG"] = "VMCSE.Resources.UI.Style.Style7BG.png",
            ["Style7FG"] = "VMCSE.Resources.UI.Style.Style7FG.png",

            ["DriveIcon"] = "VMCSE.Resources.UI.Icons.DriveIcon.png",
            ["DriveIconGlow"] = "VMCSE.Resources.UI.Icons.DriveIconGlow.png",
            ["HorizontalSkillCircle"] = "VMCSE.Resources.UI.Icons.HorizontalSkill.png",
            ["HorizontalSkillCircleGlow"] = "VMCSE.Resources.UI.Icons.HorizontalSkillGlow.png",
            ["ReactorIcon"] = "VMCSE.Resources.UI.Icons.ReactorIcon.png",
            ["ReactorIconGlow"] = "VMCSE.Resources.UI.Icons.ReactorIconGlow.png",
            ["RoundTripIcon"] = "VMCSE.Resources.UI.Icons.RoundTripIcon.png",
            ["RoundTripIconGlow"] = "VMCSE.Resources.UI.Icons.RoundTripIconGlow.png",
        };

        #endregion 

        private static readonly Dictionary<Type, Dictionary<string, Object>> Assets = new();

        public static void Init()
        {
            LoadBundle();
            LoadSprites();
        }

        private static void LoadSpriteToAssets(string name, string path)
        {
            var type = typeof(Sprite);

            Sprite sprite = LoadSprite(path);
            if (sprite == null)
            {
                VMCSE.Instance.LogError("Sprite " + name + " not able to load - path incorrect?");
                return;
            }

            Object.DontDestroyOnLoad(sprite);

            if (Assets.ContainsKey(type))
            {
                var dictionary = Assets[type];
                if (dictionary == null) { Assets.Add(type, new Dictionary<string, Object>()); dictionary = Assets[type]; }

                if (dictionary.ContainsKey(name)) { VMCSE.Instance.LogError("Sprite " + name + " already exists."); return; };

                Assets[type].Add(name, sprite);

            } else
            {
                Assets.Add(type, new Dictionary<string, Object> { [name] = sprite });
            }

        }

        private static void LoadSprites()
        {
            foreach (string key in imagePaths.Keys)
            {
                var path = imagePaths[key];
                LoadSpriteToAssets(key, path);
            }
        }

        internal static T? LoadAsset<T>(string assetName) where T : Object
        {
            Type type = typeof(T);

            if (Assets.ContainsKey(type))
            {
                var dictionary = Assets[type];
                if (dictionary == null) { VMCSE.Instance.LogError("Asset " + assetName + " does not exist, and the dictionary that stores it is null."); return null; }

                if (!dictionary.ContainsKey(assetName)) { VMCSE.Instance.LogError("Asset " + assetName + " does not exist."); }

                var asset = dictionary[assetName];
                if (asset == null) { VMCSE.Instance.LogError("Asset " + assetName + " found, but it is null."); }

                return (T?)asset;

            } else
            {
                VMCSE.Instance.LogError("Asset " + assetName + " does not exist, nor does the dictionary that stores it.");
                return null;
            }
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
