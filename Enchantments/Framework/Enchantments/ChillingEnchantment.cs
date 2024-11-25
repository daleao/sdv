namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Enchantments;
using DaLion.Core.Framework.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Progressively chill enemies on hit for 2 seconds, freezing after stacking 3 times.</summary>
[XmlType("Mods_DaLion_ChillingEnchantment")]
public sealed class ChillingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Chilling_Name();
    }

    /// <inheritdoc />
    public override void OnDealtDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, int amount)
    {
        base.OnDealtDamage(monster, location, who, fromBomb, amount);
        monster.Chill(2000, 0.2f, 0.5f);
        SoundBox.ChillingShot.PlayLocal(location);
    }
}
