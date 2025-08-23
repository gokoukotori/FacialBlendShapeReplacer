using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Gokoukotori.FacialBlendShapeReplacer
{
    public class FacialBlendShapeReplacer
    {
        /// <summary>
        /// ソースからターゲットへブレンドシェイプを変換します。
        /// </summary>
        /// <param name="sourceAvatar">変換元アバター</param>
        /// <param name="targetAvatar">変換元アバター</param>
        /// <param name="targetClip">変換対象アニメーションクリップ</param>
        /// <param name="excludeBlendShapeList">除外するブレンドシェイプ</param>
        /// <returns>変換されたアニメーションクリップ</returns>
        public AnimationClip Execute(AvatarBlendShape sourceAvatar, AvatarBlendShape targetAvatar, AnimationClip targetClip, IReadOnlyList<string> excludeBlendShapeList)
        {

            var newClip = new AnimationClip();
            var bindings = AnimationUtility.GetCurveBindings(targetClip);
            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(targetClip, binding);
                // bodyのblendShapeだけ対応
                // それ以外は何もせず転記
                if (binding.type == typeof(SkinnedMeshRenderer) && Regex.IsMatch(binding.propertyName, @"^(blendShape\.).*") && binding.path == "Body")
                {
                    // 対象外は除外
                    if (excludeBlendShapeList.Count(x => ("blendShape." + x) == binding.propertyName) != 0) continue;
                    if (sourceAvatar.excludeBlendShapeList.Count(x => ("blendShape." + x) == binding.propertyName) != 0) continue;

                    // 名称がイコールとなるものはjsonに記載しない
                    // つまりそのまま転記する
                    var targetBlendShapes = sourceAvatar.sourceBlendShapeMap.Find(x => ("blendShape." + x.source) == binding.propertyName);
                    if (targetBlendShapes is null)
                    {
                        var newClipCurve = AnimationUtility.GetEditorCurve(newClip, binding);
                        // targetBlendShapeMapで複数ブレンドシェイプ指定された場合、newCurve側のキーが重複して上書きされる可能性がある
                        // なので重複した場合は値を足す動作に変更する
                        AnimationUtility.SetEditorCurve(newClip, binding, newClipCurve is null ? curve : new AnimationCurve(MergeFrame(curve, newClipCurve, 1f)));
                        continue;
                    }
                    // 複数ブレンドシェイプに分かれる場合の対応
                    foreach (var universalBlendShape in targetBlendShapes.target)
                    {
                        var blendShape = targetAvatar.targetBlendShapeMap.Find(x => x.universalBlendShape == universalBlendShape.universalBlendShape);
                        var newBinding = new EditorCurveBinding
                        {
                            path = binding.path,
                            type = binding.type,
                            propertyName = "blendShape." + blendShape.target
                        };
                        var newClipCurve = AnimationUtility.GetEditorCurve(newClip, newBinding);
                        // targetBlendShapeMapで複数ブレンドシェイプ指定された場合、newCurve側のキーが重複して上書きされる可能性がある
                        // なので重複した場合は値を足す動作に変更する
                        AnimationUtility.SetEditorCurve(newClip, newBinding, newClipCurve is null ? curve : new AnimationCurve(MergeFrame(curve, newClipCurve, universalBlendShape.ratio)));
                    }
                }
                else
                {
                    AnimationUtility.SetEditorCurve(newClip, binding, curve);
                }
            }
            return newClip;
        }

        /// <summary>
        /// Keyframeをマージします。
        /// </summary>
        /// <param name="curve">既存アニメーションカーブ</param>
        /// <param name="newCurve">新規アニメーションカーブ</param>
        /// <param name="ratio">重み付け用</param>
        /// <returns>マージ後のKeyframe</returns>
        private Keyframe[] MergeFrame(AnimationCurve curve, AnimationCurve newCurve, float ratio)
        {
            // 複数ブレンドシェイプに分かれる場合の重み付け対応
            var newFrameTime = newCurve.keys.Select(x => x.time).ToList();
            newFrameTime.AddRange(curve.keys.Select(x => x.time));
            newFrameTime = newFrameTime.Distinct().OrderBy(x => x).ToList();
            var newKeyframe = new Keyframe[newFrameTime.Count];
            for (var i = 0; i < newKeyframe.Length; i++)
            {
                var frameEnumerable = curve.keys.Where(x => x.time == newFrameTime[i]);
                var newFrameEnumerable = newCurve.keys.Where(x => x.time == newFrameTime[i]);
                // マージ
                if (frameEnumerable.Count() != 0 && newFrameEnumerable.Count() != 0)
                {
                    var frame = frameEnumerable.First();
                    var newframe = newFrameEnumerable.First();
                    var val = newframe.value + frame.value * ratio;
                    newKeyframe[i] = new Keyframe(frame.time, val > 100 ? 100 : val, frame.inTangent, frame.outTangent, frame.inWeight, frame.outWeight);
                    continue;
                }
                // 既存カーブ
                if (frameEnumerable.Count() != 0)
                {
                    var frame = frameEnumerable.First();
                    newKeyframe[i] = new Keyframe(frame.time, frame.value * ratio, frame.inTangent, frame.outTangent, frame.inWeight, frame.outWeight);
                    continue;
                }
                // 新規カーブ
                if (newFrameEnumerable.Count() != 0)
                {
                    var frame = newFrameEnumerable.First();
                    newKeyframe[i] = new Keyframe(frame.time, frame.value, frame.inTangent, frame.outTangent, frame.inWeight, frame.outWeight);
                    continue;
                }
            }
            return newKeyframe;
        }
    }
}