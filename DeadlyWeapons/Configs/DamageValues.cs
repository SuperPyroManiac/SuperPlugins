namespace DeadlyWeapons.Configs;

public class DamageConfigurations
{
    public DamageWithArmor PlayerDamage { get; set; }
    public DamageWithArmor NpcDamage { get; set; }
}

public class DamageWithArmor
{
    public DamageValues WithArmor { get; set; }
    public DamageValues WithoutArmor { get; set; }
}

public class DamageValues
{
    public float Head { get; set; }
    public float Torso { get; set; }
    public float Arms { get; set; }
    public float Legs { get; set; }
}