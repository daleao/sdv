namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Linq;
using System.Xml.Serialization;
using DaLion.Core.Framework.Enchantments;
using DaLion.Enchantments.Framework.Projectiles;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Xna;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>Summons 2 "echoes" of the fired projectile. The echoes automatically aim at the nearest enemy after a short delay.</summary>
[XmlType("Mods_DaLion_EchoEnchantment")]
public sealed class EchoEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Runaan_Name();
    }

    /// <inheritdoc />
    public override void OnFire(
        Slingshot slingshot,
        BasicProjectile firedProjectile,
        GameLocation location,
        Farmer firer)
    {
        if (!location.characters.OfType<Monster>().Any())
        {
            return;
        }

        var facingDirectionVector = ((FacingDirection)firer.FacingDirection).ToVector() * 64f;

        // do clockwise projectile
        var runaanStartingPosition = firedProjectile.position.Value + facingDirectionVector.Rotate(30);
        var clockwise = new RunaanProjectile(
                firedProjectile,
                runaanStartingPosition,
                location,
                firer);
        firer.currentLocation.projectiles.Add(clockwise);

        // do anti-clockwise projectile
        runaanStartingPosition = firedProjectile.position.Value + facingDirectionVector.Rotate(-30);
        var antiClockwise = new RunaanProjectile(
                firedProjectile,
                runaanStartingPosition,
                location,
                firer);
        firer.currentLocation.projectiles.Add(antiClockwise);
    }
}
