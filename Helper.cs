using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VMCSE
{
    public static class Helper
    {
        public static GameObject Child(this GameObject go, string name)
        {
            return go.transform.Find(name).gameObject;
        }

        public static object CallPrivateMethod(object instance, string methodName, params object[] parameters)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var method = type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
                throw new MissingMethodException(type.FullName, methodName);

            return method.Invoke(instance, parameters);
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

        public static T GetPrivateField<T>(object instance, string fieldName)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new MissingFieldException($"Field '{fieldName}' not found in type '{type.FullName}'.");

            return (T)field.GetValue(instance);
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
    }
}
