namespace DaLion.Professions.Framework;

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion using directives

internal static class MachineTreatmentRegistry
{
    private static readonly Dictionary<string, MachineTreatment> _categories = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets a treatment which overclocks production frequency with electric power.</summary>
    internal static MachineTreatment Overclock { get; } = Register("Overclock");

    /// <summary>Gets a treatment which conditions the vessel for cultured aging and fermentation using Oak Resin.</summary>
    internal static MachineTreatment Fermentation { get; } = Register("Fermentation", I18n.Machines_Coatings_Fermentation());

    /// <summary>Gets a treatment which seasons the processing medium with a sweet glaze using Maple Syrup or Birch Water (SVE).</summary>
    internal static MachineTreatment Glazing { get; } = Register("Glazing", I18n.Machines_Coatings_Glazing());

    /// <summary>Gets a treatment which seals the processing environment for smoking, drying, and extraction using Pine Tar or Fir Wax (SVE).</summary>
    internal static MachineTreatment Sealing { get; } = Register("Sealing", I18n.Machines_Coatings_Sealing());

    /// <summary>Gets a treatment which represents a medium that cannot be treated.</summary>
    internal static MachineTreatment None { get; } = Register("None");

    internal static MachineTreatment Register(string id, string displayName = "")
    {
        if (_categories.TryGetValue(id, out var existing))
        {
            return existing;
        }

        if (string.IsNullOrEmpty(displayName))
        {
            displayName = id;
        }

        var category = new MachineTreatment(id, displayName);
        _categories.Add(id, category);
        return category;
    }

    internal static bool TryGet(string id, [NotNullWhen(true)] out MachineTreatment? category)
    {
        if (_categories.TryGetValue(id, out var existing))
        {
            category = existing;
            return true;
        }

        category = null;
        return false;
    }

    internal static MachineTreatment GetOrRegister(string id)
    {
        return TryGet(id, out var category) ? (MachineTreatment)category : Register(id);
    }

    internal static MachineTreatment FromCatalyst(string id)
    {
        return Lookups.TreatmentByCatalyst.TryGetValue(id, out var treatment) ? treatment : None;
    }
}
