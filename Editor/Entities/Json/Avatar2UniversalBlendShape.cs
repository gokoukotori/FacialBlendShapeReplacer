
namespace Gokoukotori.FacialBlendShapeReplacer.Entities.Json;

/// <summary>
/// 原則1:1
/// それ以外はどうやっても機械的に対応できません
/// 例えばuniversalBlendShapeに対してtargetが複数あった場合、どのシェイプキーに加算すれば良いか判断はできないでしょ？
/// </summary>
[Serializable]
public class Avatar2UniversalBlendShape
{
    public string blendShape;
    public BlendShapeRatio universal;
}