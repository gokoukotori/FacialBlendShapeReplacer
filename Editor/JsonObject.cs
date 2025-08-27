using System;
using System.Collections.Generic;

namespace Gokoukotori.FacialBlendShapeReplacer
{
    [Serializable]
    public class Avatar
    {
        public string guid;
        public string name;
        public string blendshapeInfoFileGuid;
    }
    [Serializable]
    public class Global
    {
        public string avatarJsonGuid;
        public List<string> avatar2UniversalMap;
        public List<string> excludeBlendShapeList;
    }

    [Serializable]
    public class AvatarBlendShape
    {
        public List<Avatar2UniversalBlendShape> universalBlendShapeMap;
        public List<string> excludeBlendShapeList;
    }
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
    [Serializable]
    public class BlendShapeRatio
    {
        public string blendShape;
        public float ratio;
    }
}