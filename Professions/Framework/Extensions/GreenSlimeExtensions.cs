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

    /// <summary>Checks for actions on the instance.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> who is checking.</param>
    /// <remarks>Used for raised Slimes who are not yet piped.</remarks>
    internal static void CheckActionNonPiped(this GreenSlime slime, Farmer who)
    {
        if (!who.HasProfession(Profession.Piper) || who.Items.Count <= who.CurrentToolIndex ||
            who.Items[who.CurrentToolIndex] is not Hat hat)
        {
            return;
        }

        slime.Set_Piped(who);
        who.Items[who.CurrentToolIndex] = null;
        slime.Get_Piped()!.Hat = hat;
        Game1.playSound("dirtyHit");
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
