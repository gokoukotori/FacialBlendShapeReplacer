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
    public static class StringExtension
    {
        /// <summary>
        /// 絶対パスから Assets/ パスに変換する
        /// </summary>
        public static string AbsoluteToAssetsPath(this string self)
        {
            return self.Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }
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
}