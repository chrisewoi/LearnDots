using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BounceAuthoring : MonoBehaviour
{
    public float value;
}

public struct Bounce : IComponentData
{
    public float value;
}
public class BounceAuthoringBaker : Baker<BounceAuthoring>
{
    public override void Bake(BounceAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Bounce
        {
            value = authoring.value,
        });
    }
}