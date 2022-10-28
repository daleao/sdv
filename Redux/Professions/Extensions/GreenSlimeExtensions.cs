namespace DaLion.Redux.Professions.Extensions;

#region using directives

using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
internal static class GreenSlimeExtensions
{
    /// <summary>Determines whether the <paramref name="slime"/> is currently jumping.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="slime"/> if mid jump animation, otherwise <see langword="false"/>.</returns>
    internal static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(slime.Read(DataFields.Jumping));
    }

    /// <summary>Grows this <paramref name="slime"/> one stage.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    internal static void Inflate(this GreenSlime slime)
    {
        var originalScale = slime.Get_OriginalScale();
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(originalScale * 2f, 2f));
        if (slime.Scale <= 1.4f || (slime.Scale < originalScale * 2f &&
                                    Game1.random.NextDouble() > 0.2 - (Game1.player.DailyLuck / 2) -
                                    (Game1.player.LuckLevel * 0.01)))
        {
            return;
        }

        slime.MaxHealth += (int)Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.Health = slime.MaxHealth;
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextDouble() < 1d / 3d)
        {
            slime.addedSpeed += Game1.random.Next(3);
        }

        if (slime.Scale >= 1.8f)
        {
            slime.willDestroyObjectsUnderfoot = true;
        }

        slime.Set_Inflated(true);
    }

    /// <summary>Shrinks this <paramref name="slime"/> one stage.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    internal static void Deflate(this GreenSlime slime)
    {
        var originalScale = slime.Get_OriginalScale();
        slime.Scale = Math.Max(slime.Scale / 1.1f, originalScale);
        if (slime.Scale > originalScale)
        {
            return;
        }

        slime.MaxHealth = slime.Get_OriginalHealth();
        slime.Health = slime.MaxHealth;
        slime.moveTowardPlayerThreshold.Value = slime.Get_OriginalRange();
        slime.willDestroyObjectsUnderfoot = false;
        slime.addedSpeed = 0;
        slime.focusedOnFarmers = false;
    }
}
