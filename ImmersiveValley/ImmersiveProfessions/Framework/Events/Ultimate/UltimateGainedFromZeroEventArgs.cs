namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Event arguments when the <see cref="Ultimate"/> charge value increases while it was previously empty.</summary>
public class UltimateGainedFromZeroEventArgs : UltimateChargeGainedEvent
{
    /// <summary>The new charge value.</summary>
    public int NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateGainedFromZeroEventArgs(int newValue)
    {
        NewValue = newValue;
    }
}