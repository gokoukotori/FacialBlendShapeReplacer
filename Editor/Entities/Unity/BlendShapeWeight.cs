namespace Gokoukotori.FacialBlendShapeReplacer.Entities.Unity;

[Serializable]
public class BlendShapeWeight
{

    public BlendShapeWeight(string name, float weight)
    {
        Name = name;
        Weight = weight;
    }
    [SerializeField]
    public string Name;

    [SerializeField]
    public float Weight;
    public bool IsWeightOver => Weight > 100f;
}