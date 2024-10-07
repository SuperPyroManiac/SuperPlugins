using Rage;

namespace PyroCommon.Objects;

public struct Location
{
    public Vector3 Position { get; set; }
    public float Heading { get; set; }

    public Location(Vector3 position, float heading = 0)
    {
        Position = position;
        Heading = heading;
    }

    public Location(float x, float y, float z, float heading = 0)
    {
        Position = new Vector3(x, y, z);
        Heading = heading;
    }

    public Location Default => new Location(Vector3.Zero);
}