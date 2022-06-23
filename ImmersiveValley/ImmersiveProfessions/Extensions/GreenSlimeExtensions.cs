namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using System.Globalization;
using StardewValley;
using StardewValley.Monsters;

using Common;
using Common.Data;

#endregion using directives

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
public static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    public static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(ModDataIO.ReadData(slime, "Jumping"));
    }

    /// <summary>Write the necessary mod data fields for this slime to function as a piped slime.</summary>
    public static void MakePipedSlime(this GreenSlime slime, Farmer theOneWhoPipedMe)
    {
        ModDataIO.WriteData(slime, "Piped", true.ToString());
        ModDataIO.WriteData(slime, "Piper", theOneWhoPipedMe.UniqueMultiplayerID.ToString());
        ModDataIO.WriteData(slime, "PipeTimer",
            (30000 / ModEntry.Config.SpecialDrainFactor).ToString(CultureInfo.InvariantCulture));
        ModDataIO.WriteData(slime, "DoneInflating", false.ToString());
        ModDataIO.WriteData(slime, "OriginalScale", slime.Scale.ToString(CultureInfo.InvariantCulture));
        ModDataIO.WriteData(slime, "OriginalHealth", slime.Health.ToString());
        ModDataIO.WriteData(slime, "OriginalAggroThreshold", slime.moveTowardPlayerThreshold.Value.ToString());

        var fakeFarmerId = slime.GetHashCode();
        if (ModEntry.HostState.FakeFarmers.ContainsKey(fakeFarmerId)) return;

        ModEntry.HostState.FakeFarmers[fakeFarmerId] = new()
            {UniqueMultiplayerID = fakeFarmerId, currentLocation = slime.currentLocation};
        Log.D($"Created fake farmer with id {fakeFarmerId}.");
    }

    /// <summary>Grow this Slime one stage.</summary>
    public static void Inflate(this GreenSlime slime)
    {
        var originalScale = ModDataIO.ReadDataAs<float>(slime, "OriginalScale");
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(originalScale * 2f, 2f));
        if (slime.Scale <= 1.4f || slime.Scale < originalScale * 2f &&
            Game1.random.NextDouble() > 0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01) return;

        slime.Health += (int) Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextDouble() < 1.0 / 3.0) slime.addedSpeed += Game1.random.Next(3);
        if (slime.Scale >= 1.8f) slime.willDestroyObjectsUnderfoot = true;

        ModDataIO.WriteData(slime, "DoneInflating", true.ToString());
    }

    /// <summary>Shrink this Slime one stage.</summary>
    public static void Deflate(this GreenSlime slime)
    {
        var originalScale = ModDataIO.ReadDataAs<float>(slime, "OriginalScale");
        slime.Scale = Math.Max(slime.Scale / 1.1f, originalScale);
        if (slime.Scale > originalScale) return;

        slime.Health = ModDataIO.ReadDataAs<int>(slime, "OriginalHealth");
        slime.moveTowardPlayerThreshold.Value = ModDataIO.ReadDataAs<int>(slime, "OriginalAggroThreshold");
        slime.willDestroyObjectsUnderfoot = false;
        slime.addedSpeed = 0;
        slime.focusedOnFarmers = false;
        ModDataIO.WriteData(slime, "Piped", false.ToString());
        ModEntry.PlayerState.PipedSlimes.Remove(slime);

        var fakeFarmerId = slime.GetHashCode();
        ModEntry.HostState.FakeFarmers.Remove(fakeFarmerId);
        Log.D($"The fake farmer {fakeFarmerId} was destroyed.");
    }

    /// <summary>Decrement the pipe timer for this Slime.</summary>
    public static void Countdown(this GreenSlime slime, double elapsed)
    {
        var pipeTimer = ModDataIO.ReadDataAs<double>(slime, "PipeTimer");
        if (pipeTimer <= 0.0) return;

        pipeTimer -= elapsed;
        ModDataIO.WriteData(slime, "PipeTimer", pipeTimer.ToString(CultureInfo.InvariantCulture));
    }
}