using Rage;

namespace PyroCommon.PyroFunctions.Extensions;

public static class Msc
{
    public static bool NotValid(this Entity entity)
    {
        return entity == null || !entity.Exists();
    }
}