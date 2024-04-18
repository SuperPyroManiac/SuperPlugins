using Rage;

namespace PyroCommon.API;

public struct Location
{
    public Vector3 Position;
    public float Heading;
    
    public Location(Vector3 position, float heading)
    {
        Position = position;
        Heading = heading;
    }

    public Location(float x, float y, float z, float heading)
    {
        Position = new Vector3(x, y, z);
        Heading = heading;
    }
}