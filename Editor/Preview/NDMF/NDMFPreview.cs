using System.Threading.Tasks;
using Gokoukotori.FacialBlendShapeReplacer.Entities.Unity;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using nadena.dev.ndmf.runtime;
using UnityEngine.Pool;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace Gokoukotori.FacialBlendShapeReplacer.Preview.NDMF
{
    /// <summary>
    /// NDMFプレビュー用クラス
    /// 何もわからん
    /// </summary>
    internal class NDMFPreview : IRenderFilter
    {
        /// <summary>
        /// たぶんどのレンダラーをプレビューの対象とするか指定するやつじゃない？
        /// わからん
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            var groups = new List<RenderGroup>();
            foreach (var root in context.GetComponentsByType<VRCAvatarDescriptor>())
            {
                if (!context.ActiveInHierarchy(root.gameObject)) continue;
                groups.Add(RenderGroup.For(GetFaceRenderer(root)).WithData((root.gameObject, root.gameObject.GetFullPath())));
            }
            return groups.ToImmutableList();
        }
        public SkinnedMeshRenderer GetFaceRenderer(VRCAvatarDescriptor descriptor)
        {
            SkinnedMeshRenderer faceRenderer = null;
            if (descriptor.lipSync == VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape && descriptor.VisemeSkinnedMesh != null)
            {
                faceRenderer = descriptor.VisemeSkinnedMesh;
            }
            else if (descriptor.customEyeLookSettings.eyelidType == VRCAvatarDescriptor.EyelidType.Blendshapes && descriptor.customEyeLookSettings.eyelidsSkinnedMesh != null)
            {
                faceRenderer = descriptor.customEyeLookSettings.eyelidsSkinnedMesh;
            }
            else
            {
                var avatarRoot = descriptor.gameObject.transform;
                for (int i = 0; i < avatarRoot.childCount; i++)
                {
                    var child = avatarRoot.GetChild(i);
                    if (child != null && child.name == "Body")
                    {
                        faceRenderer = child.GetComponentNullable<SkinnedMeshRenderer>();
                        if (faceRenderer != null) { break; }
                    }
                }
            }
            return faceRenderer;
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            throw new NotImplementedException();
        }
    }
}