using System;
using System.Collections.Generic;
using System.Text;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static HutongGames.PlayMaker.Actions.BlockEvents;
using static Mono.Security.X509.PKCS12.DeriveBytes;
using static PassColour;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.Android;
using UnityEngine.InputForUI;

namespace VMCSE.AnimationHandler
{
    public static class Tk2dHelper
    {
        //The functions within the tk2d region are licensed under the following:
        //(Minor changes were made from the original)

        /*MIT License

        Copyright(c) 2022 RedFrog6002

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files(the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
                FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.*/
        #region tk2d
        public static tk2dSpriteCollectionData CreateTk2dSpriteCollection(Texture texture, string[] names, Rect[] rects, Vector2[] anchors, GameObject go)
        {
            if (texture != null)
            {
                UnityEngine.Object.DontDestroyOnLoad(texture);
                var spriteCollection = CreateFromTexture(go, texture, new tk2dSpriteCollectionSize() { }, new Vector2((float)texture.width, (float)texture.height), names, rects, null, anchors, new bool[6]);
                string text = "SpriteFromTexture " + texture.name;
                spriteCollection.spriteCollectionName = text;
                spriteCollection.spriteDefinitions[0].material.name = text;
                spriteCollection.spriteDefinitions[0].material.hideFlags = (HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
                UnityEngine.Object.DontDestroyOnLoad(spriteCollection);
                return spriteCollection;
            }

            throw new NullReferenceException();
        }

        public static Material CreateMaterialFromTextureTk2d(Texture tex, bool setdontdestroy = true)
        {
            Material mat = new Material(Shader.Find("tk2d/BlendVertexColor"));
            mat.mainTexture = tex;
            if (setdontdestroy)
            {
                UnityEngine.Object.DontDestroyOnLoad(mat);
                UnityEngine.Object.DontDestroyOnLoad(tex);
            }

            return mat;
        }

        public static void SetCollection(this tk2dSpriteAnimation animation, tk2dSpriteCollectionData collection)
        {
            foreach (tk2dSpriteAnimationClip clip in animation.clips)
                foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                    frame.spriteCollection = collection;
        }

        public static void SetCollection(this tk2dSpriteAnimationClip clip, tk2dSpriteCollectionData collection)
        {
            foreach (tk2dSpriteAnimationFrame frame in clip.frames)
                frame.spriteCollection = collection;
        }

        public static tk2dSpriteCollectionData CreateFromTexture(GameObject parentObject, Texture texture, tk2dSpriteCollectionSize size, Vector2 textureDimensions, string[] names, Rect[] regions, Rect[] trimRects, Vector2[] anchors, bool[] rotated)
        {
            tk2dSpriteCollectionData tk2dSpriteCollectionData = parentObject.AddComponent<tk2dSpriteCollectionData>();
            tk2dSpriteCollectionData.material = CreateMaterialFromTextureTk2d(texture);
            tk2dSpriteCollectionData.materials = new Material[]
            {
                tk2dSpriteCollectionData.material
            };
            tk2dSpriteCollectionData.textures = new Texture[]
            {
                texture
            };
            float scale = AnimationManager.SPRITESCALE * size.OrthoSize / size.TargetHeight;
            Rect trimRect = new Rect(0f, 0f, 0f, 0f);
            tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                if (trimRects != null)
                {
                    trimRect = trimRects[i];
                }
                else
                {
                    trimRect.Set(0f, 0f, regions[i].width, regions[i].height);
                }

                tk2dSpriteCollectionData.spriteDefinitions[i] = CreateDefinitionForRegionInTexture(names[i], textureDimensions, scale, regions[i], trimRect, anchors[i], false);
            }

            foreach (tk2dSpriteDefinition tk2dSpriteDefinition in tk2dSpriteCollectionData.spriteDefinitions)
            {
                tk2dSpriteDefinition.material = tk2dSpriteCollectionData.material;
            }

            return tk2dSpriteCollectionData;
        }

        public static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(string name, Vector2 textureDimensions, float scale, Rect uvRegion, Rect trimRect, Vector2 anchor, bool rotated)
        {
            float height = uvRegion.height;
            float width = uvRegion.width;
            float x = textureDimensions.x;
            float y = textureDimensions.y;
            tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
            tk2dSpriteDefinition.flipped = ((!rotated) ? tk2dSpriteDefinition.FlipMode.None : tk2dSpriteDefinition.FlipMode.TPackerCW);
            tk2dSpriteDefinition.extractRegion = false;
            tk2dSpriteDefinition.name = name;
            tk2dSpriteDefinition.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
            Vector2 vector = new Vector2(0.001f, 0.001f);
            Vector2 vector2 = new Vector2((uvRegion.x + vector.x) / x, 1f - (uvRegion.y + uvRegion.height + vector.y) / y);
            Vector2 vector3 = new Vector2((uvRegion.x + uvRegion.width - vector.x) / x, 1f - (uvRegion.y - vector.y) / y);
            Vector2 a = new Vector2(trimRect.x - anchor.x, -trimRect.y + anchor.y);
            if (rotated)
            {
                a.y -= width;
            }

            a *= scale;
            Vector3 a2 = new Vector3(-anchor.x * scale, anchor.y * scale, 0f);
            Vector3 vector4 = a2 + new Vector3(trimRect.width * scale, -trimRect.height * scale, 0f);
            Vector3 a3 = new Vector3(0f, -height * scale, 0f);
            Vector3 vector5 = a3 + new Vector3(width * scale, height * scale, 0f);
            if (rotated)
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(-vector5.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, a3.x + a.y, 0f),
                    new Vector3(-vector5.y + a.x, vector5.x + a.y, 0f),
                    new Vector3(-a3.y + a.x, vector5.x + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector3.y),
                    new Vector2(vector3.x, vector2.y)
                };
            }
            else
            {
                tk2dSpriteDefinition.positions = new Vector3[]
                {
                    new Vector3(a3.x + a.x, a3.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, a3.y + a.y, 0f),
                    new Vector3(a3.x + a.x, vector5.y + a.y, 0f),
                    new Vector3(vector5.x + a.x, vector5.y + a.y, 0f)
                };
                tk2dSpriteDefinition.uvs = new Vector2[]
                {
                    new Vector2(vector2.x, vector2.y),
                    new Vector2(vector3.x, vector2.y),
                    new Vector2(vector2.x, vector3.y),
                    new Vector2(vector3.x, vector3.y)
                };
            }

            tk2dSpriteDefinition.normals = new Vector3[0];
            tk2dSpriteDefinition.tangents = new Vector4[0];
            tk2dSpriteDefinition.indices = new int[]
            {
                0,
                3,
                1,
                2,
                3,
                0
            };
            Vector3 b = new Vector3(a2.x, vector4.y, 0f);
            Vector3 a4 = new Vector3(vector4.x, a2.y, 0f);
            tk2dSpriteDefinition.boundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.untrimmedBoundsData = new Vector3[]
            {
                (a4 + b) / 2f,
                a4 - b
            };
            tk2dSpriteDefinition.texelSize = new Vector2(scale, scale);
            return tk2dSpriteDefinition;
        }

        #endregion
    }
}
