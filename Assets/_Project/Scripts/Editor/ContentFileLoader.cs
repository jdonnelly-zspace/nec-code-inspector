using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NECInspector.Editor
{
    /// <summary>
    /// Reads and validates a content JSON file for the generators. Content lives in
    /// Assets/_Project/Content as data; an invalid file is reported and nothing is imported from it.
    /// </summary>
    internal static class ContentFileLoader
    {
        public static T Load<T>(string path, Func<T, List<string>> validate) where T : class
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"[NEC Inspector] Content file not found: {path}");
                return null;
            }

            T data;
            try
            {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"[NEC Inspector] {path}: could not parse: {e.Message}");
                return null;
            }

            var errors = validate(data);
            if (errors.Count > 0)
            {
                foreach (string error in errors)
                    Debug.LogError($"[NEC Inspector] {path}: {error}");
                Debug.LogError($"[NEC Inspector] {path} has {errors.Count} problem(s); nothing was imported from it.");
                return null;
            }

            return data;
        }

        public static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
                AssetDatabase.CreateFolder(parent, name);
        }

        public static T ParseEnum<T>(string name) where T : struct
        {
            return (T)Enum.Parse(typeof(T), name);
        }
    }
}
