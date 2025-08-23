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
    public class FacialBlendShapeReplacerEditorWindow : EditorWindow
    {
        [SerializeField]
        private TextAsset jsonAsset;
        private JsonData data;
        private GUIContent[] avatarSelectbox;
        private int sourceAvatarIndex = 0;
        private int targetAvatarIndex = 1;
        private List<AnimationClip> animationClipList = new();
        private ReorderableList reorderableList;
        private readonly FacialBlendShapeReplacer replacer = new();
        //メニューの名前
        [MenuItem("Gokou/FacialBlendShapeReplacerEditorWindow")]

        public static void ShowWindow()
        {
            GetWindow<FacialBlendShapeReplacerEditorWindow>("FacialBlendShapeReplacerEditorWindow");
        }

        public void OnEnable()
        {
            animationClipList = new();
            data = JsonUtility.FromJson<JsonData>(jsonAsset.text);
            reorderableList = new(animationClipList, typeof(AnimationClip), true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "アニメーションクリップ");
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index < animationClipList.Count)
                {
                    animationClipList[index] = (AnimationClip)EditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        animationClipList[index],
                        typeof(AnimationClip),
                        false
                    );
                }
            };
            avatarSelectbox = data.avatarBlendShapeList.Select(x => new GUIContent(x.avatarName)).ToArray();
        }
        public void OnGUI()
        {
            GUILayout.Label("FacialBlendShapeReplacer", EditorStyles.boldLabel);
            sourceAvatarIndex = EditorGUILayout.Popup(
                label: new("変換元アバター"),
                selectedIndex: sourceAvatarIndex,
                displayedOptions: avatarSelectbox
            );
            targetAvatarIndex = EditorGUILayout.Popup(
                label: new("変換先アバター"),
                selectedIndex: targetAvatarIndex,
                displayedOptions: avatarSelectbox
            );
            reorderableList.DoLayoutList();
            if (animationClipList.Count != 0)
            {
                if (GUILayout.Button("変換"))
                {
                    // 保存先
                    var dir = EditorUtility.SaveFolderPanel("Save", "Assets", "");
                    if (string.IsNullOrEmpty(dir)) return;
                    var sourceAvatar = data.avatarBlendShapeList[sourceAvatarIndex];
                    var targetAvatar = data.avatarBlendShapeList[targetAvatarIndex];
                    // 変換クリップ数分
                    foreach (var item in animationClipList)
                    {
                        var newClip = replacer.Execute(sourceAvatar, targetAvatar, item, data.excludeBlendShapeList);
                        AssetDatabase.CreateAsset(newClip, dir.AbsoluteToAssetsPath() + "/" + item.name + ".anim");
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            /*
            targetObject = (GameObject)EditorGUILayout.ObjectField("服のルート", targetObject, typeof(GameObject), true);
            SkinnedMeshRenderer[] renderers = targetObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                string names = "";
                for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                {
                    names += renderer.sharedMesh.GetBlendShapeName(i) + "\n";
                }
            }*/

        }
        GameObject targetObject;
    }
}