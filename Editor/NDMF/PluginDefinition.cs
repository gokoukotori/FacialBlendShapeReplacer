using System.Threading.Tasks;
using nadena.dev.ndmf.preview;
using nadena.dev.ndmf;
using nadena.dev.ndmf.animator;

[assembly: ExportsPlugin(typeof(Gokoukotori.FacialBlendShapeReplacer.NDMF.PluginDefinition))]
namespace Gokoukotori.FacialBlendShapeReplacer.NDMF
{
    [RunsOnAllPlatforms]
    public sealed class PluginDefinition : Plugin<PluginDefinition>
    {
        public override string DisplayName => "Facial BlendShape Replacer";
        public override string QualifiedName => "gokoukotori.facialBlendShapeReplacer";

        protected override void Configure()
        {
            var sequence = InPhase(BuildPhase.Optimizing);
            //sequence.Run("Empty Pass", _ => { }).PreviewingWith(new ShapesPreview());
        }
    }
}