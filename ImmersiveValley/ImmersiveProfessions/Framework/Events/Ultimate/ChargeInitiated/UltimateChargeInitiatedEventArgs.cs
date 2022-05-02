namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Event arguments when <see cref="Ultimate"/> charge value increases while it was previously empty.</summary>
internal class UltimateChargeInitiatedEventArgs : UltimateEventArgs
{
    /// <summary>The new charge value.</summary>
    public double NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeInitiatedEventArgs(double newValue)
    {
        NewValue = newValue;
    }
}