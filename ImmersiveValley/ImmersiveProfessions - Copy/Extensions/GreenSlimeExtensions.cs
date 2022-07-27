namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common;
using Common.ModData;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Globalization;

#endregion using directives

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
public static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    public static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(ModDataIO.Read(slime, "Jumping"));
    }

    /// <summary>Write the necessary mod data fields for this slime to function as a piped slime.</summary>
    public static void MakePipedSlime(this GreenSlime slime, Farmer theOneWhoPipedMe)
    {
        ModDataIO.Write(slime, "Piped", true.ToString());
        ModDataIO.Write(slime, "Piper", theOneWhoPipedMe.UniqueMultiplayerID.ToString());
        ModDataIO.Write(slime, "PipeTimer",
            (30000 / ModEntry.Config.SpecialDrainFactor).ToString(CultureInfo.InvariantCulture));
        ModDataIO.Write(slime, "DoneInflating", false.ToString());
        ModDataIO.Write(slime, "OriginalScale", slime.Scale.ToString(CultureInfo.InvariantCulture));
        ModDataIO.Write(slime, "OriginalHealth", slime.Health.ToString());
        ModDataIO.Write(slime, "OriginalAggroThreshold", slime.moveTowardPlayerThreshold.Value.ToString());

        var fakeFarmerId = slime.GetHashCode();
        if (ModEntry.Host.FakeFarmers.ContainsKey(fakeFarmerId)) return;

        ModEntry.Host.FakeFarmers[fakeFarmerId] = new()
        { UniqueMultiplayerID = fakeFarmerId, currentLocation = slime.currentLocation };
        Log.D($"Created fake farmer with id {fakeFarmerId}.");
    }

    /// <summary>Grow this Slime one stage.</summary>
    public static void Inflate(this GreenSlime slime)
    {
        var originalScale = ModDataIO.Read<float>(slime, "OriginalScale");
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(originalScale * 2f, 2f));
        if (slime.Scale <= 1.4f || slime.Scale < originalScale * 2f &&
            Game1.random.NextDouble() > 0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01) return;

        slime.Health += (int)Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextDouble() < 1.0 / 3.0) slime.addedSpeed += Game1.random.Next(3);
        if (slime.Scale >= 1.8f) slime.willDestroyObjectsUnderfoot = true;

        ModDataIO.Write(slime, "DoneInflating", true.ToString());
    }

    /// <summary>Shrink this Slime one stage.</summary>
    public static void Deflate(this GreenSlime slime)
    {
        var originalScale = ModDataIO.Read<float>(slime, "OriginalScale");
        slime.Scale = Math.Max(slime.Scale / 1.1f, originalScale);
        if (slime.Scale > originalScale) return;

        slime.Health = ModDataIO.Read<int>(slime, "OriginalHealth");
        slime.moveTowardPlayerThreshold.Value = ModDataIO.Read<int>(slime, "OriginalAggroThreshold");
        slime.willDestroyObjectsUnderfoot = false;
        slime.addedSpeed = 0;
        slime.focusedOnFarmers = false;
        ModDataIO.Write(slime, "Piped", false.ToString());
        ModEntry.Player.PipedSlimes.Remove(slime);

        var fakeFarmerId = slime.GetHashCode();
        ModEntry.Host.FakeFarmers.Remove(fakeFarmerId);
        Log.D($"The fake farmer {fakeFarmerId} was destroyed.");
    }

    /// <summary>Decrement the pipe timer for this Slime.</summary>
    public static void Countdown(this GreenSlime slime, double elapsed)
    {
        var pipeTimer = ModDataIO.Read<double>(slime, "PipeTimer");
        if (pipeTimer <= 0.0) return;

        pipeTimer -= elapsed;
        ModDataIO.Write(slime, "PipeTimer", pipeTimer.ToString(CultureInfo.InvariantCulture));
    }
}