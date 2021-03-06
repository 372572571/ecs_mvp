using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVP.Framework.Core.States
{
    public static class TypeUtil
    {
        public static UnityEngine.Object[] ListUpUnityObjects(Type type, object value)
        {
            var list = new List<UnityEngine.Object>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (fields.Length < 1) return new UnityEngine.Object[0];

            foreach (var field in fields)
            {
                if (field.IsPublic || field.GetCustomAttribute(typeof(SerializeField)) != null)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                        list.Add(field.GetValue(value) as UnityEngine.Object);
                }
            }

            return list.ToArray();
        }

        public static bool RestoreUnityObjects(Type type, object value, UnityEngine.Object[] references)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fields.Length < 1) return true;

            var i = 0;
            foreach (var field in fields)
            {
                if (field.IsPublic || field.GetCustomAttribute(typeof(SerializeField)) != null)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                    {
                        if (i >= references.Length)
                            throw new ArgumentException("Not matched objects references size and field objects");

                        if (references[i] != null && field.FieldType.IsInstanceOfType(references[i]))
                        {
                            field.SetValue(value, references[i]);
                        }
                        else
                        {
                            Log.State.W($"Cannot restore field: {field.Name}");
                        }

                        i++;
                    }
                }
            }

            return i > 0;
        }
    }
}