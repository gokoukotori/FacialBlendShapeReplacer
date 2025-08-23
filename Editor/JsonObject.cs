using System;
using System.Collections.Generic;

namespace Gokoukotori.FacialBlendShapeReplacer
{
    [Serializable]
    public class JsonData
    {
        public List<string> universalBlendShapeList;
        public List<string> excludeBlendShapeList;
        public List<AvatarBlendShape> avatarBlendShapeList;
    }
    [Serializable]
    public class AvatarBlendShape
    {
        public int avatarId;
        public string avatarName;
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