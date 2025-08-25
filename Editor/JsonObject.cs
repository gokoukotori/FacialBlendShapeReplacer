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
        public List<string> universalBlendShapeList;
        public List<string> excludeBlendShapeList;
    }

    [Serializable]
    public class AvatarBlendShape
    {
        public List<BlendShapeMap> notExistblendShapeMap;
        public List<TargetBlendShape> universalBlendShapeMap;
        public List<string> excludeBlendShapeList;
    }
    /// <summary>
    /// 複数のブレンドシェイプで合成したい場合
    /// </summary>
    [Serializable]
    public class BlendShapeMap
    {
        public string source;
        public List<BlendShapeRatio> target;
    }
    [Serializable]
    public class BlendShapeRatio
    {
        public string universalBlendShape;
        public float ratio;
    }
    /// <summary>
    /// 原則1:1
    /// それ以外はどうやっても機械的に対応できません
    /// 例えばuniversalBlendShapeに対してtargetが複数あった場合、どのシェイプキーに加算すれば良いか判断はできないでしょ？
    /// </summary>
    [Serializable]
    public class TargetBlendShape
    {
        public string universalBlendShape;
        public string target;
    }
}