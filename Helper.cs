using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using VMCSE.AnimationHandler;

namespace VMCSE
{
    internal static class Helper
    {
        public static GameObject Child(this GameObject go, string name)
        {
            return go.transform.Find(name).gameObject;
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

        /// <summary>
        /// Adds a prebuilt DamageEnemies to a gameobject. use after collider is already added. Customise at will afterwards.
        /// </summary>
        /// <param name="target">The gameobject to be added to</param>
        /// <returns>The created DamageEnemies component</returns>
        public static DamageEnemies AddDamageEnemies(this GameObject target)
        {
            //just because setting it up through code is so long
            DamageEnemies dmg = target.AddComponent<DamageEnemies>();
            dmg.useNailDamage = true;
            dmg.nailDamageMultiplier = 1f;
            dmg.magnitudeMult = 1f;
            dmg.damageMultPerHit = new float[0];
            dmg.sourceIsHero = true;
            dmg.isNailAttack = true;
            dmg.attackType = AttackTypes.Nail;
            dmg.corpseDirection = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.corpseMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.currencyMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.slashEffectOverrides = new GameObject[0];
            dmg.DealtDamage = new UnityEngine.Events.UnityEvent();
            dmg.damageFSMEvent = "";
            dmg.dealtDamageFSMEvent = "";
            dmg.stunDamage = 1f;
            dmg.Tinked = new UnityEngine.Events.UnityEvent();
            dmg.isHeroDamage = true;


            target.layer = (int)PhysLayers.HERO_ATTACK;

            return dmg;
        }

        /// <summary>
        /// Copies the PolygonCollider2D points from the given prefab name (within asset bundle) to the target gameobject.
        /// If there is no PolygonCollider2D, one is added. The created collider has isTrigger set to true.
        /// </summary>
        /// <param name="goToCopyInto">The gameobject to be copied to</param>
        /// <param name="prefabname">The name of the prefab within the asset bundle</param>
        public static void CopyPolygonColliderFromPrefab(this GameObject goToCopyInto, string prefabname)
        {
            GameObject prefab = ResourceLoader.bundle.LoadAsset<GameObject>(prefabname);
            if (prefab == null)
            {
                throw new ArgumentException("Prefab '" + prefabname + "' not found in VMCSE AssetBundle!");
            }


            PolygonCollider2D collider = goToCopyInto.GetComponent<PolygonCollider2D>();

            if (collider == null)
            {
                collider = goToCopyInto.AddComponent<PolygonCollider2D>();
                collider.isTrigger = true;
            }

            collider.points = prefab.GetComponent<PolygonCollider2D>().points;

            //To account for the increased anim size
            Helper.ScalePolygonCollider(collider, (float)Math.Sqrt(AnimationManager.SPRITESCALE));
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

        /// <summary>
        /// Adds a Tk2dWatchAnimationEventsV3 action to a state. Only use when hornet is created already.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="eventName">The name of the event to be played upon animation trigger event</param>
        public static void AddWatchAnimationActionTrigger(this FsmState state, string eventName)
        {
            FsmOwnerDefault hornetOwnerDefault = GetHornetOwnerDefault();

            state.AddAction(new Tk2dWatchAnimationEventsV3
            {
                gameObject = hornetOwnerDefault,
                animationTriggerEvent = FsmEvent.GetFsmEvent(eventName),
                fsmComponent = state.fsm.FsmComponent
            });
        }

        #endregion
    }
}
