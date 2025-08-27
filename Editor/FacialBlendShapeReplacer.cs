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
        /// <param name="source">変換元アバター</param>
        /// <param name="target">変換先アバター</param>
        /// <param name="targetClip">変換対象アニメーションクリップ</param>
        /// <param name="excludeBlendShape">除外するブレンドシェイプ</param>
        /// <returns>変換されたアニメーションクリップ</returns>
        public AnimationClip Execute(AvatarBlendShape source, AvatarBlendShape target, AnimationClip targetClip, IReadOnlyList<string> excludeBlendShape)
        {
            var newClip = new AnimationClip();
            var bindings = AnimationUtility.GetCurveBindings(targetClip);
            var universalBlendShapeMap = ResolveBlendShape(source.universalBlendShapeMap, target.universalBlendShapeMap);
            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(targetClip, binding);
                // bodyのblendShapeだけ対応
                // それ以外は何もせず転記
                if (binding.type == typeof(SkinnedMeshRenderer) && Regex.IsMatch(binding.propertyName, @"^(blendShape\.).*") && binding.path == "Body")
                {
                    // 対象外は除外
                    if (excludeBlendShape.Count(x => ("blendShape." + x) == binding.propertyName) != 0) continue;
                    if (source.excludeBlendShapeList.Count(x => ("blendShape." + x) == binding.propertyName) != 0) continue;

                    // 名称がイコールとなるものはjsonに記載しない
                    // つまりそのまま転記する
                    var blendShapeMap = universalBlendShapeMap.Find(x => ("blendShape." + x.blendShape) == binding.propertyName);
                    if (blendShapeMap is null)
                    {
                        var clipCurve = AnimationUtility.GetEditorCurve(newClip, binding);
                        // newCurve側のキーが重複して上書きされる可能性があるので重複した場合は値を足す動作に変更する
                        AnimationUtility.SetEditorCurve(newClip, binding, clipCurve is null ? curve : new AnimationCurve(MergeFrame(curve, clipCurve, 1f)));
                        continue;
                    }
                    // 名寄せを行う
                    var newBinding = new EditorCurveBinding
                    {
                        path = binding.path,
                        type = binding.type,
                        propertyName = "blendShape." + blendShapeMap.taegetBlendShape
                    };
                    var newClipCurve = AnimationUtility.GetEditorCurve(newClip, newBinding);
                    // newCurve側のキーが重複して上書きされる可能性があるので重複した場合は値を足す動作に変更する
                    AnimationUtility.SetEditorCurve(newClip, newBinding, newClipCurve is null ? curve : new AnimationCurve(MergeFrame(curve, newClipCurve, 1f)));
                    continue;
                }
                else
                {
                    AnimationUtility.SetEditorCurve(newClip, binding, curve);
                }
            }
            return newClip;
        }

        public class BlendShapeRatioMap : BlendShapeRatio
        {
            public string taegetBlendShape;
        }
        /// <summary>
        /// 考える必要があるのは 元-名寄せ表-先
        ///                    元-名寄せ表
        ///                       名寄せ表-先
        /// のパターン
        /// </summary>
        /// <param name="sourceUniversalBlendShape"></param>
        /// <param name="targetUniversalBlendShape"></param>
        /// <returns></returns>
        private List<BlendShapeRatioMap> ResolveBlendShape(List<Avatar2UniversalBlendShape> sourceUniversalBlendShape, List<Avatar2UniversalBlendShape> targetUniversalBlendShape)
        {
            // 元-名寄せ表-先
            var query1 = sourceUniversalBlendShape
            .Join(
                targetUniversalBlendShape,
                source => source.universal?.blendShape,
                target => target.universal?.blendShape,
                (source, target) => new BlendShapeRatioMap
                {
                    blendShape = source.blendShape,
                    ratio = target.universal?.ratio ?? 1f,
                    taegetBlendShape = target.blendShape
                }
            ).Select(x => new BlendShapeRatioMap
            {
                blendShape = x.blendShape,
                ratio = x.ratio,
                taegetBlendShape = x.taegetBlendShape
            }).ToList();
            // 元-名寄せ表
            var query2 = sourceUniversalBlendShape.Where(source => !query1.Exists(Y => Y.blendShape == source.blendShape)).Select(source => new BlendShapeRatioMap
            {
                blendShape = source.universal.blendShape,
                taegetBlendShape = source.blendShape,
                ratio = source.universal.ratio
            }).ToList();
            //   名寄せ表-先
            var query3 = targetUniversalBlendShape.Where(target => !query1.Exists(Y => Y.blendShape == target.blendShape)).Select(target => new BlendShapeRatioMap
            {
                blendShape = target.blendShape,
                taegetBlendShape = target.universal.blendShape,
                ratio = target.universal.ratio
            }).ToList();
            query1.AddRange(query2);
            query1.AddRange(query3);
            return query1;
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
                    var val = frame.value * ratio;
                    newKeyframe[i] = new Keyframe(frame.time, val > 100 ? 100 : val, frame.inTangent, frame.outTangent, frame.inWeight, frame.outWeight);
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