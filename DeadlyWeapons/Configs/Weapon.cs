namespace DeadlyWeapons.Configs;

public class Weapon
{
    public string Name { get; set; }
    public long WeaponHash { get; set; }
    public string WeaponType {get; set;}
    public bool PanicIgnore { get; set; }
}