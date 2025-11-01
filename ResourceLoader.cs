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

        public static AssetBundle? bundle;

        #region png paths

        private static readonly Dictionary<string, string> imagePaths = new()
        {
            ["DriveIcon"] = "VMCSE.Resources.UI.Icons.DriveIcon.png",
            ["DriveIconGlow"] = "VMCSE.Resources.UI.Icons.DriveIconGlow.png",
            ["HorizontalSkillCircle"] = "VMCSE.Resources.UI.Icons.HorizontalSkill.png",
            ["HorizontalSkillCircleGlow"] = "VMCSE.Resources.UI.Icons.HorizontalSkillGlow.png",
            ["ReactorIcon"] = "VMCSE.Resources.UI.Icons.ReactorIcon.png",
            ["ReactorIconGlow"] = "VMCSE.Resources.UI.Icons.ReactorIconGlow.png",
            ["RoundTripIcon"] = "VMCSE.Resources.UI.Icons.RoundTripIcon.png",
            ["RoundTripIconGlow"] = "VMCSE.Resources.UI.Icons.RoundTripIconGlow.png",
        };


        //Tiled images are those with the pivot point Vector2.zero, so that they can be used with fillAmount
        private static readonly Dictionary<string, string> tiledImagePaths = new()
        {
            ["DevilTriggerFG"] = "VMCSE.Resources.UI.Trigger.FG.png",
            ["DevilTriggerBG"] = "VMCSE.Resources.UI.Trigger.BG.png",
            ["DevilTriggerFGv2"] = "VMCSE.Resources.UI.Trigger.FGv2.png",
            ["DevilTriggerBGv2"] = "VMCSE.Resources.UI.Trigger.BGv2.png",
            ["DevilTriggerSplitBar"] = "VMCSE.Resources.UI.Trigger.SplitBar.png",
            ["Style1FG"] = "VMCSE.Resources.UI.Style.Style1FG.png",
            ["Style2FG"] = "VMCSE.Resources.UI.Style.Style2FG.png",
            ["Style3FG"] = "VMCSE.Resources.UI.Style.Style3FG.png",
            ["Style4FG"] = "VMCSE.Resources.UI.Style.Style4FG.png",
            ["Style5FG"] = "VMCSE.Resources.UI.Style.Style5FG.png",
            ["Style6FG"] = "VMCSE.Resources.UI.Style.Style6FG.png",
            ["Style7FG"] = "VMCSE.Resources.UI.Style.Style7FG.png",
            ["Style1BG"] = "VMCSE.Resources.UI.Style.Style1BG.png",
            ["Style2BG"] = "VMCSE.Resources.UI.Style.Style2BG.png",
            ["Style3BG"] = "VMCSE.Resources.UI.Style.Style3BG.png",
            ["Style4BG"] = "VMCSE.Resources.UI.Style.Style4BG.png",
            ["Style5BG"] = "VMCSE.Resources.UI.Style.Style5BG.png",
            ["Style6BG"] = "VMCSE.Resources.UI.Style.Style6BG.png",
            ["Style7BG"] = "VMCSE.Resources.UI.Style.Style7BG.png",
        };

        #endregion 

        private static readonly Dictionary<Type, Dictionary<string, Object>> Assets = new();

        public static void Init()
        {
            LoadSprites();
        }

        /// <summary>
        /// Resource initialisation on hero loading. Used because for some reason, unloading a save deletes the asset bundle regardless.
        /// </summary>
        public static void InitOnHeroLoad()
        {
            LoadBundle();
        }

        private static void LoadSpriteToAssets(string name, string path)
        {
            Sprite sprite = LoadSprite(path);
            if (sprite == null)
            {
                VMCSE.Instance.LogError("Sprite " + name + " not able to load - path incorrect?");
                return;
            }

            sprite.name = name;

            LoadSpriteToAssets(name, sprite);
        }

        private static void LoadTiledSpriteToAssets(string name, string path)
        {
            Sprite sprite = LoadTiledSprite(path);
            if (sprite == null)
            {
                VMCSE.Instance.LogError("Sprite " + name + " not able to load - path incorrect?");
                return;
            }

            sprite.name = name;

            LoadSpriteToAssets(name, sprite);
        }

        private static void LoadSpriteToAssets(string name, Sprite sprite)
        {
            var type = typeof(Sprite);

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

            foreach (string key in tiledImagePaths.Keys)
            {
                var path = tiledImagePaths[key];
                LoadTiledSpriteToAssets(key, path);
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

            if (bundle != null)
            {
                bundle.Unload(false);
            }

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

        public static Sprite LoadTiledSprite(string path)
        {
            Texture2D texture = LoadTexture2D(path);
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero, 100.0f);
        }
    }
}
