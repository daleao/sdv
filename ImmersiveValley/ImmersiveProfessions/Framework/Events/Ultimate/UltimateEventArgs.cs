namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

using Framework.Ultimate;

#endregion using directives

/// <summary>Base event arguments when an <see cref="Ultimate"/> event is raised.</summary>
public class UltimateEventArgs : EventArgs
{
    public string Which { get; }
    public int MaxChargeValue { get; }

    /// <summary>Construct an instance.</summary>
    internal UltimateEventArgs()
    {
        Which = ModEntry.PlayerState.RegisteredUltimate.Index.ToString();
        MaxChargeValue = Ultimate.MaxValue;
    }
}