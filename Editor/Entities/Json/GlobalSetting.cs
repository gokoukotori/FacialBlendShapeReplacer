namespace Gokoukotori.FacialBlendShapeReplacer.Entities.Json;

[Serializable]
public class GlobalSetting
{
    public string avatarJsonGuid;
    public List<string> universalBlendShapeList;
    public List<string> excludeBlendShapeList;
}