namespace DeadlyWeapons.Configs;

public class Weapon
{
    public string Name { get; set; }
    public string WeaponHash { get; set; }
    public string WeaponType {get; set;}
    public bool PanicIgnore { get; set; }
    public float DamageMultiplier { get; set; }
}