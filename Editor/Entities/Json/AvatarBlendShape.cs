namespace Gokoukotori.FacialBlendShapeReplacer.Entities.Json;

[Serializable]
public class AvatarBlendShape
{
    public List<Avatar2UniversalBlendShape> avatar2UniversalMap;
    public List<string> excludeBlendShapeList;
}