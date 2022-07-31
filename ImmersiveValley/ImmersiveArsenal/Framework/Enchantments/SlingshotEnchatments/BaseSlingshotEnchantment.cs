namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Projectiles;
using StardewValley.Tools;
using System.Xml.Serialization;

#endregion using directives

[XmlType("Mods_DaLion_BaseSlingshotEnchantment")]
public class BaseSlingshotEnchantment : BaseEnchantment
{
    public override bool CanApplyTo(Item item) => item is Slingshot && ModEntry.Config.NewSlingshotEnchants;

    public void OnFire(Slingshot slingshot, BasicProjectile? projectile, GameLocation location, Farmer farmer)
    {
        _OnFire(slingshot, projectile, location, farmer);
    }

    protected virtual void _OnFire(Slingshot slingshot, BasicProjectile? projectile, GameLocation location, Farmer farmer)
    {
    }

    protected virtual void _OnCollisionWithMonster(Slingshot slingshot, BasicProjectile? projectile, GameLocation location, Farmer farmer)
    {
    }
}