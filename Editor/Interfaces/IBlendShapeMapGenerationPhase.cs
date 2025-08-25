using Gokoukotori.FacialBlendShapeReplacer;
using System.Collections.Generic;

namespace Gokoukotori.FacialBlendShapeReplacer.Interfaces
{
    public interface IBlendShapeMapGenerationPhase
    {
        public List<TargetBlendShape> Execute(List<TargetBlendShape> list);
    }
}