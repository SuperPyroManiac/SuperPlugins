using Rage;

namespace PyroCommon.API;

public static class Vector4Extensions
{
    public static Vector3 ToVector3(this Vector4 input)
    {
        return new Vector3(input.X, input.Y, input.Z);
    }
}