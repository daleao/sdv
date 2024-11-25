namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using Netcode;
using StardewValley.Buffs;

#endregion using directives

/// <summary>Extensions for the <see cref="BuffEffects"/> class.</summary>
public static class BuffEffectsExtensions
{
    /// <summary>Counts the number of non-zero stat values in the specified <paramref name="effects"/>.</summary>
    /// <param name="effects">The <see cref="BuffEffects"/>.</param>
    /// <returns>The number of non-zero stat values in the specified <paramref name="effects"/>.</returns>
    public static int CountValues(this BuffEffects effects)
    {
        var additiveFields = Reflector.GetUnboundFieldGetter<BuffEffects, NetFloat[]>(effects, "AdditiveFields").Invoke(effects);
        var multiplicativeFields = Reflector.GetUnboundFieldGetter<BuffEffects, NetFloat[]>(effects, "MultiplicativeFields").Invoke(effects);
        return additiveFields.Count(f => f.Value != 0f) + multiplicativeFields.Count(f => f.Value != 0f);
    }
}
