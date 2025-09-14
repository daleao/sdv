namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Objects;

#endregion

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
internal static class GreenSlimeExtensions
{
    /// <summary>Determines whether the <paramref name="slime"/> instance is currently doing its jump animation.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/> instance.</param>
    /// <returns><see langword="true"/> if the <paramref name="slime"/>'s velocity is greater than a minimum threshold, otherwise <see langword="false"/>.</returns>
    internal static bool IsJumping(this GreenSlime slime)
    {
        return Math.Sqrt((slime.xVelocity * slime.xVelocity) + (slime.yVelocity * slime.yVelocity)) >= 1d;
    }

    /// <summary>Gives the hat to the <see cref="GreenSlime"/> instance.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> who is checking.</param>
    /// <param name="hat">A <see cref="Hat"/>.</param>
    /// <remarks>Used for raised Slimes who are not yet piped.</remarks>
    internal static void GiveHatNonPiped(this GreenSlime slime, Farmer who, Hat hat)
    {
        slime.Set_Piped(who, hat);
        who.Items[who.CurrentToolIndex] = null;
        Game1.playSound("dirtyHit");
    }

    /// <summary>Applies the <paramref name="brush"/> to the <see cref="GreenSlime"/> instance.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <param name="brush">A <see cref="Hat"/>.</param>
    /// <returns><see langword="true"/> if the brush was applied, otherwise <see langword="false"/>.</returns>
    internal static bool TryUsePaintbrush(this GreenSlime slime, SObject brush)
    {
        if (brush.ItemId == RedBrushId)
        {
            if (slime.color.R < byte.MaxValue)
            {
                slime.color.R = (byte)Math.Min(slime.color.R + 50, byte.MaxValue);
            }
            else
            {
                slime.color.G = (byte)Math.Max(slime.color.G - 25, 0);
                slime.color.B = (byte)Math.Max(slime.color.B - 25, 0);
            }
        }
        else if (brush.ItemId == GreenBrushId)
        {
            if (slime.color.G < byte.MaxValue)
            {
                slime.color.G = (byte)Math.Min(slime.color.G + 50, byte.MaxValue);
            }
            else
            {
                slime.color.R = (byte)Math.Max(slime.color.R - 25, 0);
                slime.color.B = (byte)Math.Max(slime.color.B - 25, 0);
            }
        }
        else if (brush.ItemId == BlueBrushId)
        {
            if (slime.color.B < byte.MaxValue)
            {
                slime.color.B = (byte)Math.Min(slime.color.B + 50, byte.MaxValue);
            }
            else
            {
                slime.color.R = (byte)Math.Max(slime.color.R - 25, 0);
                slime.color.G = (byte)Math.Max(slime.color.G - 25, 0);
            }
        }
        else if (brush.ItemId == PurpleBrushId)
        {
            if (slime.color.R < byte.MaxValue || slime.color.B < byte.MaxValue)
            {
                slime.color.R = (byte)Math.Min(slime.color.R + 25, byte.MaxValue);
                slime.color.B = (byte)Math.Min(slime.color.B + 25, byte.MaxValue);
            }
            else
            {
                slime.color.G = (byte)Math.Max(slime.color.G - 50, 0);
            }
        }
        else if (brush.ItemId == PrismaticBrushId)
        {
            if (slime.color.R != byte.MaxValue || slime.color.G != byte.MaxValue || slime.color.B != byte.MaxValue)
            {
                return false;
            }

            slime.makePrismatic();
            slime.MaxHealth *= 2;
            slime.Health = slime.MaxHealth;
            slime.DamageToFarmer *= 2;
            slime.resilience.Value *= 2;
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>Changes the <paramref name="slime"/> into a Gold Slime.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    internal static void MakeGoldSlime(this GreenSlime slime)
    {
        slime.Name = "Gold Slime";
        slime.Sprite = new AnimatedSprite($"{UniqueId}_GoldSlime", 0, 16, 16) { SpriteHeight = 24 };
        slime.Sprite.UpdateSourceRect();
        slime.color.Value = Color.White;
    }
}
