using System.Threading.Tasks;
using Gokoukotori.FacialBlendShapeReplacer.Entities.Unity;
using nadena.dev.ndmf.preview;
using UnityEngine.Pool;

namespace Gokoukotori.FacialBlendShapeReplacer.Preview.NDMF
{
    /// <summary>
    /// NDMFプレビュー用クラス
    /// 何もわからん
    /// </summary>
    internal class NDMFPreview : IRenderFilter
    {
        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            throw new NotImplementedException();
        }
    }
}