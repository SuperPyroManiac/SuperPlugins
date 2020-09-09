using Rage;

namespace DeadlyWeapons2.Modules
{
    internal static class RubberBullet
    {
        internal static bool NonLeathal { get; set; }
        
        internal static void RubberBullets()
        {
            NonLeathal = !NonLeathal;
            Game.DisplayHelp("Using Non Lethal Ammo:~y~ " + NonLeathal);
        }
    }
}