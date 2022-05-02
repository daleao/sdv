namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Event arguments when the <see cref="Ultimate"/> charge value increases.</summary>
public class UltimateChargeGainedEventArgs : UltimateChargeGainedEvent
{
    /// <summary>The previous charge value.</summary>
    public int OldValue { get; }

    /// <summary>The new charge value.</summary>
    public int NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeGainedEventArgs(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}