using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public static class ColorToFloat
{
    public static float4 ToFloat(Color color)
    {
        return new float4(color.r, color.g, color.b, color.a);
    }
}