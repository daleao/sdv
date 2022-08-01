namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Stardew;
using Framework.VirtualProperties;
using StardewValley.Monsters;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
public static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    public static bool IsJumping(this GreenSlime slime) => !string.IsNullOrEmpty(slime.Read("Jumping"));

    /// <summary>Grow this Slime one stage.</summary>
    public static void Inflate(this GreenSlime slime)
    {
        var originalScale = slime.get_OriginalScale();
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(originalScale * 2f, 2f));
        if (slime.Scale <= 1.4f || slime.Scale < originalScale * 2f &&
            Game1.random.NextDouble() > 0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01) return;

        slime.MaxHealth += (int)Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.Health = slime.MaxHealth;
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextDouble() < 1d / 3d) slime.addedSpeed += Game1.random.Next(3);
        if (slime.Scale >= 1.8f) slime.willDestroyObjectsUnderfoot = true;

        slime.set_Inflated(true);
    }

    /// <summary>Shrink this Slime one stage.</summary>
    public static void Deflate(this GreenSlime slime)
    {
        var originalScale = slime.get_OriginalScale();
        slime.Scale = Math.Max(slime.Scale / 1.1f, originalScale);
        if (slime.Scale > originalScale) return;

        slime.MaxHealth = slime.get_OriginalHealth();
        slime.Health = slime.MaxHealth;
        slime.moveTowardPlayerThreshold.Value = slime.get_OriginalRange();
        slime.willDestroyObjectsUnderfoot = false;
        slime.addedSpeed = 0;
        slime.focusedOnFarmers = false;
    }
}