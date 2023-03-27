namespace DaLion.Overhaul.Modules.Weapons.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Weapons.Events;
using DaLion.Overhaul.Modules.Weapons.Projectiles;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
[XmlType("Mods_DaLion_BlessedEnchantment")]
public class BlessedEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.Read<bool>(DataKeys.Cursed))
        {
            return;
        }

        EventManager.Disable<CurseUpdateTickedEvent>();
        who.CurrentTool.Write(DataKeys.CursePoints, null);
    }

    /// <inheritdoc />
    protected override void _OnSwing(MeleeWeapon weapon, Farmer farmer)
    {
        base._OnSwing(weapon, farmer);
        if (farmer.health < farmer.maxHealth)
        {
            return;
        }

        var facingDirection = (FacingDirection)farmer.FacingDirection;
        var facingVector = facingDirection.ToVector();
        var startingPosition = farmer.getStandingPosition() + (facingVector * 64f) - new Vector2(32f, 32f);
        var velocity = facingVector * 10f;
        var rotation = (float)Math.PI / 180f * 32f;
        farmer.currentLocation.projectiles.Add(new LightBeamProjectile(
            weapon,
            farmer,
            startingPosition,
            velocity.X,
            velocity.Y,
            rotation));
    }
}
