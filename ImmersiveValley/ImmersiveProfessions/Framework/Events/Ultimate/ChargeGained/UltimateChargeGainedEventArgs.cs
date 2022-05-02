namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Event arguments when <see cref="Ultimate"/> charge value increases.</summary>
public class UltimateChargeGainedEventArgs : UltimateEventArgs
{
    /// <summary>The previous charge value.</summary>
    public double OldValue { get; }

    /// <summary>The new charge value.</summary>
    public double NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeGainedEventArgs(double oldValue, double newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}