namespace DaLion.Professions.Framework;

#region using directives

using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Describes one or more treatments that can be applied by a prestiged Artisan to a specific machine.</summary>
/// <param name="Default">The default treatment process used by this machine.</param>
/// <param name="Overrides">Optional overrides by output category or item id, if any.</param>
internal record MachineTreatmentRules(MachineTreatmentCategory Default, Dictionary<string, MachineTreatmentCategory> Overrides)
{
    /// <summary>Determines whether the specified <see cref="MachineTreatmentCategory"/> is contained in any of the rules.</summary>
    /// <param name="test">A <see cref="MachineTreatmentCategory"/> to test.</param>
    /// <returns><see langword="true"/> if <paramref name="test"/> is either the default rule or an override rule, otherwise <see langword="false"/>.</returns>
    internal bool Contains(MachineTreatmentCategory test)
    {
        return this.Default.Collect(this.Overrides.Values).Contains(test);
    }
}
