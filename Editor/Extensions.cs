using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;


namespace Gokoukotori.FacialBlendShapeReplacer
{

    internal static class PathExtension
    {
        /// <summary>
        /// 絶対パスから Assets/ パスに変換する
        /// </summary>
        public static string AbsoluteToAssetsPath(this string self) => self.Replace("\\", "/").Replace(Application.dataPath, "Assets");
        /// <summary>
        /// Assets/ パスから絶対パスに変換する
        /// </summary>
        public static string AssetsToAbsolutePath(this string self)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return self.Replace("Assets", Application.dataPath).Replace("/", "\\");
#else
            return self.Replace("Assets", Application.dataPath);
#endif
        }
    }
    internal static class ComponentExtension
    {
        public static T GetComponentNullable<T>(this GameObject gameObject) where T : Component => gameObject.TryGetComponent<T>(out var component) ? component : null;
        public static T GetComponentNullable<T>(this Component component) where T : Component => GetComponentNullable<T>(component.gameObject);
    }
    internal static class GameObjectExtension
    {
        public static string GetFullPath(this GameObject obj) => GetFullPath(obj.transform);

        public static string GetFullPath(this Transform t)
        {
            var path = t.name;
            var parent = t.parent;
            while (parent)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }
    }
}