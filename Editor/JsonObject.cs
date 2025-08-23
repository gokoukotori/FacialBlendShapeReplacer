using System.Collections.Generic;
using System;


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
        public List<BlendShapeMap> sourceBlendShapeMap;
        public List<TargetBlendShape> targetBlendShapeMap;
        public List<string> excludeBlendShapeList;
    }
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
    [Serializable]
    public class TargetBlendShape
    {
        public string universalBlendShape;
        public string target;
    }
}