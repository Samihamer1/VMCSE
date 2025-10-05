using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using VMCSE.AnimationHandler;

namespace VMCSE
{
    public static class Helper
    {
        public static GameObject Child(this GameObject go, string name)
        {
            return go.transform.Find(name).gameObject;
        }

        public static void CopyPolygonColliderFromPrefab(this GameObject goToCopyInto, string prefabname)
        {
            GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(prefabname);
            if (prefab == null)
            {
                throw new ArgumentException("Prefab '" + prefabname + "' not found in VMCSE AssetBundle!");
            }


            PolygonCollider2D collider = goToCopyInto.GetComponent<PolygonCollider2D>();
            collider.points = prefab.GetComponent<PolygonCollider2D>().points;

            //To account for the increased anim size
            Helper.ScalePolygonCollider(collider, (float)Math.Sqrt(AnimationManager.SPRITESCALE));
        }
        public static void SetPrivateField<T>(object instance, string fieldName, T value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new MissingFieldException($"Field '{fieldName}' not found in type '{type.FullName}'.");

            field.SetValue(instance, value);
        }

        public static FsmEvent CreateFsmEvent(this PlayMakerFSM fsm, string eventName)
        {
            var @new = new FsmEvent(eventName);

            fsm.Fsm.Events = fsm.Fsm.Events.Append(@new).ToArray();

            return @new;
        }
        private static FsmEvent GetFsmEvent(this PlayMakerFSM fsm, string eventName)
        {
            foreach (FsmEvent Event in fsm.Fsm.Events)
            {
                if (Event.Name == eventName) { return Event; }

            }
            

            return null;
        }

        public static T GetAction<T>(this FsmState state, int index) where T : FsmStateAction
        {
            FsmStateAction act = state.Actions[index];

            return (T)act;
        }

        public static T GetAction<T>(this FsmState state) where T : FsmStateAction
        {
            return state.Actions.OfType<T>().First();
        }

        public static void SetPrivateStaticField<T>(Type type, string fieldName, T value)
        {
            var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingFieldException(type.FullName, fieldName);

            field.SetValue(null, value); // null because it's static
        }

        public static T GetPrivateStaticField<T>(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
                throw new MissingFieldException(type.FullName, fieldName);

            return (T)field.GetValue(null); // null because it's static
        }

        #region Gameobject based functions

        public static void ScalePolygonCollider(PolygonCollider2D collider, float multiplier)
        {
            if (collider == null) return;

            Vector2[] points = collider.points;

            // Find centroid of all points
            Vector2 centroid = Vector2.zero;
            foreach (var p in points)
                centroid += p;
            centroid /= points.Length;

            // Scale around centroid
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 offset = points[i] - centroid;
                points[i] = centroid + offset * multiplier;
            }

            collider.points = points;
        }

        #endregion

        #region Fsm functions for common cases

        public static FsmOwnerDefault GetHornetOwnerDefault()
        {
            PlayMakerFSM fsm = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (fsm == null) { throw new Exception("VMCSE GetHornetOwnerDefault should only be called when Hornet is active"); };

            FsmOwnerDefault hornetOwnerDefault = fsm.GetAction<Tk2dWatchAnimationEventsV3>("Rpr Downslash Antic", 5).gameObject;

            return hornetOwnerDefault;
        }

        /// <summary>
        /// Adds a Tk2dPlayAnimation action to a state. Only use when hornet is created already.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="animLibName">The name of the animation library</param>
        /// <param name="clipName">The name of the animation clip</param>
        public static void AddAnimationAction(this FsmState state, string animLibName, string clipName)
        {
            FsmOwnerDefault hornetOwnerDefault = GetHornetOwnerDefault();
            state.AddAction(new Tk2dPlayAnimation
            {
                animLibName = animLibName,
                heroAnim = HeroController.instance.animCtrl,
                clipName = clipName,
                fsmComponent = state.fsm.FsmComponent,
                gameObject = hornetOwnerDefault
            });
        }

        /// <summary>
        /// Adds a Tk2dWatchAnimationEventsV3 action to a state. Only use when hornet is created already.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="eventName">The name of the event to be played upon animation completion</param>
        public static void AddWatchAnimationAction(this FsmState state, string eventName)
        {
            FsmOwnerDefault hornetOwnerDefault = GetHornetOwnerDefault();

            state.AddAction(new Tk2dWatchAnimationEventsV3
            {
                gameObject = hornetOwnerDefault,
                animationCompleteEvent = FsmEvent.GetFsmEvent(eventName),
                fsmComponent = state.fsm.FsmComponent
            });
        }

        #endregion
    }
}
