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
        return I18n.Enchantments_Echo_Name();
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

        var facingDirectionVector = ((Direction)firer.FacingDirection).ToVector() * 64f;

        // do clockwise projectile
        var startingPosition = firedProjectile.position.Value + facingDirectionVector.Rotate(30);
        var clockwise = new EchoProjectile(
                firedProjectile,
                startingPosition,
                location,
                firer,
                0.7f);
        firer.currentLocation.projectiles.Add(clockwise);

        // do anti-clockwise projectile
        startingPosition = firedProjectile.position.Value + facingDirectionVector.Rotate(-30);
        var antiClockwise = new EchoProjectile(
                firedProjectile,
                startingPosition,
                location,
                firer,
                0.49f);
        firer.currentLocation.projectiles.Add(antiClockwise);
    }
}
