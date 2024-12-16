﻿namespace DaLion.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Extensions;
using DaLion.Shared.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Enchantments;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Lava Katana.</summary>
[XmlType("Mods_DaLion_LavaEnchantment")]
public sealed class LavaEnchantment : BaseWeaponEnchantment
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

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
    public override void OnDealtDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, int amount)
    {
        if (this._random.NextDouble() < 0.2)
        {
            monster.Burn(who);
        }

        var monsterBox = monster.GetBoundingBox();
        var sprites = new TemporaryAnimatedSprite(
            362,
            Game1.random.Next(50, 120),
            6,
            1,
            new Vector2(monsterBox.Center.X - 32, monsterBox.Center.Y - 32),
            flicker: false,
            flipped: false)
            { color = Color.OrangeRed };
        Reflector
            .GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer")
            .Invoke()
            .broadcastSprites(location, sprites);
    }
}
