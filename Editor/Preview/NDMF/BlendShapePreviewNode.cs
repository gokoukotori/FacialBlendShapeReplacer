
using Gokoukotori.FacialBlendShapeReplacer.Entities.Unity;
using nadena.dev.ndmf.preview;

namespace Gokoukotori.FacialBlendShapeReplacer.Preview.NDMF;

internal class BlendShapePreviewNode : IRenderFilterNode
{
    /// <summary>
    /// このノードが、Update呼出し前のノードと比べて、レンダラーの何を変更したか。
    /// 今回はブレンドシェイプなので<see cref="RenderAspects">Shapes</see>を指定。
    /// <a href="https://ndmf.nadena.dev/api/nadena.dev.ndmf.preview.IRenderFilterNode.html#nadena_dev_ndmf_preview_IRenderFilterNode_WhatChanged">NDMFDocument</a>
    /// </summary>
    public RenderAspects WhatChanged => RenderAspects.Shapes;

    /// <summary>
    /// ブレンドシェイプの数
    /// </summary>
    private readonly int _blendShapeCount;
    /// <summary>
    /// ブレンドシェイプの名前
    /// </summary>
    private readonly List<string> _blendShapeNames;
    /// <summary>
    /// ブレンドシェイプのウェイト
    /// </summary>
    private readonly List<float> _blendShapeWeights;
    public BlendShapePreviewNode(SkinnedMeshRenderer renderer, IReadOnlyList<BlendShapeWeight> list)
    {
        var mesh = renderer.sharedMesh;
        _blendShapeCount = mesh.blendShapeCount;
        _blendShapeNames = new List<string>(_blendShapeCount);
        _blendShapeWeights = new List<float>(_blendShapeCount);
        for (int i = 0; i < _blendShapeCount; i++)
        {
            _blendShapeNames.Add(mesh.GetBlendShapeName(i));
            _blendShapeWeights.Add(mesh.GetBlendShapeFrameWeight(i, 0));
        }
        for (int i = 0; i < _blendShapeCount; i++)
        {
            var blendShape = list.First(x => x.Name == _blendShapeNames[i]);
            if (blendShape is not null)
            {
                _blendShapeWeights[i] += blendShape.Weight;
            }
        }
    }
}