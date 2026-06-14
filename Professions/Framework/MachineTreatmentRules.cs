namespace DaLion.Professions.Framework;

/// <summary>Describes one or more treatments that can be applied by a prestiged Artisan to a specific machine.</summary>
/// <param name="Default">The default treatment process used by this machine.</param>
/// <param name="Overrides">Optional overrides by output category or item id, if any.</param>
internal record MachineTreatmentRules(MachineTreatmentCategory Default, Dictionary<string, MachineTreatmentCategory> Overrides);
