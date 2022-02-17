namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using System;
using StardewValley.Monsters;
using StardewValley;

#endregion using directives

/// <summary>Wrapper for Slimes affected by Piper's Superfluidity.</summary>
internal class SuperfluidSlime
{
    private static int _MillisecondsDuration => (int) (SuperMode.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10);
    
    public GreenSlime Instance { get; }
    public Farmer TheOneWhoPipedMe { get; }
    public float OriginalScale { get; }
    public int OriginalDamage { get; }
    public bool DoneInflating { get; set; }
    public double BuffTimer { get; private set; }

    /// <summary>Construct an instance.</summary>
    /// <param name="instance">The original Slime instance.</param>
    /// <param name="piper">The Piper who created this instance.</param>
    public SuperfluidSlime(GreenSlime instance, Farmer piper)
    {
        Instance = instance;
        TheOneWhoPipedMe = piper;
        OriginalScale = instance.Scale;
        OriginalDamage = instance.DamageToFarmer;
        BuffTimer = _MillisecondsDuration;
    }

    /// <summary>Decrement the Superfluidity timer for this Slime.</summary>
    /// <param name="amount">The number of ticks to deduct.</param>
    public void Countdown(double amount)
    {
        if (Game1.game1.IsActive && Game1.shouldTimePass())
            BuffTimer -= amount;
    }

    /// <summary>Grow this Slime by one stage.</summary>
    public void Inflate()
    {
        Instance.Scale = Math.Min(Instance.Scale * 1.1f, Math.Min(OriginalScale * 2f, 2f));
        if (Instance.Scale <= 1.4f ||
            Game1.random.NextDouble() > 0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01 &&
            Instance.Scale < OriginalScale * 2f) return;

        Instance.DamageToFarmer +=
            (int) Math.Round(Instance.DamageToFarmer * (Instance.Scale - OriginalScale));
        if (Instance.Scale >= 1.8f) Instance.willDestroyObjectsUnderfoot = true;
        DoneInflating = true;
    }

    /// <summary>Shrink this Slime by one stage.</summary>
    public void Deflate()
    {
        Instance.Scale = Math.Max(Instance.Scale / 1.1f, OriginalScale);
        if (Instance.Scale > OriginalScale) return;

        Instance.DamageToFarmer = OriginalDamage;
        Instance.willDestroyObjectsUnderfoot = false;
        ModEntry.State.Value.SuperfluidSlimes.Remove(this);
    }
}