namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Globalization;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
internal static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    internal static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(slime.ReadData("Jumping"));
    }

    /// <summary>Write the necessary mod data fields for this slime to function as a piped slime.</summary>
    internal static void MakePipedSlime(this GreenSlime slime, Farmer theOneWhoPipedMe)
    {
        slime.WriteData("Piped", true.ToString());
        slime.WriteData("Piper", theOneWhoPipedMe.UniqueMultiplayerID.ToString());
        slime.WriteData("PipeTimer", (30000 / ModEntry.Config.UltimateDrainFactor).ToString(CultureInfo.InvariantCulture));
        slime.WriteData("DoneInflating", false.ToString());
        slime.WriteData("OriginalScale", slime.Scale.ToString(CultureInfo.InvariantCulture));
        slime.WriteData("OriginalHealth", slime.Health.ToString());
        slime.WriteData("OriginalAggroThreshold", slime.moveTowardPlayerThreshold.Value.ToString());

        var fakeFarmerId = slime.GetHashCode();
        if (ModEntry.HostState.FakeFarmers.ContainsKey(fakeFarmerId)) return;

        ModEntry.HostState.FakeFarmers[fakeFarmerId] = new()
            {UniqueMultiplayerID = fakeFarmerId, currentLocation = slime.currentLocation};
        Log.D($"Created fake farmer with id {fakeFarmerId}.");
    }

    /// <summary>Grow this Slime one stage.</summary>
    internal static void Inflate(this GreenSlime slime)
    {
        var originalScale = slime.ReadDataAs<float>("OriginalScale");
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(originalScale * 2f, 2f));
        if (slime.Scale <= 1.4f || slime.Scale < originalScale * 2f &&
            Game1.random.NextDouble() > 0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01) return;

        slime.Health += (int) Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (slime.Scale >= 1.8f) slime.willDestroyObjectsUnderfoot = true;

        slime.WriteData("DoneInflating", true.ToString());
    }

    /// <summary>Shrink this Slime one stage.</summary>
    internal static void Deflate(this GreenSlime slime)
    {
        var originalScale = slime.ReadDataAs<float>("OriginalScale");
        slime.Scale = Math.Max(slime.Scale / 1.1f, originalScale);
        if (slime.Scale > originalScale) return;

        slime.Health = slime.ReadDataAs<int>("OriginalHealth");
        slime.moveTowardPlayerThreshold.Value = slime.ReadDataAs<int>("OriginalAggroThreshold");
        slime.willDestroyObjectsUnderfoot = false;
        slime.WriteData("Piped", false.ToString());
        ModEntry.PlayerState.Value.PipedSlimes.Remove(slime);

        var fakeFarmerId = slime.GetHashCode();
        ModEntry.HostState.FakeFarmers.Remove(fakeFarmerId);
        Log.D($"The fake farmer {fakeFarmerId} was destroyed.");
    }

    /// <summary>Decrement the pipe timer for this Slime.</summary>
    internal static void Countdown(this GreenSlime slime, double elapsed)
    {
        var pipeTimer = slime.ReadDataAs<double>("PipeTimer");
        if (pipeTimer <= 0.0) return;

        pipeTimer -= elapsed;
        slime.WriteData("PipeTimer", pipeTimer.ToString(CultureInfo.InvariantCulture));
    }
}