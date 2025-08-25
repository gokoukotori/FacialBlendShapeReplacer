using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using System.IO;


namespace Gokoukotori.FacialBlendShapeReplacer
{
    public class FacialBlendShapeReplacerEditorWindow : EditorWindow
    {
        private const string GUID = "a6ffa1e5ab78f4e439b2977042dd913d";
        #region 外部ソース
        [SerializeField]
        private TextAsset jsonAsset;
        [SerializeField]
        private TextAsset avatarAsset;
        #endregion
        private List<Avatar> avatarList;

        private JsonData data;
        private readonly FacialBlendShapeReplacer replacer = new();
        private List<AnimationClip> animationClipList = new();
        private Avatar sourceAvatar;
        private Avatar targetAvatar;

        #region UXMLエレメント 
        private ObjectField _fieldSourceAvatar;
        private ObjectField _fieldTargetAvatar;
        private Label _fieldLabelSourceAvatar;
        private Label _fieldLabelTargetAvatar;
        private Button _bottonExecuteReplace;
        private ListView _listViewAnimationClip;
        #endregion
        //メニューの名前
        [MenuItem("Gokou/FacialBlendShapeReplacerEditorWindow")]

        public static void ShowWindow()
        {
            var window = CreateWindow<FacialBlendShapeReplacerEditorWindow>();
            window.Show();
        }
        public void OnEnable()
        {
            animationClipList = new();
            data = JsonUtility.FromJson<JsonData>(jsonAsset.text);
            avatarList = JsonUtility.FromJson<AvatarJson>(avatarAsset.text).avatar;
        }
        private void CreateGUI()
        {
            var uxmlPath = AssetDatabase.GUIDToAssetPath(GUID);
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            uxml.CloneTree(rootVisualElement);
            _fieldLabelSourceAvatar = rootVisualElement.Q<Label>("labelSourceAvatar");
            _fieldLabelTargetAvatar = rootVisualElement.Q<Label>("labelTargetAvatar");
            _fieldSourceAvatar = rootVisualElement.Q<ObjectField>("sourceAvatar");
            _fieldSourceAvatar.RegisterValueChangedCallback(x => OnChangeAvatar(x, _fieldLabelSourceAvatar, ref sourceAvatar));
            _fieldTargetAvatar = rootVisualElement.Q<ObjectField>("targetAvatar");
            _fieldTargetAvatar.RegisterValueChangedCallback(x => OnChangeAvatar(x, _fieldLabelTargetAvatar, ref targetAvatar));
            _listViewAnimationClip = rootVisualElement.Q<ListView>("animationClipListView");
            _listViewAnimationClip.makeItem += () =>
            {
                var field = new ObjectField
                {
                    objectType = typeof(AnimationClip)
                };
                field.RegisterValueChangedCallback(x =>
                {
                    if (field.userData is int index)
                    {
                        animationClipList[index] = (AnimationClip)x.newValue;
                    }
                });
                return field;
            };
            _listViewAnimationClip.bindItem += (element, index) =>
            {
                var field = (ObjectField)element;
                field.userData = index;
                field.SetValueWithoutNotify(animationClipList[index]);
            };
            _listViewAnimationClip.itemsSource = animationClipList;

            _bottonExecuteReplace = rootVisualElement.Q<Button>("executeReplace");
            _bottonExecuteReplace.clicked += () =>
            {
                // 保存先
                var dir = EditorUtility.SaveFolderPanel("Save", "Assets", "");
                if (string.IsNullOrEmpty(dir)) return;
                var sourceAvatar = data.avatarBlendShapeList.Where(x => x.guid == this.sourceAvatar.guid).First();
                var targetAvatar = data.avatarBlendShapeList.Where(x => x.guid == this.targetAvatar.guid).First();
                // 変換クリップ数分
                foreach (var item in animationClipList)
                {
                    var newClip = replacer.Execute(sourceAvatar, targetAvatar, item, data.excludeBlendShapeList);
                    AssetDatabase.CreateAsset(newClip, dir.AbsoluteToAssetsPath() + "/" + item.name + ".anim");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
            _bottonExecuteReplace.SetEnabled(false);
        }

        /// <summary>
        /// アバタープレハブ指定時の動作
        /// </summary>
        /// <param name="evt">ChangeEvent</param>
        /// <param name="_fieldLabelAvatarName">Label</param>
        /// <param name="avatar">アバター情報</param>
        private void OnChangeAvatar(ChangeEvent<UnityEngine.Object> evt, Label _fieldLabelAvatarName, ref Avatar avatar)
        {
            if (evt.newValue == null) return;
            if (evt.newValue is not VRCAvatarDescriptor avatarDescriptor) return;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(avatarDescriptor.gameObject))
            {
                var avatarPrefabs = new List<GameObject>() { avatarDescriptor.gameObject };
                var current = avatarDescriptor.gameObject;
                while (true)
                {
                    var parent = PrefabUtility.GetCorrespondingObjectFromSource(current);
                    if (parent == null || parent == current) break;
                    avatarPrefabs.Add(parent);
                    current = parent;
                }
                avatarPrefabs.Reverse();
                foreach (var avatarinfo in avatarList)
                {
                    if (avatarPrefabs.Select(avatarPrefab => AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(avatarPrefab)).ToString()).Any(prefabGuid => prefabGuid == avatarinfo.guid))
                    {
                        _fieldLabelAvatarName.text = avatarinfo.name;
                        avatar = avatarinfo;
                        break;
                    }
                }
                _bottonExecuteReplace.SetEnabled(sourceAvatar is not null || targetAvatar is not null || sourceAvatar.guid != targetAvatar.guid);
            }
        }
    }
}